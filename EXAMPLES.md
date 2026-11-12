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

Here is a sample of live system logs from `ywc-watchdog` running on a Linux host (via `journalctl -u ywc-watchdog`), showing startup configuration, and emergency Shelly relay cutoff actions triggered by a loss of communication to the Yaesu Web Control Api endpoints.:

```text
Nov 11 17:49:25 127.0.0.1 systemd[1]: Started ywc-watchdog.service - YWC Watchdog .NET Service for Raspberry Pi.
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]:       Starting YWC REST Watchdog Service polling status endpoint...
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]:       [CONFIG] Polling API: https://127.0.0.1:8443/api/cat/status
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]:       [CONFIG] Shelly IP: 127.0.0.1 (Channel: 1)
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]:       [CONFIG] Max TX Duration: 2 minutes
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]:       [CONFIG] Max Allowed SWR Meter: 2.5
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:49:26 127.0.0.1 ywc-watchdog[9699]:       [CONFIG] Allowed Antennas: [1]
Nov 11 17:52:40 127.0.0.1 ywc-watchdog[9699]:       Transmit started. Power: 35, Raw SWR: 255
Nov 11 17:52:40 127.0.0.1 ywc-watchdog[9699]: warn: RadioWatchdogWorker[0]
Nov 11 17:52:40 127.0.0.1 ywc-watchdog[9699]:       High SWR detected: 9.90 (Raw: 255, Violation count: 1)
Nov 11 17:52:41 127.0.0.1 ywc-watchdog[9699]: warn: RadioWatchdogWorker[0]
Nov 11 17:52:41 127.0.0.1 ywc-watchdog[9699]:       High SWR detected: 9.90 (Raw: 255, Violation count: 2)
Nov 11 17:52:41 127.0.0.1 ywc-watchdog[9699]: crit: RadioWatchdogWorker[0]
Nov 11 17:52:41 127.0.0.1 ywc-watchdog[9699]:       SWR threshold breached sustained limit (9.90). Executing emergency cutoff!
Nov 11 17:52:41 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:52:41 127.0.0.1 ywc-watchdog[9699]:       Successfully sent PTT cutoff command to radio.
Nov 11 17:52:42 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:52:42 127.0.0.1 ywc-watchdog[9699]:       Transmit ended. Resetting timer.
Nov 11 17:52:44 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:52:44 127.0.0.1 ywc-watchdog[9699]:       Transmit started. Power: 35, Raw SWR: 255
Nov 11 17:52:44 127.0.0.1 ywc-watchdog[9699]: warn: RadioWatchdogWorker[0]
Nov 11 17:52:44 127.0.0.1 ywc-watchdog[9699]:       High SWR detected: 9.90 (Raw: 255, Violation count: 1)
Nov 11 17:52:45 127.0.0.1 ywc-watchdog[9699]: warn: RadioWatchdogWorker[0]
Nov 11 17:52:45 127.0.0.1 ywc-watchdog[9699]:       High SWR detected: 9.90 (Raw: 255, Violation count: 2)
Nov 11 17:52:45 127.0.0.1 ywc-watchdog[9699]: crit: RadioWatchdogWorker[0]
Nov 11 17:52:45 127.0.0.1 ywc-watchdog[9699]:       SWR threshold breached sustained limit (9.90). Executing emergency cutoff!
Nov 11 17:52:45 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:52:45 127.0.0.1 ywc-watchdog[9699]:       Successfully sent PTT cutoff command to radio.
Nov 11 17:52:46 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:52:46 127.0.0.1 ywc-watchdog[9699]:       Transmit ended. Resetting timer.
Nov 11 17:54:14 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:54:14 127.0.0.1 ywc-watchdog[9699]:       Transmit started. Power: 35, Raw SWR: 0
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]: fail: RadioWatchdogWorker[0]
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:       Error occurred while polling radio status API. Triggering emergency Shelly shutdown due to lost communication!
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:       System.Net.Http.HttpRequestException: Connection refused (127.0.0.1:8443)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:        ---> System.Net.Sockets.SocketException (111): Connection refused
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.ThrowException(SocketError error, CancellationToken cancellationToken)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.System.Threading.Tasks.Sources.IValueTaskSource.GetResult(Int16 token)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at System.Net.Http.HttpConnectionPool.ConnectToTcpHostAsync(String host, Int32 port, HttpRequestMessage initialRequest, Boolean async, CancellationToken cancellationToken)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          --- End of inner exception stack trace ---
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at System.Net.Http.HttpConnectionPool.ConnectToTcpHostAsync(String host, Int32 port, HttpRequestMessage initialRequest, Boolean async, CancellationToken cancellationToken)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at System.Net.Http.HttpConnectionPool.ConnectAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at System.Net.Http.HttpConnectionPool.CreateHttp11ConnectionAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at System.Net.Http.HttpConnectionPool.InjectNewHttp11ConnectionAsync(QueueItem queueItem)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at System.Threading.Tasks.TaskCompletionSourceWithCancellation`1.WaitWithCancellationAsync(CancellationToken cancellationToken)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at System.Net.Http.HttpConnectionPool.SendWithVersionDetectionAndRetryAsync(HttpRequestMessage request, Boolean async, Boolean doRequestAuth, CancellationToken cancellationToken)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at System.Net.Http.RedirectHandler.SendAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at Microsoft.Extensions.Http.Logging.LoggingHttpMessageHandler.<SendCoreAsync>g__Core|4_0(HttpRequestMessage request, Boolean useAsync, CancellationToken cancellationToken)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at Microsoft.Extensions.Http.Logging.LoggingScopeHttpMessageHandler.<SendCoreAsync>g__Core|4_0(HttpRequestMessage request, Boolean useAsync, CancellationToken cancellationToken)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:          at RadioWatchdogWorker.ExecuteAsync(CancellationToken stoppingToken) in C:\Users\w8beu\Documents\dotnet\YwcWatchdog\Program.cs:line 71
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]: crit: RadioWatchdogWorker[0]
Nov 11 17:54:18 127.0.0.1 ywc-watchdog[9699]:       Connecting to Shelly WebSocket at ws://127.0.0.1/rpc...
Nov 11 17:54:19 127.0.0.1 ywc-watchdog[9699]: crit: RadioWatchdogWorker[0]
Nov 11 17:54:19 127.0.0.1 ywc-watchdog[9699]:       EMERGENCY: Sent WebSocket shutdown command to Shelly channel 1.
Nov 11 17:54:19 127.0.0.1 ywc-watchdog[9699]: crit: RadioWatchdogWorker[0]
Nov 11 17:54:19 127.0.0.1 ywc-watchdog[9699]:       VERIFIED: Shelly channel 1 is confirmed OFF.
Nov 11 17:54:33 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:54:33 127.0.0.1 ywc-watchdog[9699]:       Radio API connection restored. Resetting watchdog state.
Nov 11 17:54:33 127.0.0.1 ywc-watchdog[9699]: info: RadioWatchdogWorker[0]
Nov 11 17:54:33 127.0.0.1 ywc-watchdog[9699]:       Transmit ended. Resetting timer.
```
