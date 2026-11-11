using System;
using System.Text;
using System.Net.WebSockets;
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

var builder = Host.CreateApplicationBuilder(args);

var watchdogSettings = builder.Configuration.GetSection(nameof(WatchdogSettings)).Get<WatchdogSettings>()
    ?? throw new InvalidOperationException("WatchdogSettings configuration section is missing or empty.");

builder.Services.AddHttpClient("RadioClient", client =>
{
    client.BaseAddress = new Uri(watchdogSettings.YwcApiUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
})
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

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

    public RadioWatchdogWorker(
        ILogger<RadioWatchdogWorker> logger,
        IHttpClientFactory httpClientFactory,
        IOptions<WatchdogSettings> settings)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
    }
    private bool _shutdownTriggered = false;
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
                var response = await client.GetAsync(string.Empty, stoppingToken);

                if (response.IsSuccessStatusCode)
                {
                    var radioStatus = await response.Content.ReadFromJsonAsync<RadioStatus>(cancellationToken: stoppingToken);
                    if (radioStatus != null)
                    {
                        // 2. If connection is back and we were previously shutdown, reset the flag
                        if (_shutdownTriggered)
                        {
                            _logger.LogInformation("Radio API connection restored. Resetting watchdog state.");
                            _shutdownTriggered = false;
                        }

                        await EvaluateRadioStateAsync(radioStatus);
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to poll radio status. HTTP Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                // 3. Only trigger and log on the FIRST failure of the outage
                if (!_shutdownTriggered)
                {
                    _logger.LogError(ex, "Error occurred while polling radio status API. Triggering emergency Shelly shutdown due to lost communication!");

                    await TriggerShellyShutdownAsync();

                    _shutdownTriggered = true; // Locks it out from spamming every second!
                }
                else
                {
                    // Optional: Debug log so it doesn't completely blind you, but keeps error logs silent
                    _logger.LogDebug("Radio API still offline. Shutdown already triggered.");
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
    private async Task EvaluateRadioStateAsync(RadioStatus status)
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
                    await TriggerEmergencyCutoffAsync();
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
                await TriggerEmergencyCutoffAsync();
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
                    await TriggerEmergencyCutoffAsync();
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
                return p1.Swr + ratio * (p2.Swr - p2.Swr);
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

    private async Task<bool?> CheckShellyRelayStatusAsync()
    {
        try
        {
            string wsUri = $"ws://{_settings.ShellyIp}/rpc";
            using var ws = new ClientWebSocket();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            await ws.ConnectAsync(new Uri(wsUri), cts.Token);

            var rpcCommand = new Dictionary<string, object>
        {
            { "id", 10 },
            { "src", "ywc-watchdog" },
            { "method", "Switch.GetStatus" },
            { "params", new Dictionary<string, object>
                {
                    { "id", _settings.ShellyChannel }
                }
            }
        };

            string jsonPayload = JsonSerializer.Serialize(rpcCommand);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonPayload);

            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cts.Token);

            // Receive response buffer
            var buffer = new byte[1024];
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string responseString = Encoding.UTF8.GetString(buffer, 0, result.Count);
                using var doc = JsonDocument.Parse(responseString);

                // Look for the output property in the result object
                if (doc.RootElement.TryGetProperty("result", out var resElement) &&
                    resElement.TryGetProperty("output", out var outputElement))
                {
                    return outputElement.GetBoolean(); // true = ON, false = OFF
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to poll Shelly status via WebSocket.");
        }

        return null; // Status unknown
    }

    private async Task TriggerShellyShutdownAsync()
    {
        try
        {
            string wsUri = $"ws://{_settings.ShellyIp}/rpc";

            _logger.LogCritical("Connecting to Shelly WebSocket at {Uri}...", wsUri);

            using var ws = new ClientWebSocket();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            await ws.ConnectAsync(new Uri(wsUri), cts.Token);

            var rpcCommand = new Dictionary<string, object>
        {
            { "id", 1 }, // Request tracking ID (standard protocol format)
            { "src", "ywc-watchdog" },
            { "method", "Switch.Set" },
            { "params", new Dictionary<string, object>
                {
                    { "id", _settings.ShellyChannel }, // Dynamic channel from your settings!
                    { "on", false }
                }
            }
        };

            string jsonPayload = JsonSerializer.Serialize(rpcCommand);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonPayload);

            // Send shutdown command once cleanly
            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cts.Token);
            _logger.LogCritical("EMERGENCY: Sent WebSocket shutdown command to Shelly channel {Channel}.", _settings.ShellyChannel);

            // Wait a moment, then poll status to verify it actually turned off
            await Task.Delay(500, cts.Token);
            bool? relayState = await CheckShellyRelayStatusAsync();

            if (relayState == false)
            {
                _logger.LogCritical("VERIFIED: Shelly channel {Channel} is confirmed OFF.", _settings.ShellyChannel);
            }
            else
            {
                _logger.LogError("WARNING: Shelly channel {Channel} state could not be verified or is still ON!", _settings.ShellyChannel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to communicate with Shelly via WebSocket at {Ip}", _settings.ShellyIp);
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
    public int ActiveVfo { get; set; }

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
    public string Antenna { get; set; } = "1";
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