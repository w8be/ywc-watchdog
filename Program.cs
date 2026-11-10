using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// 1. Configure Host & Register SSL Bypass Client
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("RadioClient")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

builder.Services.Configure<WatchdogSettings>(builder.Configuration.GetSection("WatchdogSettings"));
builder.Services.AddHostedService<RadioWatchdogWorker>();

var host = builder.Build();
host.Run();

// 2. Worker Service
public class RadioWatchdogWorker : BackgroundService
{
    private readonly ILogger<RadioWatchdogWorker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly WatchdogSettings _settings;
    private int _swrViolationCount = 0;
    private DateTime? _txStartTime = null;

    public RadioWatchdogWorker(ILogger<RadioWatchdogWorker> logger, IHttpClientFactory httpClientFactory, IOptions<WatchdogSettings> settings)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting YWC REST Watchdog Service polling status endpoint...");
        _logger.LogInformation("[CONFIG] Polling API: {Url}", _settings.YwcApiUrl);
        _logger.LogInformation("[CONFIG] Shelly IP: {Ip} (Channel: {Channel})", _settings.ShellyIp, _settings.ShellyChannel);
        _logger.LogInformation("[CONFIG] Max TX Duration: {Duration} minutes", _settings.MaxTxDurationMinutes);
        _logger.LogInformation("[CONFIG] Max Allowed SWR Meter: {Swr}", _settings.MaxSwr);
        _logger.LogInformation("[CONFIG] Allowed Antennas: [{Antennas}]", string.Join(", ", _settings.AllowedAntennas));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("RadioClient");
                var response = await client.GetAsync(_settings.YwcApiUrl, stoppingToken);

