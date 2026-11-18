
using System.Text;
using System.Net.WebSockets;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using System.Reflection;


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

var logger = host.Services.GetRequiredService<ILogger<Program>>();

if (logger.IsEnabled(LogLevel.Information))
{
    var releaseId = Assembly.GetEntryAssembly()?
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion ?? "unknown";

    logger.LogInformation("Application starting up. ReleaseId: {ReleaseId}", releaseId);
}

host.Run();

public class RadioWatchdogWorker : BackgroundService
{
    private readonly ILogger<RadioWatchdogWorker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly WatchdogSettings _settings;
    private int _swrViolationCount = 0;
    private DateTime? _txStartTime = null;
    private DateTime _calibrationTimer = DateTime.MinValue;
    public bool IsRadioPowerOn { get; private set; }
    private bool? _lastLoggedPowerState;
    private DateTime _powerPollTimer = DateTime.MinValue;

    public RadioWatchdogWorker(
        ILogger<RadioWatchdogWorker> logger,
        IHttpClientFactory httpClientFactory,
        IOptions<WatchdogSettings> settings
        )

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
        _logger.LogInformation("[CONFIG] Permitted Antennas: [{Antennas}]", string.Join(", ", _settings.AllowedAntennas));
        _logger.LogInformation("[CONFIG] Max Allowed SWR Meter: {Swr}", _settings.MaxSwr);
        _logger.LogInformation("[CONFIG] Meter Calibration Timer: {CalibrateTimerSec} seconds", _settings.CalibrateTimerSec);
        _logger.LogInformation("[CONFIG] SWR Meter Calibration Values: {Swr}", string.Join(", ", _settings.SwrCalibration.Select(p => $"Raw {p.Raw} -> SWR {p.Swr}")));
        _logger.LogInformation("[CONFIG] PWR Meter Calibration Values: {Pwr}", string.Join(", ", _settings.PwrCalibration.Select(p => $"Raw {p.Raw} -> PWR {p.Pwr}")));
        _logger.LogInformation("[CONFIG] Initialization Complete");

        await UpdateCalibrationAsync(stoppingToken);

        _calibrationTimer = DateTime.UtcNow;
        bool isCurrentlyTransmitting = false;

        long pollCount = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            pollCount++;

            // 1. Calibration check (runs every x seconds)
            if ((DateTime.UtcNow - _calibrationTimer).TotalSeconds >= _settings.CalibrateTimerSec)
            {
                await UpdateCalibrationAsync(stoppingToken);
                _calibrationTimer = DateTime.UtcNow;
            }

            // 2. Radio power check (runs every 1 second)
            if ((DateTime.UtcNow - _powerPollTimer).TotalSeconds >= 1)
            {
                await UpdateRadioPowerAsync(stoppingToken);
                _powerPollTimer = DateTime.UtcNow;
            }

            try
            {
                var client = _httpClientFactory.CreateClient("RadioClient");
                var response = await client.GetAsync(string.Empty, stoppingToken);

                if (response.IsSuccessStatusCode)
                {
                    var radioStatus = await response.Content.ReadFromJsonAsync<RadioStatus>(cancellationToken: stoppingToken);
                    if (radioStatus != null)

                    {
                        if (_shutdownTriggered)
                        {
                            _logger.LogInformation("Radio API connection restored. Resetting watchdog state.");
                            _shutdownTriggered = false;
                        }

                        isCurrentlyTransmitting = await EvaluateRadioStateAsync(radioStatus, pollCount);

                    }
                }
                else
                {
                    _logger.LogWarning("Failed to poll radio status. HTTP Status: {StatusCode}", response.StatusCode);
                    isCurrentlyTransmitting = false;
                }
            }
            catch (Exception ex)
            {
                if (!_shutdownTriggered)
                {
                    _logger.LogError(ex, "Error occurred while polling radio status API. Triggering emergency Shelly shutdown due to lost communication!");
                    await TriggerShellyShutdownAsync();
                    _shutdownTriggered = true;
                }
                else
                {
                    _logger.LogDebug("Radio API still offline. Shutdown already triggered.");
                }
                isCurrentlyTransmitting = false;
            }

