using System.Windows;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System;
using System.IO;
using System.Collections.ObjectModel;
using V1.Scripts;
using System.Reflection;
using Newtonsoft.Json;
using V1.Scripts.Classes;
using System.Runtime.InteropServices;

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


        private bool _isLoggedIn = false;

        private splashScreen _splash;
        private MainWindow _mainWindow;
        private HiddenWindowHost _hiddenHost;

        private TaskCompletionSource<bool> _scrapingTaskCompletionSource = new TaskCompletionSource<bool>();


        private async void OnStartup(object sender,StartupEventArgs e)
        {
            _splash = new splashScreen();


            _mainWindow = new MainWindow();
            _mainWindow.Visibility = Visibility.Hidden;

            _splash.UpdateStatus("Loading...");

            await InitializeWebView2Async();
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

                _hiddenHost = new HiddenWindowHost(true);
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
                var retrievedJson = JsonConvert.DeserializeObject<AccountData>(jsonData);

                // Update the UI on the main thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var mainWindow = _mainWindow;
                    mainWindow?.UpdateUI(retrievedJson);
                });

                _scrapingTaskCompletionSource.TrySetResult(true);

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

        private async Task ScrapeAndUpdateData()
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
                _splash.Close();
                _mainWindow.Visibility = Visibility.Visible;
            }
            catch
            {
                _splash.UpdateStatus("Failed to scrape data");
                await Task.Delay(1000);
                Application.Current.Shutdown();
            }
        }
    }
}

