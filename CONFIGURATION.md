# Configuration Guide

This document details all available configuration options for the Watchdog Service. For download and installation instructions, see [README.md](./README.md).

## Quick Start

1. Open `appsettings.json` in the application directory.
2. Update the values under `WatchdogSettings` to match your station's network addresses and safety thresholds.
3. Restart the service to apply your changes.

---

## Full Example (`appsettings.json`)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Warning",
      "System.Net.Http": "Warning",
      "RadioWatchdogWorker": "Information"
    }
  },
  "WatchdogSettings": {
    "YwcApiUrl": "https://127.0.0.1:8443",
    "ShellyIp": "127.0.0.1",
    "ShellyDevice": "Shelly Pro4PM",
    "ShellyChannel": 0,
    "MaxTxDurationMinutes": 2,
    "MaxSwr": 2.5,
    "MaxTmp": 60,
    "AllowedAntennas": [1],
    "CalibrateTimerSec": 300
  }
}
```

---

## Settings Reference

### `WatchdogSettings`

Controls YWC api endpoint polling, Radio CAT polling, transmit inhibit on threshold, and Shelly /rpc relay control for failsafe power cutoff.

| Key                    | Type     | Required | Default                    | Description                                                                                                                                                                                                                                                                                                               |
| :--------------------- | :------- | :------- | :------------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `YwcApiUrl`            | `string` | **Yes**  | `"https://127.0.0.1:8443"` | URL to the Yaesu Web Control (YWC) API endpoint.                                                                                                                                                                                                                                                                          |
| `ShellyIp`             | `string` | **Yes**  | `"127.0.0.1"`              | Local IPv4 address of the Shelly relay controlling station power.                                                                                                                                                                                                                                                         |
| `ShellyDevice`         | `string` | **Yes**  | `"Shelly Pro4PM"`          | Model name of your Shelly relay device. Currently tested with **Shelly Plug (Gen4)** and **Shelly Pro 4PM**.                                                                                                                                                                                                              |
| `ShellyChannel`        | `int`    | **Yes**  | `1`                        | 0-based relay channel ID to trip when a fault occurs. Use `0` for single-channel devices (such as the Gen4 Plug) or `0`–`3` for the 4-channel Shelly Pro 4PM.                                                                                                                                                             |
| `MaxTxDurationMinutes` | `double` | **Yes**  | `2`                        | Maximum continuous transmit time (in decimal minutes) before the watchdog sends a CAT PTT Off cmd. After 3 attempts a Power off cmd will be sent to the Shelly relay. Accepts decimals for partial minutes. Seconds / 60 = Decimal Minute (e.g., 0.17 for ~10 seconds, 0.5 for 30 seconds, 2.5 for 2 minutes 30 seconds). |
| `MaxSwr`               | `double` | **Yes**  | `2.5`                      | Maximum allowed SWR threshold. Exceeding this value during transmit triggers a safety cutoff.                                                                                                                                                                                                                             |
| `MaxTmp`               | `int`    | **Yes**  | `60`                       | Maximum allowed Temperature threshold. Exceeding this value during transmission triggers a saftey cutoff.                                                                                                                                                                                                                 |
| `AllowedAntennas`      | `int[]`  | **Yes**  | `[1]`                      | List of permitted antenna port numbers (e.g., `[1, 2]`).                                                                                                                                                                                                                                                                  |
| `CalibrateTimerSec`    | `int`    | **Yes**  | `300`                      | Value in seconds of the refresh poll timer for meter calibration data.                                                                                                                                                                                                                                                    |

`Note: ywc-watchdog thresholds are configured in standard units (e.g., Watts, SWR ratio, °C/°F). Yaesu Web Control converts the radio's raw meter values (0–255) into standard units for the UI display. To ensure accurate threshold triggers, you must first calibrate the Power, SWR, and Temperature meters in Yaesu Web Control so that your standard unit thresholds map correctly to the radio's raw 0–255 output.`

### `Logging`

Controls standard .NET console and event logging verbosity. Valid values are `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`, and `None`.

- **`Default`:** Baseline log level for the application (`Information` is recommended for normal operation; use `Debug` for troubleshooting).
- **`Microsoft.Hosting.Lifetime`:** Controls service startup and shutdown messages.
- **`System.Net.Http`:** Set to `Warning` by default to prevent frequent HTTP polling requests from flooding your logs.

---

## Environment & Command-Line Overrides

Because this service uses the standard .NET configuration provider, you can override any setting without editing `appsettings.json` directly:

1. **Environment-Specific File:** Create an `appsettings.Production.json` file in the same folder. Any settings placed inside will override `appsettings.json` when `DOTNET_ENVIRONMENT=Production`.
2. **Environment Variables:** Use a double underscore (`__`) to separate nested sections:
   ```bash
   WatchdogSettings__ShellyIp=192.168.1.50
   WatchdogSettings__MaxSwr=2.0
   ```
3. **Command-Line Arguments:** Pass overrides when starting the executable:
   ```bash
   ./WatchdogService.exe --WatchdogSettings:MaxTxDurationMinutes=3
   ```

_(Precedence order: Command-line arguments > Environment variables > `appsettings.{Environment}.json` > `appsettings.json`)_
