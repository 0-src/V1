using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace V1.Pages
{
    public partial class AlgoExecutions : UserControl
    {
        private readonly string targetUrl = "https://www.tradingview.com/chart/QfDkHINN/?symbol=CME_MINI%3ANQ1%21";
        private List<string> capturedLogs = new List<string>();
        public ObservableCollection<string> LogEntries { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> DebugMessages { get; set; } = new ObservableCollection<string>();

        private CoreWebView2Environment _webViewEnvironment;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint SetThreadExecutionState(uint esFlags);

        private const uint ES_CONTINUOUS = 0x80000000;
        private const uint ES_SYSTEM_REQUIRED = 0x00000001;
        private const uint ES_DISPLAY_REQUIRED = 0x00000002;

        public AlgoExecutions()
        {
            InitializeComponent();
            orderList.ItemsSource = LogEntries;
            debugList.ItemsSource = DebugMessages;
            InitializeWebView();
            StartKeepAliveTask();
        }

        private async void InitializeWebView()
        {
            try
            {
                DebugLog("Initializing WebView2...");

                _webViewEnvironment = await CoreWebView2Environment.CreateAsync(userDataFolder: "C:\\Temp\\WebView2Algo");
                await webView.EnsureCoreWebView2Async(_webViewEnvironment);

                webView.CoreWebView2.NavigationCompleted += WebView_NavigationCompleted;
                webView.CoreWebView2.Navigate(targetUrl);
            }
            catch (Exception ex)
            {
                DebugLog("Error initializing WebView2: " + ex.Message, true);
            }
        }

        private async void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                Task.Delay(1000).Wait();
                await webView.CoreWebView2.ExecuteScriptAsync("document.querySelector('button#log')?.click();");
                DebugLog("TradingView alerts page loaded.");
                webView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;
                await webView.CoreWebView2.ExecuteScriptAsync(observerScript);
            }
            else
            {
                DebugLog("Navigation failed!", true);
            }
        }

        private async void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string rawJson = e.WebMessageAsJson;
                Debug.WriteLine($"📥 Received raw JSON: {rawJson}");

                if (string.IsNullOrWhiteSpace(rawJson) || rawJson == "null")
                {
                    Debug.WriteLine("⚠️ Received empty or null JSON. Ignoring.");
                    return;
                }

                if (rawJson.StartsWith("\"") && rawJson.EndsWith("\""))
                {
                    rawJson = rawJson.Substring(1, rawJson.Length - 2);
                    rawJson = rawJson.Replace("\\\"", "\""); // Fix escaped quotes
                }

                var logs = JsonConvert.DeserializeObject<List<TradeLog>>(rawJson);
                bool newLogFound = false;

                DateTime nowUtc = DateTime.UtcNow;
                TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"); // Adjust to your region

                foreach (var log in logs)
                {
                    if (!DateTime.TryParseExact(log.time, "yyyy-MM-ddTHH:mm:ssZ",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tradeTimeLocal))
                    {
                        Debug.WriteLine($"❌ Invalid trade time format: {log.time}");
                        continue;
                    }

                    // ✅ Ensure `tradeTimeLocal` is explicitly marked as UTC-6 (Adjust if needed)
                    tradeTimeLocal = DateTime.SpecifyKind(tradeTimeLocal, DateTimeKind.Unspecified);

                    // ✅ Convert trade time from UTC-6 to UTC
                    DateTime tradeTimeUtc = TimeZoneInfo.ConvertTimeToUtc(tradeTimeLocal, localTimeZone);

                    Debug.WriteLine($"⏱️ Now UTC: {nowUtc}, Trade Time UTC: {tradeTimeUtc}");

                    TimeSpan timeDifference = nowUtc - tradeTimeUtc;
                    Debug.WriteLine($"📊 Trade Age: {timeDifference.TotalMilliseconds} milliseconds");

                    string logString = JsonConvert.SerializeObject(log);
                    if (timeDifference.TotalSeconds > 60)
                    {
                        Debug.WriteLine($"⏳ Ignoring old trade: {log.action.ToUpper()} {log.ticker} at {log.price} (Age: {timeDifference.TotalSeconds}s)");
                        LogEntries.Add($"⏳ Old trade: {log.action.ToUpper()} {log.ticker} at {log.price} (Age: {timeDifference.TotalSeconds}s)");
                        capturedLogs.Add(logString);
                        SaveLogsToFile();
                        continue;
                    }

                    if (!capturedLogs.Contains(logString))
                    {
                        await ExecuteTrade(log);
                        DebugLog($"🚀 Executing Trade: {log.action.ToUpper()} {log.quantity} {log.ticker} at {log.price} (Age: {timeDifference.TotalMilliseconds}s)");
                        LogEntries.Add($"{log.ticker} - {log.action.ToUpper()} @ {log.price} :: {timeDifference.TotalMilliseconds}ms");
                        capturedLogs.Add(logString);
                        ShowWindowsNotification("Trade Executed", $"Executed: {log.action.ToUpper()} - {log.ticker} at {log.price} :: Delay - {timeDifference.Milliseconds}");
                        SaveLogsToFile();
                        newLogFound = true;
                    }
                }

                if (newLogFound)
                {
                    SaveLogsToFile();
                    Debug.WriteLine($"✅ {capturedLogs.Count} logs saved to logs.json");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error processing logs: {ex.Message}");
            }
        }

        private async Task ExecuteTrade(TradeLog trade)
        {
            try
            {
                Debug.WriteLine($"🚀 Executing Trade: {trade.sentiment.ToUpper()} {trade.quantity} {trade.ticker} at {trade.price}");

                string script = "";

                // ✅ Hardcoded scripts for each sentiment
                if (trade.action.ToLower() == "buy")
                {
                    script = @"
            (function() {
                let button = document.querySelector('[data-name=""buy-order-button""]');
                if (button) {
                    button.click();
                    window.chrome.webview.postMessage('✅ Trade executed: LONG');
                } else {
                    window.chrome.webview.postMessage('❌ BUY button NOT found');
                }
            })();
            ";
                }
                else if (trade.action.ToLower() == "sell")
                {
                    script = @"
            (function() {
                let button = document.querySelector('[data-name=""sell-order-button""]');
                if (button) {
                    button.click();
                    window.chrome.webview.postMessage('✅ Trade executed: SHORT');
                } else {
                    window.chrome.webview.postMessage('❌ SELL button NOT found');
                }
            })();
            ";
                }
                else
                {
                    Debug.WriteLine("⚠️ Invalid sentiment. Skipping trade.");
                    return;
                }

                // ✅ Execute JavaScript inside WebView2
                await webView.CoreWebView2.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error executing trade: {ex.Message}");
            }
        }

        private void ShowWindowsNotification(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }



        private void SaveLogsToFile()
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "V1_Logs", "logs.json");

                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                if (capturedLogs.Count == 0)
                {
                    DebugLog("⚠️ No new logs to save. Skipping file write.");
                    return;
                }

                File.WriteAllText(filePath, JsonConvert.SerializeObject(capturedLogs, Formatting.Indented));
            }
            catch (Exception ex)
            {
                DebugLog("❌ Error saving logs: " + ex.Message, true);
            }
        }
        /// <summary>
        /// JavaScript MutationObserver script to detect new TradingView log entries.
        /// </summary>
        private readonly string observerScript = @"
