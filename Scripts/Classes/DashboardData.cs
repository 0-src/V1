using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using V1.Scripts.Classes;

namespace V1
{
    public class DashboardData : INotifyPropertyChanged
    {
        private string _accountName;
        private decimal _totalPL;
        private decimal _equity;
        private decimal _totalProfit;
        private int _numberOfWinningTrades;
        private int _numberOfWinningContracts;
        private decimal _largestWinningTrade;
        private decimal _averageWinningTrade;
        private decimal _stdDevWinningTrade;
        private string _averageWinningTradeTime;
        private string _longestWinningTradeTime;
        private decimal _maxRunUp;

        private decimal _totalLoss;
        private int _numberOfLosingTrades;
        private int _numberOfLosingContracts;
        private decimal _largestLosingTrade;
        private decimal _averageLosingTrade;
        private decimal _stdDevLosingTrade;
        private string _averageLosingTradeTime;
        private string _longestLosingTradeTime;
        private decimal _maxDrawdown;

        public string AccountName
        {
            get => _accountName;
            set { _accountName = value; OnPropertyChanged(nameof(AccountName)); }
        }

        public decimal TotalPL
        {
            get => _totalPL;
            set
            {
                _totalPL = value;
                OnPropertyChanged(nameof(TotalPL));
                OnPropertyChanged(nameof(TotalPLColor));
            }
        }

        public decimal Equity
        {
            get => _equity;
            set
            {
                _equity = value;
                OnPropertyChanged(nameof(Equity));
                OnPropertyChanged(nameof(EquityColor));
            }
        }

        // **Color Properties (Auto-Updated)**
        public string TotalPLColor => TotalPL >= 0 ? "Green" : "Red";
        public string EquityColor => Equity >= 0 ? "Green" : "Red";

        // **Profit Trades**
        public decimal TotalProfit
        {
            get => _totalProfit;
            set { _totalProfit = value; OnPropertyChanged(nameof(TotalProfit)); }
        }

        public int NumberOfWinningTrades
        {
            get => _numberOfWinningTrades;
            set { _numberOfWinningTrades = value; OnPropertyChanged(nameof(NumberOfWinningTrades)); }
        }

        public int NumberOfWinningContracts
        {
            get => _numberOfWinningContracts;
            set { _numberOfWinningContracts = value; OnPropertyChanged(nameof(NumberOfWinningContracts)); }
        }

        public decimal LargestWinningTrade
        {
            get => _largestWinningTrade;
            set { _largestWinningTrade = value; OnPropertyChanged(nameof(LargestWinningTrade)); }
        }

        public decimal AverageWinningTrade
        {
            get => _averageWinningTrade;
            set { _averageWinningTrade = value; OnPropertyChanged(nameof(AverageWinningTrade)); }
        }

        public decimal StdDevWinningTrade
        {
            get => _stdDevWinningTrade;
            set { _stdDevWinningTrade = value; OnPropertyChanged(nameof(StdDevWinningTrade)); }
        }

        public string AverageWinningTradeTime
        {
            get => _averageWinningTradeTime;
            set { _averageWinningTradeTime = value; OnPropertyChanged(nameof(AverageWinningTradeTime)); }
        }

        public string LongestWinningTradeTime
        {
            get => _longestWinningTradeTime;
            set { _longestWinningTradeTime = value; OnPropertyChanged(nameof(LongestWinningTradeTime)); }
        }

        public decimal MaxRunUp
        {
            get => _maxRunUp;
            set { _maxRunUp = value; OnPropertyChanged(nameof(MaxRunUp)); }
        }

        // **Losing Trades**
        public decimal TotalLoss
        {
            get => _totalLoss;
            set { _totalLoss = value; OnPropertyChanged(nameof(TotalLoss)); }
        }

        public int NumberOfLosingTrades
        {
            get => _numberOfLosingTrades;
            set { _numberOfLosingTrades = value; OnPropertyChanged(nameof(NumberOfLosingTrades)); }
        }

        public int NumberOfLosingContracts
        {
            get => _numberOfLosingContracts;
            set { _numberOfLosingContracts = value; OnPropertyChanged(nameof(NumberOfLosingContracts)); }
        }

        public decimal LargestLosingTrade
        {
            get => _largestLosingTrade;
            set { _largestLosingTrade = value; OnPropertyChanged(nameof(LargestLosingTrade)); }
        }

        public decimal AverageLosingTrade
        {
            get => _averageLosingTrade;
            set { _averageLosingTrade = value; OnPropertyChanged(nameof(AverageLosingTrade)); }
        }

        public decimal StdDevLosingTrade
        {
            get => _stdDevLosingTrade;
            set { _stdDevLosingTrade = value; OnPropertyChanged(nameof(StdDevLosingTrade)); }
        }

        public string AverageLosingTradeTime
        {
            get => _averageLosingTradeTime;
            set { _averageLosingTradeTime = value; OnPropertyChanged(nameof(AverageLosingTradeTime)); }
        }

        public string LongestLosingTradeTime
        {
            get => _longestLosingTradeTime;
            set { _longestLosingTradeTime = value; OnPropertyChanged(nameof(LongestLosingTradeTime)); }
        }

        public decimal MaxDrawdown
        {
            get => _maxDrawdown;
            set { _maxDrawdown = value; OnPropertyChanged(nameof(MaxDrawdown)); }
        }

        // **Trades Collection**
        public ObservableCollection<Trade> Trades { get; set; } = new ObservableCollection<Trade>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
