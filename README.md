# V1 - Trading Algorithm Monitoring Dashboard

V1 is a WPF-based desktop application designed for monitoring algorithmic trading activity on prop firms. It provides real-time insights into account performance, and trade history

## Features

### ✅ Current Features:
- **Account Overview**: Displays account name, total profit/loss, and equity in an easy-to-read format.
- **Performance Summary**: Detailed trade statistics, including win/loss metrics, expectancy, and drawdowns.
- **Live Trade Table**: Shows executed trades with buy/sell prices, durations, and profit/loss in a structured grid.
- **Data Scraping**: Uses WebView2 to scrape and update account data.
- **Modern UI**: Built with WPF for a clean, intuitive user experience.
- **Multi-Account Support**: Introduce a config file to allow switching between different accounts.
- **Trading View Integration**: Executes trades directly inside of tradingview
- **Optimized Trade Execution**: Improve order execution speeds by bypassing third-party APIs.

### 🚀 Planned Features:
- **TradingView Chart Integration**: Display charts for better visual analysis.
- **Performance Analyzer**: Use execution history to analyze strategy

## Installation
- **Install and Build on your own**
- **Place config.json in Data/ for account information and login**

### Prerequisites:
- .NET 6+ installed on your machine.
- WebView2 Runtime (for web scraping functionality).