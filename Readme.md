# ywc-watchdog

`ywc-watchdog` is a lightweight .NET watchdog service designed for `linux-arm64` and `win-x64` to monitor [Yaesu Web Control](https://github.com/mm5agm/Yaesu_Web_Control). The service monitors:

- Radio transmit time
- Radio SWR
- Radio antennas

If a threshold is exceeded, `ywc-watchdog` will inhibit transmit. If the inhibit command fails, `ywc-watchdog` will fall back to disconnecting mains power via a Shelly relay.

## Features

- **Transceiver Protection:** Continuously monitors TX duration, SWR limits, and antenna selection.
- **Fail-Safe Relay Fallback:** Automatically cuts mains power via a Shelly smart relay if CAT transmit inhibit fails.
- **Cross-Platform:** Runs natively as a background service on Raspberry Pi (`linux-arm64`) and Windows (`win-x64`).
- **Simple Configuration:** Easily adjustable thresholds and endpoints via `appsettings.json`.

---

## Installation & Deployment (Raspberry Pi / Linux ARM64)

You do not need to build this project from source to run it on your Raspberry Pi. Pre-compiled binaries are provided under GitHub Releases.

### Option A: Using Pre-Compiled Binaries (Recommended)

1. Create your application directory and download the latest release package directly from GitHub:

```bash
sudo mkdir -p /opt/ywc-watchdog
cd /opt/ywc-watchdog
sudo curl -L -o ywc-watchdog-release.zip "https://github.com/w8be/ywc-watchdog/releases/latest/download/YwcWatchdog-linux-arm64.zip"
```

2. Install `unzip` (if not already installed) and extract the archive:

```bash
sudo apt-get update && sudo apt-get install -y unzip
sudo unzip -o ywc-watchdog-release.zip
sudo rm ywc-watchdog-release.zip
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
git clone https://github.com/w8be/ywc-watchdog.git
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

1. Create an application directory (e.g., `C:\ywc-watchdog`) and download the latest release package `YwcWatchdog-win-x64.zip` directly from GitHub Releases.
2. Extract the archive into that directory.
3. Edit `appsettings.json` to configure your Yaesu Web Control IP, Shelly IP, and threshold settings.
4. Run `YwcWatchdog.exe` to start the application.

---

## Configuration

Before starting the service, update `appsettings.json` with your Yaesu Web Control URL, Shelly relay IP address, and safety thresholds (max TX duration, SWR limit, and allowed antennas).

For a complete breakdown of all settings and environment overrides, see **[CONFIGURATION.md](./CONFIGURATION.md)**.

## Example Output

For `ywc-watchdog` examples, see **[EXAMPLES.md](./EXAMPLES.md)**.

## License

Distributed under the MIT License. See `LICENSE` for more information.