            // short/long delay: 200ms when transmitting to catch SSB voice/SWR peaks, 1 second when idle
            int delayMs = isCurrentlyTransmitting ? 200 : 1000;
            await Task.Delay(delayMs, stoppingToken);
        }
    }

    private async Task UpdateRadioPowerAsync(CancellationToken stoppingToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("RadioClient");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var ywcBaseUrl = new Uri(_settings.YwcApiUrl).GetLeftPart(UriPartial.Authority);

            var powerStatus = await client.GetFromJsonAsync<PowerStatusResponse>(
                $"{ywcBaseUrl}/api/cat/radiopower", options, stoppingToken);

            if (powerStatus != null)
            {
                IsRadioPowerOn = powerStatus.PowerOn;
                string radioState = IsRadioPowerOn ? "On" : "Off";

                if (_lastLoggedPowerState != IsRadioPowerOn)
                {
                    _logger.LogInformation("[CONFIG] Radio Power Status: {RadioPower}", radioState);
                    _lastLoggedPowerState = IsRadioPowerOn;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to poll radio power endpoint.");
            //_logger.LogWarning("Failed to poll radio power endpoint: {Message}", ex.Message);
        }
    }

    private async Task UpdateCalibrationAsync(CancellationToken stoppingToken)
    {
        try

        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var client = new HttpClient(handler);

            var uri = new Uri(_settings.YwcApiUrl);
            string ywcBaseUrl = $"{uri.Scheme}://{uri.Authority}";

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var response = await client.GetFromJsonAsync<CalibrationApiResponse>($"{ywcBaseUrl}/api/calibration/all", new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, stoppingToken);

            if (response != null)
            {

                if (response.PWR != null && response.PWR.Count > 0)
                {
                    var newPwrPoints = new List<WatchdogSettings.PowerCalibrationPoint>();
                    foreach (var entry in response.PWR)
                    {
                        if (double.TryParse(entry.Radio, out double powerVal))
                        {
                            newPwrPoints.Add(new WatchdogSettings.PowerCalibrationPoint((int)entry.Raw, powerVal));
                        }
                    }
                    if (newPwrPoints.Count > 0)
                    {
                        _settings.PwrCalibration = newPwrPoints;
                        _logger.LogInformation("Successfully applied {Count} PWR calibration points from API.", newPwrPoints.Count);
                    }
                }
            }


            if (response?.SWR != null && response.SWR.Count > 0)
            {
                var newSwrPoints = new List<WatchdogSettings.SwrCalibrationPoint>(); // Or whatever your SWR point type is named
                foreach (var entry in response.SWR)
                {
                    if (double.TryParse(entry.Radio, out double swrVal))
                    {
                        newSwrPoints.Add(new WatchdogSettings.SwrCalibrationPoint((int)entry.Raw, swrVal));
                    }
                }
                if (newSwrPoints.Count > 0)
                {
                    _settings.SwrCalibration = newSwrPoints;
                    _logger.LogInformation("Successfully applied {Count} SWR calibration points from API.", newSwrPoints.Count);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch live calibration points from YWC app.");
        }
    }
    private async Task<bool> EvaluateRadioStateAsync(RadioStatus status, long pollCount)
    {
        var client = _httpClientFactory.CreateClient("RadioClient");
        var txUrl = _settings.YwcApiUrl.Replace("/status", "/tx");

        var responseString = await client.GetStringAsync(txUrl);
        var txStatus = JsonSerializer.Deserialize<TxStatus>(responseString);
        var modeStatus = JsonSerializer.Deserialize<VfoStatus>(responseString);

        bool isTransmitting = txStatus?.Transmitting ?? false;

        if (isTransmitting)
        {
            // 1. Check Ant Status
            string activeAntennaStr = status.ActiveVfo == 1 ? status.VfoB?.Antenna ?? "1" : status.VfoA?.Antenna ?? "1";
            string currentMode = status.ActiveVfo == 1 ? status.VfoB?.Mode ?? "LSB" : status.VfoA?.Mode ?? "LSB";

            if (int.TryParse(activeAntennaStr, out int currentAntenna))
            {
                if (!_settings.AllowedAntennas.Contains(currentAntenna))
                {
                    _logger.LogCritical("Invalid Antenna detected ({Antenna}) during transmission. Executing emergency cutoff!", currentAntenna);
                    await TriggerEmergencyCutoffAsync();
                    return false;
                }
            }
            double calibratedPwr = GetCalibratedPwr(status.PowerMeter);
            double calibratedSwr = GetCalibratedSwr(status.SwrMeter);
            if (_txStartTime == null)

            {
                _txStartTime = DateTime.UtcNow;
                _logger.LogInformation("Transmit started. Mode: {ModeStatus}, Power: {CalibratedPwr:F2}, SWR: {CalibratedSwr:F1}", currentMode, calibratedPwr, calibratedSwr);
            }

            var txDuration = (DateTime.UtcNow - _txStartTime.Value).TotalMinutes;
            if (txDuration > _settings.MaxTxDurationMinutes)
            {
                _logger.LogCritical("Max TX duration exceeded ({Duration:F2} mins). Triggering emergency PTT cutoff!", txDuration);
                await TriggerEmergencyCutoffAsync();
            }

            if (calibratedSwr > _settings.MaxSwr)
            {
                _swrViolationCount++;
                _logger.LogWarning("High SWR detected: {CalibratedSwr:F2}, Violation count: {Count})", calibratedSwr, _swrViolationCount);

                if (_swrViolationCount >= 2)
                {
                    _logger.LogCritical("SWR threshold breached sustained limit ({CalibratedSwr:F2}). Executing emergency cutoff!", calibratedSwr);
                    await TriggerEmergencyCutoffAsync();
                }
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

        return isTransmitting;
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
    private double GetCalibratedPwr(double rawValue)
    {
        var points = _settings.PwrCalibration;

        if (points == null || points.Count == 0)
            return rawValue;

        if (rawValue <= points[0].Raw) return points[0].Pwr;
        if (rawValue >= points[^1].Raw) return points[^1].Pwr;

        for (int i = 0; i < points.Count - 1; i++)
        {
            var p1 = points[i];
            var p2 = points[i + 1];

            if (rawValue >= p1.Raw && rawValue <= p2.Raw)
            {
                double ratio = (double)(rawValue - p1.Raw) / (p2.Raw - p1.Raw);
                return p1.Pwr + ratio * (p2.Pwr - p1.Pwr);
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
            { "id", 1 },
            { "src", "ywc-watchdog" },
            { "method", "Switch.Set" },
            { "params", new Dictionary<string, object>
                {
                    { "id", _settings.ShellyChannel },
                    { "on", false }
                }
            }
        };

            string jsonPayload = JsonSerializer.Serialize(rpcCommand);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonPayload);

            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cts.Token);
            _logger.LogCritical("EMERGENCY: Sent WebSocket shutdown command to Shelly channel {Channel}.", _settings.ShellyChannel);

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
public class PowerStatusResponse
{
    public bool PowerOn { get; set; }
}
public class TxStatus
{
    [JsonPropertyName("transmitting")]
    public bool Transmitting { get; set; }
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

    [JsonPropertyName("mode")]
    public string Mode { get; set; } = "LSB";
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
    public int CalibrateTimerSec { get; set; } = 300;

    public List<SwrCalibrationPoint> SwrCalibration { get; set; } = new()
    {
        new(0, 1.0),
        new(51, 1.5),
        new(77, 2.0),
        new(128, 3.0),
        new(173, 5.0),
        new(242, 9.9)
    };
    //}

    public record SwrCalibrationPoint(int Raw, double Swr);

    public List<PowerCalibrationPoint> PwrCalibration { get; set; } = new()
    {
       // new(0,0),
        new(30, 5),
        new(76, 25),
        new(112, 50),
        new(157, 100),
        new(190, 150),
        new(222, 200)
    };

    public record PowerCalibrationPoint(int Raw, double Pwr);
}
public class CalibrationApiResponse
{
    public List<CalibrationEntry> PWR { get; set; } = new();
    public List<CalibrationEntry> SWR { get; set; } = new();

    //     [JsonPropertyName("Compression")]
    //     public List<CalibrationEntry> Compression { get; set; } = new();

    //     [JsonPropertyName("ALC")]
    //     public List<CalibrationEntry> ALC { get; set; } = new();

    //     [JsonPropertyName("TPA")]
    //     public List<CalibrationEntry> TPA { get; set; } = new();

    //     [JsonPropertyName("IDD")]
    //     public List<CalibrationEntry> IDD { get; set; } = new();

    //     [JsonPropertyName("VPA")]
    //     public List<CalibrationEntry> VPA { get; set; } = new();

    //     [JsonPropertyName("S-Meter")]
    //     public List<CalibrationEntry> SMeter { get; set; } = new();
}
public class CalibrationEntry
{
    [JsonPropertyName("Radio")]
    public string Radio { get; set; } = string.Empty;

    [JsonPropertyName("raw")]
    public double Raw { get; set; }
}