(function() {
    function extractLogs() {
        let logs = [];
        document.querySelectorAll('[data-name=""alert-log-item""]').forEach(item => {
            let messageDiv = item.querySelector('.message-PQUvhamm');
            if (messageDiv) {
                try {
                    let parsedLog = JSON.parse(messageDiv.innerText.trim());
                    logs.push(parsedLog);
                } catch (err) {
                    console.error(""❌ Error parsing existing log JSON:"", err);
                }
            }
        });

        if (logs.length > 0) {
            console.log(""📋 Sending existing logs:"", logs);
            window.chrome.webview.postMessage(JSON.stringify(logs));
        }
    }

    function attachObserver() {
        let targetNode = document.querySelector('.scrollContainer-RT_yiIRf');

        if (!targetNode) {
            console.log(""⏳ Waiting for TradingView log container..."");
            setTimeout(attachObserver, 500);
            return;
        }

        console.log(""✅ MutationObserver attached."");

        // 🔥 Extract & send old logs BEFORE starting observer
        extractLogs();

        let observer = new MutationObserver(function(mutations) {
            let newLogs = [];
            mutations.forEach(mutation => {
                mutation.addedNodes.forEach(node => {
                    if (node.nodeType === 1 && node.matches('[data-name=""alert-log-item""]')) {
                        let messageDiv = node.querySelector('.message-PQUvhamm');
                        if (messageDiv) {
                            try {
                                let parsedLog = JSON.parse(messageDiv.innerText.trim());
                                newLogs.push(parsedLog);
                            } catch (err) {
                                console.error(""❌ Error parsing new log JSON:"", err);
                            }
                        }
                    }
                });
            });

            if (newLogs.length > 0) {
                console.log(""🔄 New logs detected: "", newLogs);
                window.chrome.webview.postMessage(JSON.stringify(newLogs));
            }
        });

        observer.observe(targetNode, { childList: true, subtree: true });
    }

    attachObserver();
})();";

        private void StartKeepAliveTask()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
                    Task.Delay(5000).Wait();
                }
            });
        }

        /// <summary>
        /// Adds debug messages to the UI and console.
        /// </summary>
        private void DebugLog(string message, bool isError = false)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DebugMessages.Add(message);
            });

            if (isError)
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Debug.WriteLine("[AlgoExecutions] " + message);
        }

        public class TradeLog
        {
            public string ticker { get; set; }
            public string action { get; set; }
            public string sentiment { get; set; }
            public string quantity { get; set; }
            public string price { get; set; }
            public string time { get; set; }
        }
    }
}