# YwcWatchdog

## Log Output Reference

1.  Startup

    ```
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       Application starting up. ReleaseId: 1.3.0
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    ```

2.  Connect to ywc {"YwcApiUrl": "https://127.0.0.1:8443"}

    ```
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       Starting YWC REST Watchdog Service polling status endpoint...
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       [CONFIG] Polling API: https://127.0.0.1:8443
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    ```

3.  Connect to Shelly {"ShellyIp": "127.0.0.1"}, {"ShellyChannel": 1}

    ```
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       [CONFIG] Shelly IP: 127.0.0.1 (Channel: 0)
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    ```

4.  Read ywc-watchdog settings [appsettings.json]:

    ```
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       [CONFIG] Permitted Antennas: [1]
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       [CONFIG] Max Allowed SWR: 2.5
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       [CONFIG] Max Allowed Temp: 60 Celsius
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       [CONFIG] Max TX Duration: 2 minutes
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       [CONFIG] Meter Calibration Timer: 300 seconds
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    ```

5.  Read Pwr/Swr/Temp calibration [api endpoint: /cat/status]:
    ```
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]:       [CONFIG] SWR Meter Calibration Values: Raw 0 -> SWR 1, Raw 51 -> SWR 1.5, Raw 77 -> SWR 2, Raw 128 -> SWR 3, Raw 173 -> SWR 5, Raw 242 -> SWR 9.9
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]:       [CONFIG] PWR Meter Calibration Values: Raw 30 -> PWR 5, Raw 76 -> PWR 25, Raw 112 -> PWR 50, Raw 157 -> PWR 100, Raw 190 -> PWR 150, Raw 222 -> PWR 200
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]:       [CONFIG] Temp Meter Calibration Values: Raw 14 ->  TMP 0, Raw 60 ->  TMP 20, Raw 106 ->  TMP 40, Raw 152 ->  TMP 60, Raw 198 ->  TMP 80, Raw 244 ->  TMP 100
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    ```
6.  Apply Pwr/Swr/Temp calibration:
    ```
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]:       [CONFIG] 5 API Power calibration points applied.
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]:       [Config] 6 API SWR calibration points applied.
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]:       [Config] 6 Temp calibration points applied.
    Nov 21 10:18:56 Argon5 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    ```
7.  Mesure YWC - ywc-watchdog calibration cycle completion {"CalibrateTimerSec": 300}

    ```
    Nov 21 10:28:57 Argon5 ywc-watchdog[50112]:       [CONFIG] 5 API Power calibration points applied.
    Nov 21 10:28:57 Argon5 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:28:57 Argon5 ywc-watchdog[50112]:       [Config] 6 API SWR calibration points applied.
    Nov 21 10:28:57 Argon5 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:28:57 Argon5 ywc-watchdog[50112]:       [Config] 6 Temp calibration points applied.
    Nov 21 10:33:57 Argon5 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:33:57 Argon5 ywc-watchdog[50112]:       [CONFIG] 5 API Power calibration points applied.
    Nov 21 10:33:57 Argon5 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:33:57 Argon5 ywc-watchdog[50112]:       [Config] 6 API SWR calibration points applied.
    Nov 21 10:33:57 Argon5 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:33:57 Argon5 ywc-watchdog[50112]:       [Config] 6 Temp calibration points applied.
    ```

8.  Track Pwr on/off status [api endpoint: /api/cat/radioPower]

    ```
    Nov 21 08:23:31 Argon5 ywc-watchdog[49714]:       [CONFIG] Radio Power Status: On
    Nov 21 08:23:38 Argon5 ywc-watchdog[49714]: info: RadioWatchdogWorker[0]
    Nov 21 10:15:56 Argon5 ywc-watchdog[50059]:       [CONFIG] Radio Power Status: Off
    Nov 21 10:15:56 Argon5 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    ```