                if (response.IsSuccessStatusCode)
                {
                    var radioStatus = await response.Content.ReadFromJsonAsync<RadioStatus>(cancellationToken: stoppingToken);
                    if (radioStatus != null)
                    {
                        EvaluateRadioState(radioStatus);
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to poll radio status. HTTP Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while polling radio status API.");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private void EvaluateRadioState(RadioStatus status)
    {
        bool isTransmitting = status.PowerMeter > 0;

        if (isTransmitting)
        {
            // 1. Check if the antenna is allowed FIRST before allowing TX logic to proceed
            string activeAntennaStr = status.ActiveVfo == 1 ? status.VfoB?.Antenna ?? "1" : status.VfoA?.Antenna ?? "1";

            if (int.TryParse(activeAntennaStr, out int currentAntenna))
            {
                if (!_settings.AllowedAntennas.Contains(currentAntenna))
                {
                    _logger.LogCritical("Invalid Antenna detected ({Antenna}) during transmission. Executing emergency cutoff!", currentAntenna);
                    TriggerEmergencyCutoffAsync().Wait();
                    return;
                }
            }

            if (_txStartTime == null)
            {
                _txStartTime = DateTime.UtcNow;
                _logger.LogInformation("Transmit started. Power: {Power}, Raw SWR: {RawSwr}", status.PowerMeter, status.SwrMeter);
            }

            var txDuration = (DateTime.UtcNow - _txStartTime.Value).TotalMinutes;
            if (txDuration > _settings.MaxTxDurationMinutes)
            {
                _logger.LogCritical("Max TX duration exceeded ({Duration:F2} mins). Triggering emergency PTT cutoff!", txDuration);
                TriggerEmergencyCutoffAsync().Wait();
            }

            // Interpolate raw byte to calibrated SWR ratio using configuration mapping
            double calibratedSwr = GetCalibratedSwr(status.SwrMeter);

            if (calibratedSwr > _settings.MaxSwr)
            {
                _swrViolationCount++;
                _logger.LogWarning("High SWR detected: {CalibratedSwr:F2} (Raw: {Raw}, Violation count: {Count})", calibratedSwr, status.SwrMeter, _swrViolationCount);

                if (_swrViolationCount >= 2)
                {
                    _logger.LogCritical("SWR threshold breached sustained limit ({CalibratedSwr:F2}). Executing emergency cutoff!", calibratedSwr);
                    TriggerEmergencyCutoffAsync().Wait();
                }
            }
            else
            {
                _swrViolationCount = 0;
            }
        }
        else
        {
            if (_txStartTime != null)
            {
                _logger.LogInformation("Transmit ended. Resetting timer.");
                _txStartTime = null;
                _swrViolationCount = 0;
            }
        }
    }

    private double GetCalibratedSwr(double rawValue)
    {
        var points = _settings.SwrCalibration;

        if (points == null || points.Count == 0)
            return rawValue;

        if (rawValue <= points[0].Raw) return points[0].Swr;
        if (rawValue >= points[^1].Raw) return points[^1].Swr;

        for (int i = 0; i < points.Count - 1; i++)
        {
            var p1 = points[i];
            var p2 = points[i + 1];

            if (rawValue >= p1.Raw && rawValue <= p2.Raw)
            {
                double ratio = (double)(rawValue - p1.Raw) / (p2.Raw - p1.Raw);
                return p1.Swr + ratio * (p2.Swr - p1.Swr);
            }
        }

        return rawValue;
    }

    private async Task TriggerEmergencyCutoffAsync()
    {
        try
        {
            var pttOffPayload = new { transmitting = false };
            var cutoffUrl = _settings.YwcApiUrl.Replace("/status", "/tx");

            var client = _httpClientFactory.CreateClient("RadioClient");
            var response = await client.PostAsJsonAsync(cutoffUrl, pttOffPayload);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully sent PTT cutoff command to radio.");
            }
            else
            {
                _logger.LogError("Failed to send PTT cutoff command. Status: {Status}", response.StatusCode);
                await TriggerShellyShutdownAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception thrown while executing PTT cutoff. Escalating to Shelly.");
            await TriggerShellyShutdownAsync();
        }
    }

    private async Task TriggerShellyShutdownAsync()
    {
        try
        {
            string shellyUrl = $"http://{_settings.ShellyIp}/relay/{_settings.ShellyChannel - 1}?turn=off";
            var client = _httpClientFactory.CreateClient("RadioClient");
            var shellyResponse = await client.GetAsync(shellyUrl);
            if (shellyResponse.IsSuccessStatusCode)
            {
                _logger.LogCritical("EMERGENCY: Shelly hardware relay successfully tripped!");
            }
            else
            {
                _logger.LogCritical("CRITICAL ERROR: Failed to trigger Shelly relay hardware shutdown.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to Shelly relay at {Ip}", _settings.ShellyIp);
        }
    }
}

public class RadioStatus
{
    [JsonPropertyName("isConnected")]
    public bool IsConnected { get; set; }

    [JsonPropertyName("powerMeter")]
    public double PowerMeter { get; set; }

    [JsonPropertyName("swrMeter")]
    public double SwrMeter { get; set; }

    [JsonPropertyName("activeVfo")]
    public int ActiveVfo { get; set; } // 0 for VFO A, 1 for VFO B

    [JsonPropertyName("vfoA")]
    public VfoStatus? VfoA { get; set; }

    [JsonPropertyName("vfoB")]
    public VfoStatus? VfoB { get; set; }
}
public class VfoStatus
{
    [JsonPropertyName("frequency")]
    public long Frequency { get; set; }

    [JsonPropertyName("antenna")]
    public string Antenna { get; set; } = "1"; // Comes as a string from JSON
}

public class WatchdogSettings
{
    public string YwcApiUrl { get; set; } = string.Empty;
    public string ShellyIp { get; set; } = string.Empty;
    public string ShellyDevice { get; set; } = string.Empty;
    public int ShellyChannel { get; set; } = 1;
    public double MaxTxDurationMinutes { get; set; } = 0.17;
    public List<int> AllowedAntennas { get; set; } = new();
    public double MaxSwr { get; set; } = 2.5;

    // Default calibration curve matching your UI meter settings[cite: 1]
    public List<SwrCalibrationPoint> SwrCalibration { get; set; } = new()
    {
        new(0, 1.0),
        new(51, 1.5),
        new(77, 2.0),
        new(128, 3.0),
        new(173, 5.0),
        new(242, 9.9)
    };
}

public record SwrCalibrationPoint(int Raw, double Swr);