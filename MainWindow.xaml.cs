using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using V1.Pages;
using V1.Scripts;
using V1.Scripts.Classes;

namespace V1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public DashboardData DashboardData { get; private set; } // Make it a public property
        public MainWindow()
        {
            InitializeComponent();
            DashboardData = new DashboardData(); // Assign it once
            DataContext = this;

            MainContent.Content = new DashboardPage();
        }

        // Handles window dragging
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Handles window close button
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
           MainContent.Content = new DashboardPage();
        }

        private void AlgoExecutionsButton_Click(object sender, RoutedEventArgs e)
        {
          MainContent.Content = new AlgoExecutions();
        }


        // Method to update UI when new data is received
        public void UpdateUI(AccountData accountData)
        {
            try
            {
                Debug.WriteLine("Updating UI with new account data");

                // Ensure UI updates are done on the main thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Debug.Write(accountData.ToString());
                    // **Update Account Details**
                    DashboardData.AccountName = accountData.AccountName;
                    DashboardData.TotalPL = accountData.PerformanceData.AllTrades.TotalPL;
                    DashboardData.Equity = accountData.Equity;

                    Debug.WriteLine(accountData.PerformanceData.AllTrades.TotalPL);
                    Debug.WriteLine(accountData.PerformanceData.LosingTrades.TotalLoss);
                    Debug.WriteLine(accountData.PerformanceData.ProfitTrades.TotalProfit);

                    // **Update Profit Trades**
                    DashboardData.TotalProfit = accountData.PerformanceData.ProfitTrades.TotalProfit;
                    DashboardData.NumberOfWinningTrades = accountData.PerformanceData.ProfitTrades.NumberOfWinningTrades;
                    DashboardData.NumberOfWinningContracts = accountData.PerformanceData.ProfitTrades.NumberOfWinningContracts;
                    DashboardData.LargestWinningTrade = accountData.PerformanceData.ProfitTrades.LargestWinningTrade;
                    DashboardData.AverageWinningTrade = accountData.PerformanceData.ProfitTrades.AverageWinningTrade;
                    DashboardData.StdDevWinningTrade = DataConverter.ConvertToDecimal(accountData.PerformanceData.ProfitTrades.StdDevWinningTrade);
                    DashboardData.AverageWinningTradeTime = accountData.PerformanceData.ProfitTrades.AverageWinningTradeTime;
                    DashboardData.LongestWinningTradeTime = accountData.PerformanceData.ProfitTrades.LongestWinningTradeTime;
                    DashboardData.MaxRunUp = accountData.PerformanceData.ProfitTrades.MaxRunUp;

                    // **Update Losing Trades**
                    DashboardData.TotalLoss = accountData.PerformanceData.LosingTrades.TotalLoss;
                    DashboardData.NumberOfLosingTrades = accountData.PerformanceData.LosingTrades.NumberOfLosingTrades;
                    DashboardData.NumberOfLosingContracts = accountData.PerformanceData.LosingTrades.NumberOfLosingContracts;
                    DashboardData.LargestLosingTrade = accountData.PerformanceData.LosingTrades.LargestLosingTrade;
                    DashboardData.AverageLosingTrade = accountData.PerformanceData.LosingTrades.AverageLosingTrade;
                    DashboardData.StdDevLosingTrade = DataConverter.ConvertToDecimal(accountData.PerformanceData.LosingTrades.StdDevLosingTrade);
                    DashboardData.AverageLosingTradeTime = accountData.PerformanceData.LosingTrades.AverageLosingTradeTime;
                    DashboardData.LongestLosingTradeTime = accountData.PerformanceData.LosingTrades.LongestLosingTradeTime;
                    DashboardData.MaxDrawdown = accountData.PerformanceData.LosingTrades.MaxDrawdown;

                    // **Update Trades**
                    DashboardData.Trades.Clear(); // Clear existing trades before adding new ones
                    foreach (var trade in accountData.PerformanceData?.Trades ?? new List<Trade>())
                    {
                        DashboardData.Trades.Add(trade);
                    }

                    // Notify UI of property changes
                    OnPropertyChanged(nameof(DashboardData));
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error updating UI: " + ex.Message);
            }
        }



        // INotifyPropertyChanged implementation for UI binding updates
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}