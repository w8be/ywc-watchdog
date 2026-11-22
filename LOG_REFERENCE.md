# YwcWatchdog

## Log Output Reference

[1. Startup](#startup)\
[2. Connect to ywc](#connect-to-ywc-ywcapiurl-https1270018443)\
 [3. Connect to Shelly](#connect-to-shelly-shellyip-127001-shellychannel-1)\
 [4. Read ywc-watchdog settings](#read-ywc-watchdog-settings-appsettingsjson)\
 [5. Read Pwr,Swr,Temp Calibration](#read-pwrswrtemp-calibration-api-endpoint-catstatus)\
 [6. Apply Pwr,Swr,Temp Calibraion](#apply-pwrswrtemp-calibration)\
 [7. Measure YWC - ywc-watchdog cycle](#mesure-ywc---ywc-watchdog-calibration-cycle-completion-calibratetimersec-300)\
 [8. Track Radio Power](#track-pwr-onoff-status-api-endpoint-apicatradiopower)\
 [9. Track Transmit Mode](#track-transmit-mode-status-api-endpoint-apicatstatusactivevfomode)\
 [10. Inhibit Invalid Antenna](#inhibit-on-invalid-antenna-api-endpoint-apicatstatusactivevfoantenna-allowedantennas-1)\
 [11. Inhibit High SWR](#inhibit-on-high-swr-api-endpoint-apicalibrationall-maxswr-25)\
 [12. Inhibit High Temperature](#inhibit-on-high-temp-api-enpoint-apicalibrationall-maxtmp-30)\
 [13. Inhibit Max TX Duration](#inhibit-on-maxdurationtx-threshold-maxtxdurationminutes-005)\
 [14. Shelly Failover Test](#failover-to-shelly-on-api-endpoint-loss-of-communication-docker-compose-down-ywc)\
 [15. Confirm YWC - ywc-watchdog API restore](#confirm-ywc-api-endpoint-on-restore-communications)

1.  ### Startup

    ```yaml
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       Application starting up. ReleaseId: 1.3.0
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    ```

2.  ### Connect to ywc {"YwcApiUrl": "https://127.0.0.1:8443"}

    ```yaml
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       Starting YWC REST Watchdog Service polling status endpoint...
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       [CONFIG] Polling API: https://127.0.0.1:8443
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    ```

3.  ### Connect to Shelly {"ShellyIp": "127.0.0.1"}, {"ShellyChannel": 1}

    ```yaml
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]:       [CONFIG] Shelly IP: 127.0.0.1 (Channel: 0)
    Nov 21 10:15:55 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    ```

4.  ### Read ywc-watchdog settings [appsettings.json]:

    ```yaml
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

5.  ### Read Pwr/Swr/Temp calibration [api endpoint: /cat/status]:
    ```yaml
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]:       [CONFIG] SWR Meter Calibration Values: Raw 0 -> SWR 1, Raw 51 -> SWR 1.5, Raw 77 -> SWR 2, Raw 128 -> SWR 3, Raw 173 -> SWR 5, Raw 242 -> SWR 9.9
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]:       [CONFIG] PWR Meter Calibration Values: Raw 30 -> PWR 5, Raw 76 -> PWR 25, Raw 112 -> PWR 50, Raw 157 -> PWR 100, Raw 190 -> PWR 150, Raw 222 -> PWR 200
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]:       [CONFIG] Temp Meter Calibration Values: Raw 14 ->  TMP 0, Raw 60 ->  TMP 20, Raw 106 ->  TMP 40, Raw 152 ->  TMP 60, Raw 198 ->  TMP 80, Raw 244 ->  TMP 100
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    ```
6.  ### Apply Pwr/Swr/Temp calibration:
    ```yaml
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]:       [CONFIG] 5 API Power calibration points applied.
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]:       [Config] 6 API SWR calibration points applied.
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]:       [Config] 6 Temp calibration points applied.
    Nov 21 10:18:56 127.0.0.1 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    ```
7.  ### Mesure YWC - ywc-watchdog calibration cycle completion {"CalibrateTimerSec": 300}

    ```yaml
    Nov 21 10:28:57 127.0.0.1 ywc-watchdog[50112]:       [CONFIG] 5 API Power calibration points applied.
    Nov 21 10:28:57 127.0.0.1 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:28:57 127.0.0.1 ywc-watchdog[50112]:       [Config] 6 API SWR calibration points applied.
    Nov 21 10:28:57 127.0.0.1 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:28:57 127.0.0.1 ywc-watchdog[50112]:       [Config] 6 Temp calibration points applied.
    Nov 21 10:33:57 127.0.0.1 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:33:57 127.0.0.1 ywc-watchdog[50112]:       [CONFIG] 5 API Power calibration points applied.
    Nov 21 10:33:57 127.0.0.1 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:33:57 127.0.0.1 ywc-watchdog[50112]:       [Config] 6 API SWR calibration points applied.
    Nov 21 10:33:57 127.0.0.1 ywc-watchdog[50112]: info: RadioWatchdogWorker[0]
    Nov 21 10:33:57 127.0.0.1 ywc-watchdog[50112]:       [Config] 6 Temp calibration points applied.
    ```

8.  ### Track Pwr on/off status [api endpoint: /api/cat/radioPower]

    ```yaml
    Nov 21 08:23:31 127.0.0.1 ywc-watchdog[49714]:       [CONFIG] Radio Power Status: On
    Nov 21 08:23:38 127.0.0.1 ywc-watchdog[49714]: info: RadioWatchdogWorker[0]
    Nov 21 10:15:56 127.0.0.1 ywc-watchdog[50059]:       [CONFIG] Radio Power Status: Off
    Nov 21 10:15:56 127.0.0.1 ywc-watchdog[50059]: info: RadioWatchdogWorker[0]
    ```

9.  ### Track transmit mode status [api endpoint: /api/cat/status/Activevfo.mode]
    ```yaml
    Nov 21 19:25:39 127.0.0.1 ywc-watchdog[51051]:       [CONFIG] Radio Power Status: On
    Nov 21 19:25:53 127.0.0.1 ywc-watchdog[51051]: info: RadioWatchdogWorker[0]
    Nov 21 19:25:53 127.0.0.1 ywc-watchdog[51051]:       Transmit started. Mode: LSB, Power: 5.00, SWR: 1.1, Temp: 36.5
    Nov 21 19:25:58 127.0.0.1 ywc-watchdog[51051]: info: RadioWatchdogWorker[0]
    Nov 21 19:25:58 127.0.0.1 ywc-watchdog[51051]:       Transmit ended. Resetting timer.
    Nov 21 19:26:18 127.0.0.1 ywc-watchdog[51051]: info: RadioWatchdogWorker[0]
    Nov 21 19:26:18 127.0.0.1 ywc-watchdog[51051]:       Transmit started. Mode: CW-U, Power: 5.00, SWR: 1.1, Temp: 36.5
    Nov 21 19:26:21 127.0.0.1 ywc-watchdog[51051]: info: RadioWatchdogWorker[0]
    Nov 21 19:26:21 127.0.0.1 ywc-watchdog[51051]:       Transmit ended. Resetting timer.
    Nov 21 19:26:34 127.0.0.1 ywc-watchdog[51051]: info: RadioWatchdogWorker[0]
    Nov 21 19:26:34 127.0.0.1 ywc-watchdog[51051]:       Transmit started. Mode: AM, Power: 5.00, SWR: 1.1, Temp: 38.3
    Nov 21 19:26:37 127.0.0.1 ywc-watchdog[51051]: info: RadioWatchdogWorker[0]
    Nov 21 19:26:37 127.0.0.1 ywc-watchdog[51051]:       Transmit ended. Resetting timer.
    Nov 21 19:27:07 127.0.0.1 ywc-watchdog[51051]: info: RadioWatchdogWorker[0]
    ```
10. ### Inhibit on invalid antenna [api endpoint: /api/cat/status/Activevfo.Antenna {"AllowedAntennas": [1]}]

    ```yaml
    Nov 21 08:21:19 127.0.0.1 ywc-watchdog[49644]:       Invalid Antenna detected (2) during transmission. Executing emergency cutoff!
    Nov 21 08:21:20 127.0.0.1 ywc-watchdog[49644]: info: RadioWatchdogWorker[0]
    Nov 21 08:21:20 127.0.0.1 ywc-watchdog[49644]:       Successfully sent PTT cutoff command to radio.
    Nov 21 08:21:31 127.0.0.1 ywc-watchdog[49644]: info: RadioWatchdogWorker[0]
    ```

11. ### Inhibit on high swr [api endpoint: /api/calibration/all {MaxSwr: 2.5}]

    ```yaml
    Nov 21 19:27:22 127.0.0.1 ywc-watchdog[51051]:       Transmit started. Mode: LSB, Power: 5.00, SWR: 1.1, Temp: 38.3
    Nov 21 19:27:23 127.0.0.1 ywc-watchdog[51051]: warn: RadioWatchdogWorker[0]
    Nov 21 19:27:23 127.0.0.1 ywc-watchdog[51051]:       High SWR detected: 2.24, Violation count: 1
    Nov 21 19:27:23 127.0.0.1 ywc-watchdog[51051]: warn: RadioWatchdogWorker[0]
    Nov 21 19:27:23 127.0.0.1 ywc-watchdog[51051]:       High SWR detected: 9.90, Violation count: 2
    Nov 21 19:27:23 127.0.0.1 ywc-watchdog[51051]: crit: RadioWatchdogWorker[0]
    Nov 21 19:27:23 127.0.0.1 ywc-watchdog[51051]:       SWR threshold breached sustained limit (9.90). Executing emergency cutoff!
    Nov 21 19:27:23 127.0.0.1 ywc-watchdog[51051]: info: RadioWatchdogWorker[0]
    Nov 21 19:27:23 127.0.0.1 ywc-watchdog[51051]:       Successfully sent PTT cutoff command to radio.
    Nov 21 19:27:23 127.0.0.1 ywc-watchdog[51051]: info: RadioWatchdogWorker[0]
    Nov 21 19:27:23 127.0.0.1 ywc-watchdog[51051]:       Transmit ended. Resetting timer.
    ```

12. ### Inhibit on high Temp [api enpoint: /api/calibration/all {MaxTmp: 30}]

    ```yaml
    Nov 21 08:21:31 127.0.0.1 ywc-watchdog[49644]:       Transmit started. Mode: LSB, Power: 5.00, SWR: 1.1, Temp: 39.6
    Nov 21 08:21:31 127.0.0.1 ywc-watchdog[49644]: warn: RadioWatchdogWorker[0]
    Nov 21 08:21:31 127.0.0.1 ywc-watchdog[49644]:       High Temp detected: 39.57, Violation count: 1)
    Nov 21 08:21:31 127.0.0.1 ywc-watchdog[49644]: warn: RadioWatchdogWorker[0]
    Nov 21 08:21:31 127.0.0.1 ywc-watchdog[49644]:       High Temp detected: 39.57, Violation count: 2)
    Nov 21 08:21:31 127.0.0.1 ywc-watchdog[49644]: crit: RadioWatchdogWorker[0]
    Nov 21 08:21:31 127.0.0.1 ywc-watchdog[49644]:       Temp threshold breached sustained limit (39.57). Executing emergency cutoff!
    Nov 21 08:21:31 127.0.0.1 ywc-watchdog[49644]: info: RadioWatchdogWorker[0]
    Nov 21 08:21:31 127.0.0.1 ywc-watchdog[49644]:       Successfully sent PTT cutoff command to radio.
    Nov 21 08:21:31 127.0.0.1 ywc-watchdog[49644]: info: RadioWatchdogWorker[0]
    ```

13. ### Inhibit on MaxDurationTx threshold {"MaxTxDurationMinutes": 0.05}

    ```yaml
    Nov 21 19:32:24 127.0.0.1 ywc-watchdog[51121]:       [CONFIG] Radio Power Status: On
    Nov 21 19:32:37 127.0.0.1 ywc-watchdog[51121]: info: RadioWatchdogWorker[0]
    Nov 21 19:32:37 127.0.0.1 ywc-watchdog[51121]:       Transmit started. Mode: LSB, Power: 5.00, SWR: 1.1, Temp: 38.3
    Nov 21 19:32:40 127.0.0.1 ywc-watchdog[51121]: crit: RadioWatchdogWorker[0]
    Nov 21 19:32:40 127.0.0.1 ywc-watchdog[51121]:       Max TX duration exceeded (0.05 mins). Triggering emergency PTT cutoff!
    Nov 21 19:32:40 127.0.0.1 ywc-watchdog[51121]: info: RadioWatchdogWorker[0]
    Nov 21 19:32:40 127.0.0.1 ywc-watchdog[51121]:       Successfully sent PTT cutoff command to radio.
    Nov 21 19:32:40 127.0.0.1 ywc-watchdog[51121]: info: RadioWatchdogWorker[0]
    Nov 21 19:32:40 127.0.0.1 ywc-watchdog[51121]:       Transmit ended. Resetting timer.
    ```

14. ### Failover to Shelly on api endpoint loss of communication [docker compose down ywc]

    ```yaml
    Nov 21 19:36:12 127.0.0.1 ywc-watchdog[51194]:       [CONFIG] Radio Power Status: On
    Nov 21 19:36:28 127.0.0.1 ywc-watchdog[51194]: info: RadioWatchdogWorker[0]
    Nov 21 19:36:28 127.0.0.1 ywc-watchdog[51194]:       Transmit started. Mode: LSB, Power: 5.00, SWR: 1.1, Temp: 39.6
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]: fail: RadioWatchdogWorker[0]
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:       Error occurred while polling radio status API. Triggering emergency Shelly shutdown due to lost communication!
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:       System.Net.Http.HttpRequestException: Connection refused (192.168.1.196:8443)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:        ---> System.Net.Sockets.SocketException (111): Connection refused
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.ThrowException(SocketError error, CancellationToken cancellationToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.System.Threading.Tasks.Sources.IValueTaskSource.GetResult(Int16 token)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at System.Net.Http.HttpConnectionPool.ConnectToTcpHostAsync(String host, Int32 port, HttpRequestMessage initialRequest, Boolean async, CancellationToken cancellationToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          --- End of inner exception stack trace ---
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at System.Net.Http.HttpConnectionPool.ConnectToTcpHostAsync(String host, Int32 port, HttpRequestMessage initialRequest, Boolean async, CancellationToken cancellationToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at System.Net.Http.HttpConnectionPool.ConnectAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at System.Net.Http.HttpConnectionPool.CreateHttp11ConnectionAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at System.Net.Http.HttpConnectionPool.InjectNewHttp11ConnectionAsync(QueueItem queueItem)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at System.Threading.Tasks.TaskCompletionSourceWithCancellation`1.WaitWithCancellationAsync(CancellationToken cancellationToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at System.Net.Http.HttpConnectionPool.SendWithVersionDetectionAndRetryAsync(HttpRequestMessage request, Boolean async, Boolean doRequestAuth, CancellationToken cancellationToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at System.Net.Http.RedirectHandler.SendAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at Microsoft.Extensions.Http.Logging.LoggingHttpMessageHandler.<SendCoreAsync>g__Core|4_0(HttpRequestMessage request, Boolean useAsync, CancellationToken cancellationToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at Microsoft.Extensions.Http.Logging.LoggingScopeHttpMessageHandler.<SendCoreAsync>g__Core|4_0(HttpRequestMessage request, Boolean useAsync, CancellationToken cancellationToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:          at RadioWatchdogWorker.ExecuteAsync(CancellationToken stoppingToken)
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]: crit: RadioWatchdogWorker[0]
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:       Connecting to Shelly WebSocket at ws://192.168.1.136/rpc...
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]: crit: RadioWatchdogWorker[0]
    Nov 21 19:36:32 127.0.0.1 ywc-watchdog[51194]:       EMERGENCY: Sent WebSocket shutdown command to Shelly channel 1.
    Nov 21 19:36:33 127.0.0.1 ywc-watchdog[51194]: crit: RadioWatchdogWorker[0]
    Nov 21 19:36:33 127.0.0.1 ywc-watchdog[51194]:       VERIFIED: Shelly channel 1 is confirmed OFF.
    ```

15. ### Confirm YWC API endpoint on restore communications:
    ```yaml
    Nov 21 19:39:19 127.0.0.1 ywc-watchdog[51194]:       Radio API connection restored. Resetting watchdog state.
    Nov 21 19:39:19 127.0.0.1 ywc-watchdog[51194]: info: RadioWatchdogWorker[0]
    Nov 21 19:39:19 127.0.0.1 ywc-watchdog[51194]:       Transmit ended. Resetting timer.
    Nov 21 19:39:20 127.0.0.1 ywc-watchdog[51194]: info: RadioWatchdogWorker[0]
    Nov 21 19:39:20 127.0.0.1 ywc-watchdog[51194]:       [CONFIG] Radio Power Status: Off
    ```
