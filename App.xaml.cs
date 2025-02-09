using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using V1.Scripts;
using V1.Scripts.Classes;

namespace V1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private string GetEmbeddedScript(string resourceName)
        {

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new Exception($"Resource '{resourceName}' not found.");
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }



        private CoreWebView2Environment _environment;
        private CoreWebView2Controller _controller;
        private CoreWebView2 _webView;

        private DispatcherTimer _tradeDataTimer;

        private AccountData _accountData;


        private bool _isLoggedIn = false;

        private splashScreen _splash;
        private MainWindow _mainWindow;
        private HiddenWindowHost _hiddenHost;

        private TaskCompletionSource<bool> _scrapingTaskCompletionSource = new TaskCompletionSource<bool>();


        private async void OnStartup(object sender, StartupEventArgs e)
        {
            _splash = new splashScreen();


            _mainWindow = new MainWindow();
            _mainWindow.Visibility = Visibility.Hidden;

            _splash.UpdateStatus("Loading...");

            await InitializeWebView2Async();
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsVisible)
            {
                // Main window is still open, so don't exit
                return;
            }

            // If main window is closed, exit the application
            Environment.Exit(0);
        }

        private async void OnLoginConfirmed()
        {
            string username = _splash.SelectedUsername;
            string password = _splash.SelectedPassword;

            await HandleLogin(username, password);
            _isLoggedIn = true;

            // Proceed with scraping
            await ScrapeAndUpdateData();
        }


        private async Task InitializeWebView2Async()
        {
            try
            {
                _splash.UpdateStatus("Loading WebView2 environment...");

                _environment = await CoreWebView2Environment.CreateAsync(null);

                _hiddenHost = new HiddenWindowHost(false);
                IntPtr hwnd = _hiddenHost.WindowHandle;

                _controller = await _environment.CreateCoreWebView2ControllerAsync(hwnd);


                var hostWindow = _hiddenHost.HiddenWindow;

                _controller.Bounds = new System.Drawing.Rectangle(
                    0,
                    0,
                    (int)hostWindow.Width,
                    (int)hostWindow.Height
                );

                _webView = _controller.CoreWebView2;
                _webView.NavigationCompleted += WebView_NavigationCompleted;
                _splash.UpdateStatus("Loading Tradovate...");
                _webView.Navigate("https://trader.tradovate.com/welcome");
            }
            catch (Exception ex)
            {
                _splash.UpdateStatus("Error Initializing WebView2...");
                Debug.WriteLine($"Error initializing WebView2: {ex.Message}");
                Console.Error.WriteLine(ex.ToString());
            }
        }
        private async void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (_webView.Source.Contains("tradovate.com"))
            {
                if (!_isLoggedIn)
                {
                    // Show splash without blocking execution
                    _splash.Show();
                    _splash.LoginConfirmed += OnLoginConfirmed; // Subscribe to login event
                }
                else
                {
                    await ScrapeAndUpdateData();
                }
            }
        }


        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            try
            {
                string jsonData = args.WebMessageAsJson;
                Debug.WriteLine("Received from JavaScript: " + jsonData);

                // Parse JSON
                _accountData = JsonConvert.DeserializeObject<AccountData>(jsonData);

                // Update the UI on the main thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var mainWindow = _mainWindow;
                    mainWindow?.UpdateUI(_accountData);
                });

                _scrapingTaskCompletionSource.TrySetResult(true);
                StartTradeDataLoop();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error processing web message: " + ex.Message);
                _scrapingTaskCompletionSource.TrySetResult(false);

            }
        }

        private async Task HandleLogin(string username, string password)
        {
            _splash?.UpdateStatus("Running login script...");

            string script = GetEmbeddedScript("V1.Scripts.loginScript.js")
                .Replace("{{Username}}", username)
                .Replace("{{Password}}", password);


            await _webView.ExecuteScriptAsync(script);
            _splash?.UpdateStatus("Login successful!");
        }

        public async Task ScrapeAndUpdateData()
        {
            try
            {
                _splash.UpdateStatus("Scraping account data...");

                _scrapingTaskCompletionSource = new TaskCompletionSource<bool>();


                // Ensure we only attach the event handler once
                _webView.WebMessageReceived -= WebView_WebMessageReceived;
                _webView.WebMessageReceived += WebView_WebMessageReceived;

                // Execute JavaScript script (triggers scraping)
                await _webView.ExecuteScriptAsync(GetEmbeddedScript("V1.Scripts.DataScraper.js"));

                await _scrapingTaskCompletionSource.Task;

                _splash.UpdateStatus("Finished Scraping Account Data");

                // Close splash after scraping starts
                await Task.Delay(1000);
                _mainWindow.Visibility = Visibility.Visible;
                await Task.Delay(500);
                _splash.Close();
            }
            catch
            {
                _splash.UpdateStatus("Failed to scrape data");
                await Task.Delay(1000);
                Application.Current.Shutdown();
            }
        }

        public async Task StartTradeDataLoop()
        {
            if (_tradeDataTimer == null)
            {
                _tradeDataTimer = new DispatcherTimer();
                _tradeDataTimer.Interval = TimeSpan.FromSeconds(30);
                _tradeDataTimer.Tick += async (sender, e) => await ScrapeAndUpdateData();
            }


            _tradeDataTimer.Start(); // ✅ Auto-fetch trade data
            _webView.Navigate("https://trader.tradovate.com/");
        }

        public static AccountData GetLatestAccountData()
        {
            var appInstance = (App)Application.Current;
            return appInstance._accountData;
        }
    }
}

