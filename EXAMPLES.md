# YwcWatchdog Example

## Example Output

Here is a sample of live system logs from `ywc-watchdog` running on a Linux host (via `journalctl -u ywc-watchdog`), showing startup configuration, normal transmissions, and emergency cutoff actions triggered by an invalid antenna and high SWR:

```text
Nov 11 09:57:49 127.0.0.1 ywc-watchdog[8091]: info: Microsoft.Hosting.Lifetime[0]
Nov 11 09:57:49 127.0.0.1 ywc-watchdog[8091]:        Content root path: /opt/ywc-watchdog
Nov 11 09:57:49 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 09:57:49 127.0.0.1 ywc-watchdog[8091]:        [CONFIG] Shelly IP: 192.168.1.136 (Channel: 1)
Nov 11 09:57:49 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 09:57:49 127.0.0.1 ywc-watchdog[8091]:        [CONFIG] Max TX Duration: 2 minutes
Nov 11 09:57:49 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 09:57:49 127.0.0.1 ywc-watchdog[8091]:        [CONFIG] Max Allowed SWR Meter: 2.5
Nov 11 09:57:49 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 09:57:49 127.0.0.1 ywc-watchdog[8091]:        [CONFIG] Allowed Antennas: [1]
Nov 11 12:33:08 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 12:33:08 127.0.0.1 ywc-watchdog[8091]:        Transmit started. Power: 35, Raw SWR: 0
Nov 11 12:33:11 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 12:33:11 127.0.0.1 ywc-watchdog[8091]:        Transmit ended. Resetting timer.
Nov 11 12:34:10 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 12:34:10 127.0.0.1 ywc-watchdog[8091]:        Transmit started. Power: 35, Raw SWR: 0
Nov 11 12:34:13 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 12:34:13 127.0.0.1 ywc-watchdog[8091]:        Transmit ended. Resetting timer.
Nov 11 12:34:25 127.0.0.1 ywc-watchdog[8091]: crit: RadioWatchdogWorker[0]
Nov 11 12:34:25 127.0.0.1 ywc-watchdog[8091]:        Invalid Antenna detected (2) during transmission. Executing emergency cutoff!
Nov 11 12:34:25 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 12:34:25 127.0.0.1 ywc-watchdog[8091]:        Successfully sent PTT cutoff command to radio.
Nov 11 12:34:53 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 12:34:53 127.0.0.1 ywc-watchdog[8091]:        Transmit started. Power: 35, Raw SWR: 0
Nov 11 12:34:54 127.0.0.1 ywc-watchdog[8091]: warn: RadioWatchdogWorker[0]
Nov 11 12:34:54 127.0.0.1 ywc-watchdog[8091]:        High SWR detected: 9.90 (Raw: 255, Violation count: 1)
Nov 11 12:34:55 127.0.0.1 ywc-watchdog[8091]: warn: RadioWatchdogWorker[0]
Nov 11 12:34:55 127.0.0.1 ywc-watchdog[8091]:        High SWR detected: 9.90 (Raw: 255, Violation count: 2)
Nov 11 12:34:55 127.0.0.1 ywc-watchdog[8091]: crit: RadioWatchdogWorker[0]
Nov 11 12:34:55 127.0.0.1 ywc-watchdog[8091]:        SWR threshold breached sustained limit (9.90). Executing emergency cutoff!
Nov 11 12:34:55 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 12:34:55 127.0.0.1 ywc-watchdog[8091]:        Successfully sent PTT cutoff command to radio.
Nov 11 12:34:56 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 12:34:56 127.0.0.1 ywc-watchdog[8091]:        Transmit ended. Resetting timer.
Nov 11 12:35:49 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 12:35:49 127.0.0.1 ywc-watchdog[8091]:        Transmit started. Power: 35, Raw SWR: 0
Nov 11 12:35:51 127.0.0.1 ywc-watchdog[8091]: info: RadioWatchdogWorker[0]
Nov 11 12:35:51 127.0.0.1 ywc-watchdog[8091]:        Transmit ended. Resetting timer.
```

Here is a sample of live system logs from `ywc-watchdog` running on a Linux host (via `journalctl -u ywc-watchdog`), showing startup configuration, normal transmissions, and emergency cutoff actions triggered by a Maximum TX duration of 5 seconds:

```text
Nov 11 12:55:15 127.0.0.1 ywc-watchdog[8567]: info: Microsoft.Hosting.Lifetime[0]
Nov 11 12:55:15 127.0.0.1 ywc-watchdog[8567]:       Content root path: /opt/ywc-watchdog
Nov 11 12:55:15 127.0.0.1 ywc-watchdog[8567]: info: RadioWatchdogWorker[0]
Nov 11 12:55:15 127.0.0.1 ywc-watchdog[8567]:       [CONFIG] Shelly IP: 192.168.1.136 (Channel: 1)
Nov 11 12:55:15 127.0.0.1 ywc-watchdog[8567]: info: RadioWatchdogWorker[0]
Nov 11 12:55:15 127.0.0.1 ywc-watchdog[8567]:       [CONFIG] Max TX Duration: 0.08 minutes
Nov 11 12:55:15 127.0.0.1 ywc-watchdog[8567]: info: RadioWatchdogWorker[0]
Nov 11 12:55:15 127.0.0.1 ywc-watchdog[8567]:       [CONFIG] Max Allowed SWR Meter: 2.5
Nov 11 12:55:15 127.0.0.1 ywc-watchdog[8567]: info: RadioWatchdogWorker[0]
Nov 11 12:55:15 127.0.0.1 ywc-watchdog[8567]:       [CONFIG] Allowed Antennas: [1]
Nov 11 12:56:00 127.0.0.1 ywc-watchdog[8567]: info: RadioWatchdogWorker[0]
Nov 11 12:56:00 127.0.0.1 ywc-watchdog[8567]:       Transmit started. Power: 35, Raw SWR: 0
Nov 11 12:56:05 127.0.0.1 ywc-watchdog[8567]: crit: RadioWatchdogWorker[0]
Nov 11 12:56:05 127.0.0.1 ywc-watchdog[8567]:       Max TX duration exceeded (0.08 mins). Triggering emergency PTT cutoff!
Nov 11 12:56:05 127.0.0.1 ywc-watchdog[8567]: info: RadioWatchdogWorker[0]
Nov 11 12:56:05 127.0.0.1 ywc-watchdog[8567]:       Successfully sent PTT cutoff command to radio.
Nov 11 12:56:06 127.0.0.1 ywc-watchdog[8567]: info: RadioWatchdogWorker[0]
Nov 11 12:56:06 127.0.0.1 ywc-watchdog[8567]:       Transmit ended. Resetting timer.
```
