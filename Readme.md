# ywc-watchdog

`ywc-watchdog` is a lightweight .NET watchdog service designed for Raspberry Pi to monitor Yaesu Web Control https://github.com/mm5agm/Yaesu_Web_Control. The service monitors:

- Radio transmit time
- Radio SWR
- Radio Antennas

If a threshold is exceeded ywc-watchdog will inhibit transmit. If the command fails ywc-watchdog will fallback to disconnecting the mains power via a Shelly relay.

## Features

- Background process monitoring for Raspberry Pi (Linux ARM64).
- Automatic service restart upon failure detection.
- Configurable settings via `appsettings.json`.

---

## Installation & Deployment (Raspberry Pi)

You do not need to build this project from source to run it on your Raspberry Pi. Pre-compiled binaries are provided under GitHub Releases.

### Option A: Using Pre-Compiled Binaries (Recommended)

1. Create your application directory and download the latest release package directly from GitHub:

```bash
   sudo mkdir -p /opt/ywc-watchdog
   cd /opt/ywc-watchdog
   sudo curl -L -o ywc-watchdog-release.zip "https://github.com/w8be/ywc-watchdog/releases/latest/download/YwcWatchdog-linux-arm64.zip"
```

2. Instll unzip (if not already installed) and extract the archive:

```bash
   sudo apt-get update && sudo apt-get install -y unzip
   sudo unzip -o ywc-watchdog-release.zip
   sudo rm ywc-watchdog-release.zip
```

3. Set execution permissions for your compiled binary:

```bash
   sudo chmod +x YwcWatchdog
```

4. Edit appsettings.json to configure Yaesu Web Control Ip, Shelly IP and threshold settings:

```bash
   sudo nano appsettings.json
```

### Option B: Building from Source

```bash
   # Clone the repository
   git clone [https://github.com/w8be/ywc-watchdog.git](https://github.com/w8be/ywc-watchdog.git)
   cd ywc-watchdog

   # Publish self-contained executable for Raspberry Pi (64-bit ARM)
   dotnet publish -c Release -r linux-arm64 --self-contained true -o ./publish
```

### Running as a System Service (systemd)

1. Create and open the systemd service file:

```
   sudo nano /etc/systemd/system/ywc-watchdog.service
```

2. Paste the following configuration into the file:

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

## Installation & Deployment (win-x64)

1. Create your application directory and download the latest release package (ywc-watchdog-win-x64.zip)directly from GitHub

2. Unzip and extract the archive:

3. Edit appsettings.json to configure Yaesu Web Control Ip, Shelly IP and threshold settings

## Configuration

Before starting the service, update `appsettings.json` with your Yaesu Web Control URL, Shelly relay IP address, and safety thresholds (max TX duration, SWR limit, and allowed antennas).

For a complete breakdown of all settings and environment overrides, see **[CONFIGURATION.md](./CONFIGURATION.md)**.

## Example Output

For ywc-watchdog examples, see [EXAMPLES.md](./EXAMPLES.md)

### License

Distributed under the MIT License. See LICENSE for more information.

---

   <ElicitationsGroup message="If you are ready to update this in VS Code and push it:">
   <Elicitation label="Review git commit commands" query="What git commands do I run to save and push my updated README.md to GitHub?"/>
   </ElicitationsGroup>