9.  Track transmit mode status [api endpoint: /api/cat/status/Activevfo.mode]
    ```
    Nov 15 13:48:13 127.0.0.1 ywc-watchdog[33962]:       Transmit started. Mode: LSB, Power: 5.00, SWR: 1.0
    Nov 15 13:48:16 127.0.0.1 ywc-watchdog[33962]: info: RadioWatchdogWorker[0]
    Nov 15 13:48:16 127.0.0.1 ywc-watchdog[33962]:       Transmit ended. Resetting timer.
    Nov 15 13:48:27 127.0.0.1 ywc-watchdog[33962]: info: RadioWatchdogWorker[0]
    Nov 15 14:30:30 127.0.0.1 ywc-watchdog[34147]:       Transmit started. Mode: CW-U, Power: 5.00, SWR: 1.0
    Nov 15 14:30:31 127.0.0.1 ywc-watchdog[34147]: info: RadioWatchdogWorker[0]
    Nov 15 14:38:27 127.0.0.1 ywc-watchdog[34147]:       Transmit started. Mode: AM, Power: 6.74, SWR: 1.0
    Nov 15 14:38:30 127.0.0.1 ywc-watchdog[34147]: info: RadioWatchdogWorker[0]
    Nov 15 14:38:30 127.0.0.1 ywc-watchdog[34147]:       Transmit ended. Resetting timer.
    Nov 15 14:38:32 127.0.0.1 ywc-watchdog[34147]: info: RadioWatchdogWorker[0]
    ```
10. Inhibit on invalid antenna [api endpoint: /api/cat/status/Activevfo.Antenna {"AllowedAntennas": [1]}]

    ```
    Nov 21 08:21:19 Argon5 ywc-watchdog[49644]:       Invalid Antenna detected (2) during transmission. Executing emergency cutoff!
    Nov 21 08:21:20 Argon5 ywc-watchdog[49644]: info: RadioWatchdogWorker[0]
    Nov 21 08:21:20 Argon5 ywc-watchdog[49644]:       Successfully sent PTT cutoff command to radio.
    Nov 21 08:21:31 Argon5 ywc-watchdog[49644]: info: RadioWatchdogWorker[0]
    ```

11. Inhibit on high swr [api endpoint: /api/calibration/all {MaxSwr: 2.5}]

    ```
    Nov 15 14:31:05 127.0.0.1 ywc-watchdog[34147]:       Transmit started. Mode: CW-U, Power: 7.17, SWR: 1.0
    Nov 15 14:31:06 127.0.0.1 ywc-watchdog[34147]: warn: RadioWatchdogWorker[0]
    Nov 15 14:31:06 127.0.0.1 ywc-watchdog[34147]:       High SWR detected: 9.90 (Raw: 255, Violation count: 1)
    Nov 15 14:31:06 127.0.0.1 ywc-watchdog[34147]: warn: RadioWatchdogWorker[0]
    Nov 15 14:31:06 127.0.0.1 ywc-watchdog[34147]:       High SWR detected: 9.90 (Raw: 255, Violation count: 2)
    Nov 15 14:31:06 127.0.0.1 ywc-watchdog[34147]: crit: RadioWatchdogWorker[0]
    Nov 15 14:31:06 127.0.0.1 ywc-watchdog[34147]:       SWR threshold breached sustained limit (9.90). Executing emergency cutoff!
    Nov 15 14:31:06 127.0.0.1 ywc-watchdog[34147]: info: RadioWatchdogWorker[0]
    Nov 15 14:31:06 127.0.0.1 ywc-watchdog[34147]:       Successfully sent PTT cutoff command to radio.
    Nov 15 14:31:06 127.0.0.1 ywc-watchdog[34147]: info: RadioWatchdogWorker[0]
    ```

12. Inhibit on high Temp [api enpoint: /api/calibration/all {MaxTmp: 30}]

    ```
    Nov 21 08:21:31 Argon5 ywc-watchdog[49644]:       Transmit started. Mode: LSB, Power: 5.00, SWR: 1.1, Temp: 39.6
    Nov 21 08:21:31 Argon5 ywc-watchdog[49644]: warn: RadioWatchdogWorker[0]
    Nov 21 08:21:31 Argon5 ywc-watchdog[49644]:       High Temp detected: 39.57, Violation count: 1)
    Nov 21 08:21:31 Argon5 ywc-watchdog[49644]: warn: RadioWatchdogWorker[0]
    Nov 21 08:21:31 Argon5 ywc-watchdog[49644]:       High Temp detected: 39.57, Violation count: 2)
    Nov 21 08:21:31 Argon5 ywc-watchdog[49644]: crit: RadioWatchdogWorker[0]
    Nov 21 08:21:31 Argon5 ywc-watchdog[49644]:       Temp threshold breached sustained limit (39.57). Executing emergency cutoff!
    Nov 21 08:21:31 Argon5 ywc-watchdog[49644]: info: RadioWatchdogWorker[0]
    Nov 21 08:21:31 Argon5 ywc-watchdog[49644]:       Successfully sent PTT cutoff command to radio.
    Nov 21 08:21:31 Argon5 ywc-watchdog[49644]: info: RadioWatchdogWorker[0]
    ```

