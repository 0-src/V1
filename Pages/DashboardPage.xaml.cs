using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


namespace V1.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : UserControl
    {
        private DispatcherTimer refreshTimer;

        public DashboardPage()
        {
            InitializeComponent();

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                Debug.WriteLine("Refreshing Trade Data");
                this.DataContext = mainWindow.DashboardData; // ✅ Bind directly to the live DashboardData
            }
            StartAutoRefresh();
        }
        private void StartAutoRefresh()
        {
            if (refreshTimer == null)
            {
                refreshTimer = new DispatcherTimer();
                refreshTimer.Interval = TimeSpan.FromSeconds(35); // ✅ Refresh every 10 seconds
                refreshTimer.Tick += (sender, e) => RefreshTradeData();
            }

            refreshTimer.Start(); // ✅ Start timer when Dashboard is opened
        }

        private void StopAutoRefresh()
        {
            refreshTimer?.Stop(); // ✅ Stop timer when leaving Dashboard
        }

        private void RefreshTradeData()
        {
            var latestData = App.GetLatestAccountData(); // ✅ Fetch latest trade data

            if (latestData != null)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.UpdateUI(latestData); // ✅ Update UI only when the Dashboard is active
            }
        }

        // ✅ Stop refreshing when the user leaves the Dashboard
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            StopAutoRefresh();
        }
    }
}

