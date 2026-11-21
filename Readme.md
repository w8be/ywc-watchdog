# ywc-watchdog

`ywc-watchdog` is a lightweight .NET watchdog service designed for `linux-arm64` and `win-x64` to monitor [Yaesu Web Control](https://github.com/mm5agm/Yaesu_Web_Control). The service monitors:

- Radio Antennas
- Radio Power
- Radio SWR
- Radio Temperature

If a threshold is exceeded, `ywc-watchdog` will inhibit transmit. If the inhibit command fails, `ywc-watchdog` will fall back to disconnecting mains power via a Shelly relay.

> [!WARNING] > **Use at your own risk.** `ywc-watchdog` is a secondary safeguard, not a guaranteed protection system. Network delays, hardware faults, or misconfiguration can prevent timely transmit inhibit or relay cutoff. You remain solely responsible for equipment safety and amateur radio control-operator compliance. See [LICENSE](LICENSE) for full terms.

## Features

- **Transceiver Protection:** Continuously monitors TX duration, SWR / Temperature limits, and Antenna selection.
- **Automated App Calibration**: SWR / Power / Temperature calibration settings polled on timer.
- **Fail-Safe Relay Fallback:** Automatically cuts mains power via a Shelly smart relay if CAT transmit inhibit fails.
- **Cross-Platform:** Runs natively as a background service on Raspberry Pi (`linux-arm64`) and Windows (`win-x64`).
- **Simple Configuration:** Adjustable thresholds and API endpoints via `appsettings.json`.
- **Status Logging:** Startup / API polling / Radio Power / Transmit RF Power - SWR - Mode - Temperature

---

## Installation & Deployment (Raspberry Pi / Linux ARM64)

You do not need to build this project from source to run it on your Raspberry Pi. Pre-compiled binaries are provided under GitHub Releases.

### Option A: Using Pre-Compiled Binaries (Recommended)

1. Create your application directory and download the latest release package directly from GitHub:

```bash
sudo mkdir -p /opt/ywc-watchdog
cd /opt/ywc-watchdog
sudo curl -fL -o ywc-watchdog-linux-arm64.zip "https://github.com/w8be/ywc-watchdog/releases/download/v1.3.0/YwcWatchdog-linux-arm64.zip"
```

2. Install `unzip` (if not already installed) and extract the archive:

```bash
sudo apt-get update && sudo apt-get install -y unzip
sudo unzip -o ywc-watchdog-linux-arm64.zip
sudo rm ywc-watchdog-linux-arm64.zip
```

3. Set execution permissions for the compiled binary:

```bash
sudo chmod +x YwcWatchdog
```

4. Edit `appsettings.json` to configure your Yaesu Web Control IP, Shelly IP, and threshold settings:

```bash
sudo nano appsettings.json
```

### Option B: Building from Source

```bash
# Clone the repository
git clone https://github.com/w8be/ywc-watchdog
cd ywc-watchdog

# Publish self-contained executable for Raspberry Pi (64-bit ARM)
dotnet publish -c Release -r linux-arm64 --self-contained true -o ./publish
```

### Running as a System Service (systemd)

1. Create and open the systemd service file:

```bash
sudo nano /etc/systemd/system/ywc-watchdog.service
```

2. Paste the following configuration into the file:

```ini
[Unit]
Description=YWC Watchdog .NET Service for Raspberry Pi
After=network.target

[Service]
Type=simple
User=root
WorkingDirectory=/opt/ywc-watchdog
ExecStart=/opt/ywc-watchdog/YwcWatchdog
Restart=always
RestartSec=10
SyslogIdentifier=ywc-watchdog

[Install]
WantedBy=multi-user.target
```

3. Enable and start the service:

```bash
sudo systemctl daemon-reload
sudo systemctl enable ywc-watchdog.service
sudo systemctl start ywc-watchdog.service
```

4. Check service status or view live logs:

```bash
sudo systemctl status ywc-watchdog.service
sudo journalctl -u ywc-watchdog -f
```

---

## Installation & Deployment (Windows x64)

1. Create an application directory (e.g., `C:\ywc-watchdog`) and download the latest release package `ywc-watchdog-win-x64.zip` directly from GitHub Releases.
2. Extract the archive into that directory.
3. Edit `appsettings.json` to configure your Yaesu Web Control IP, Shelly IP, and threshold settings.
4. Run `YwcWatchdog.exe` directly to test it in a console window.

### Running as a Windows Service

#### Using NSSM (Recommended)

[NSSM (the Non-Sucking Service Manager)](https://nssm.cc) wraps standard console executables cleanly, ensures the working directory is set properly so `appsettings.json` is found, and automatically restarts the app if it crashes.

1. Download NSSM and open **Command Prompt** or **PowerShell as Administrator**.
2. Install and start the service:

```powershell
nssm install ywc-watchdog "C:\ywc-watchdog\YwcWatchdog.exe"
nssm set ywc-watchdog AppDirectory "C:\ywc-watchdog"
nssm set ywc-watchdog Description "YWC Watchdog .NET Service"
nssm set ywc-watchdog Start SERVICE_AUTO_START
nssm start ywc-watchdog
```

---

## Configuration

Before starting the service, update `appsettings.json` with your Yaesu Web Control URL, Shelly relay IP address, and safety thresholds (max TX duration, SWR limit, and allowed antennas).

For a complete breakdown of all settings and environment overrides, see **[CONFIGURATION.md](./CONFIGURATION.md)**.

## Example Output

For `ywc-watchdog` examples, see **[LOG_REFERENCE.md](./LOG_REFERENCE.md)**.

## ⚠️ Disclaimer

**Use at your own risk.** This software interfaces with amateur radio transceivers and mains power relays (Shelly) to monitor and interrupt transmissions.

- **No Guarantee of Protection:** Network latency, API failures, misconfigured thresholds, or hardware issues may prevent `ywc-watchdog` from inhibiting transmit or cutting power in time to prevent damage to your transceiver, amplifier, or antenna system.
- **Mains Power Hazard:** Cutting mains power abruptly via a smart relay can cause equipment damage or data loss if not properly configured for your station setup. Always verify your relay wiring, ratings, and fail-safe behaviors.
- **Control Operator Responsibility:** You remain solely responsible for compliance with all applicable amateur radio regulations (including FCC Part 97 or local equivalents) and safe station operation. Automated monitoring is a secondary safeguard, not a replacement for an attentive control operator.

## License

Distributed under the MIT License. See [LICENSE](LICENSE) for more information.