13. Inhibit on MaxDurationTx threshold {"MaxTxDurationMinutes": 0.05}

    ```
    Nov 15 15:03:35 127.0.0.1 ywc-watchdog[34147]:       Transmit started. Mode: LSB, Power: 5.00, SWR: 1.0
    Nov 15 15:03:38 127.0.0.1 ywc-watchdog[34147]: crit: RadioWatchdogWorker[0]
    Nov 15 15:03:38 127.0.0.1 ywc-watchdog[34147]:       Max TX duration exceeded (0.05 mins). Triggering emergency PTT cutoff!
    Nov 15 15:03:38 127.0.0.1 ywc-watchdog[34147]: info: RadioWatchdogWorker[0]
    Nov 15 15:03:38 127.0.0.1 ywc-watchdog[34147]:       Successfully sent PTT cutoff command to radio.
    Nov 15 15:03:39 127.0.0.1 ywc-watchdog[34147]: info: RadioWatchdogWorker[0]
    ```

14. Failover to Shelly on api endpoint loss of communication [docker compose down ywc]
    ```
    Nov 15 15:07:48 127.0.0.1 ywc-watchdog[34354]:       Transmit started. Mode: LSB, Power: 5.00, SWR: 1.0
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]: fail: RadioWatchdogWorker[0]
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:       Error occurred while polling radio status API. Triggering emergency Shelly shutdown due to lost communication!
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:       System.Net.Http.HttpRequestException: Connection refused (127.0.0.1:8443)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:        ---> System.Net.Sockets.SocketException (111): Connection refused
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.ThrowException(SocketError error, CancellationToken cancellationToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.System.Threading.Tasks.Sources.IValueTaskSource.GetResult(Int16 token)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at System.Net.Http.HttpConnectionPool.ConnectToTcpHostAsync(String host, Int32 port, HttpRequestMessage initialRequest, Boolean async, CancellationToken cancellationToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          --- End of inner exception stack trace ---
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at System.Net.Http.HttpConnectionPool.ConnectToTcpHostAsync(String host, Int32 port, HttpRequestMessage initialRequest, Boolean async, CancellationToken cancellationToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at System.Net.Http.HttpConnectionPool.ConnectAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at System.Net.Http.HttpConnectionPool.CreateHttp11ConnectionAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at System.Net.Http.HttpConnectionPool.InjectNewHttp11ConnectionAsync(QueueItem queueItem)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at System.Threading.Tasks.TaskCompletionSourceWithCancellation`1.WaitWithCancellationAsync(CancellationToken cancellationToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at System.Net.Http.HttpConnectionPool.SendWithVersionDetectionAndRetryAsync(HttpRequestMessage request, Boolean async, Boolean doRequestAuth, CancellationToken cancellationToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at System.Net.Http.RedirectHandler.SendAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at Microsoft.Extensions.Http.Logging.LoggingHttpMessageHandler.<SendCoreAsync>g__Core|4_0(HttpRequestMessage request, Boolean useAsync, CancellationToken cancellationToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at Microsoft.Extensions.Http.Logging.LoggingScopeHttpMessageHandler.<SendCoreAsync>g__Core|4_0(HttpRequestMessage request, Boolean useAsync, CancellationToken cancellationToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:          at RadioWatchdogWorker.ExecuteAsync(CancellationToken stoppingToken)
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]: crit: RadioWatchdogWorker[0]
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:       Connecting to Shelly WebSocket at ws://192.168.1.136/rpc...
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]: crit: RadioWatchdogWorker[0]
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:       EMERGENCY: Sent WebSocket shutdown command to Shelly channel 1.
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]: crit: RadioWatchdogWorker[0]
    Nov 15 15:07:51 127.0.0.1 ywc-watchdog[34354]:       VERIFIED: Shelly channel 1 is confirmed OFF.
    ```
