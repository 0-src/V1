# V1 - Trading Algorithm Monitoring Dashboard

V1 is a WPF-based desktop application designed to execute trades based on tradingview notifications to cut down on execution times.
## Features

- **Account Overview**: Displays account name, total profit/loss, and equity in an easy-to-read format.
- **Performance Summary**: Offers detailed trade statistics, including win/loss metrics, expectancy, and drawdowns.
- **Live Trade Table**: Shows executed trades with buy/sell prices, durations, and profit/loss in a structured grid.
- **Data Scraping**: Utilizes WebView2 to scrape and update account data.
- **Modern UI**: Built with WPF for a clean, intuitive user experience.

## Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/0-src/V1.git
   cd V1
   ```
2. **Open the Solution**:
   - Launch `V1.sln` using Visual Studio 2022 or later.
3. **Restore NuGet Packages**:
   - Navigate to the Solution Explorer.
   - Right-click on the solution and select "Restore NuGet Packages."
4. **Build the Solution**:
   - Ensure the build configuration is set to `Release`.
   - Press `Ctrl + Shift + B` to build the solution.
5. **Run the Application**:
   - Press `F5` to start the application.

## Usage

Upon launching V1:

- **Account Overview**: View your account's total profit/loss and equity.
- **Performance Summary**: Analyze detailed trade statistics to assess your trading strategy's effectiveness.
- **Live Trade Table**: Monitor executed trades in real-time, including entry/exit prices and trade durations.

## Dependencies

- **.NET Framework 4.8**: Ensure that .NET Framework 4.8 is installed on your system.
- **WebView2 Runtime**: The application uses WebView2 for data scraping. Download and install the WebView2 runtime from the [official Microsoft website](https://developer.microsoft.com/en-us/microsoft-edge/webview2/).

## Contributing

We welcome contributions to enhance V1:

1. **Fork the Repository**: Click on the "Fork" button at the top right of the repository page.
2. **Create a New Branch**: Use a descriptive name for your branch.
3. **Make Changes**: Implement your features or bug fixes.
4. **Commit Changes**: Write clear and concise commit messages.
5. **Push to Your Fork**: Upload your changes to your forked repository.
6. **Submit a Pull Request**: Navigate to the original repository and open a pull request with a detailed description of your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Contact

For questions or support, please open an issue in this repository.
