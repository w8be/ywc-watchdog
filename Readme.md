# ywc-watchdog

`ywc-watchdog` is a lightweight .NET watchdog service designed for Raspberry Pi to monitor system services and ensure high uptime.

## Features

- Background process monitoring for Raspberry Pi (Linux).
- Automatic service restart upon failure detection.
- Configurable settings via `appsettings.json`.

---

## Installation & Deployment (Raspberry Pi)

You do **not** need to build this project from source to run it on your Raspberry Pi. Pre-compiled binaries are provided under GitHub Releases.

### Option A: Using Pre-Compiled Binaries (Recommended)

1. Go to the [Releases](../../releases) tab on this GitHub repository and download the latest `.zip` package.
2. Transfer the package to your Raspberry Pi and extract it:
   ```bash
   mkdir -p ~/ywc-watchdog
   unzip ywc-watchdog-linux-arm64.zip -d ~/ywc-watchdog
   cd ~/ywc-watchdog
   ```
3. Edit appsettings.json to configure your target processes and threshold settings

```bash
   nano appsettings.json
```

3. Set execution permissions for the binary:

```bash
   chmod +x ywc-watchdog
```

### Option B: Building from Source

```bash
   # Clone the repository
   git clone [https://github.com/YOUR_USERNAME/ywc-watchdog.git](https://github.com/YOUR_USERNAME/ywc-watchdog.git)
  cd ywc-watchdog

  # Publish self-contained executable for Raspberry Pi (64-bit ARM)
  dotnet publish -c Release -r linux-arm64 --self-contained true -o ./publish
```

### Running as a System Service (systemd)

```
   bash
   sudo nano /etc/systemd/system/ywc-watchdog.service
```

[Unit]
Description=YWC Watchdog Service
After=network.target

[Service]
Type=simple
WorkingDirectory=/home/pi/ywc-watchdog
ExecStart=/home/pi/ywc-watchdog/ywc-watchdog
Restart=always
RestartSec=10
User=pi

[Install]
WantedBy=multi-user.target

3. Enable and start the service:

```
   bash
   sudo systemctl daemon-reload
   sudo systemctl enable ywc-watchdog
   sudo systemctl start ywc-watchdog
```

License
Distributed under the MIT License. See LICENSE for more information.

---

<ElicitationsGroup message="If you run into issues setting up your repository or releasing binaries:">
  <Elicitation label="Publish command for Pi" query="What exact dotnet publish command should I run to build self-contained binaries for Raspberry Pi?"/>
  <Elicitation label="Setting up systemd" query="How do I configure ywc-watchdog as a systemd background daemon on Raspberry Pi?"/>
  <Elicitation label="How to make Release" query="How do I attach compiled zip files to a GitHub Release step-by-step?"/>
</ElicitationsGroup>
