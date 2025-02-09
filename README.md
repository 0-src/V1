# V1 - Algorithmic Trading Monitor

![V1 App](https://github.com/0-src/V1/blob/master/Data/Dashboard.png) <!-- Replace with an actual image URL if available -->

**V1** is a **WPF-based desktop application** designed for executing trades based off of TradingView alerts without the need for webhooks to allow for faster execution into proprietary Tradovate accounts.

---

## 🔹 Features

### 📊 Account Overview
- Displays account names, **total profit/loss**, and **equity** in a structured format.

### 📈 Performance Summary
- Offers detailed trade statistics, including:
  - ✅ Win/Loss ratio
  - 📉 Maximum drawdowns
  - 📊 Trade expectancy
  - 🔄 Total trades executed

### 💹 Live Trade Table
- Presents real-time trade executions with:
  - 🛒 Buy/Sell prices
  - ⏳ Trade duration
  - 💰 Profit/Loss tracking

### 🔎 Data Scraping with WebView2
- Uses **WebView2** for scraping and updating **account data** from web-based sources.
- Automates login handling with **loginScript.js**.
- Extracts account/trade data via **DataScraper.js**.
- **HiddenWindowHost.cs** manages WebView2 instances in the background.

### 🤖 Algorithmic Execution & Logging
- **AlgoExecutions.xaml** tracks automated trade execution.
- Stores trade logs for historical review and analysis.

### 📊 Advanced Dashboard Analytics
- **DashboardPage.xaml** aggregates trade statistics into a comprehensive view.
- Provides insights on **profitability trends** and **risk metrics**.

### 🎨 Modern WPF UI
- Built with **Windows Presentation Foundation (WPF)** for a clean, modern, and user-friendly interface.

### 🔔 Notifications
- Windows Push Notifications for executed trades.
- Email alerts for significant trade events.

### 🔄 Auto Refresh Data
- Automatically refreshes dashboard data at set intervals.

### ⚙️ Editing Account Configuration
- The application stores account credentials in a **configuration file (`config.json`)**.
- **Location:**
  ```plaintext
  C:\Users\YourUsername\AppData\Roaming\V1_TradingApp\config.json
  ```
- **Example Format:**
  ```json
  {
      "accounts": [
          {
              "Name": "Primary Account",
              "Username": "your_username",
              "Password": "your_password"
          }
      ]
  }
  ```
- To update credentials:
  1. Open `config.json` in **Notepad** or a JSON editor like **VS Code**.
  2. Modify the `Username` and `Password` fields.
  3. Save the file and **restart the application**.
- New accounts can be added by inserting additional objects inside the `accounts` list.

---

## 📂 Project Structure

```
📁 V1/
│── 📁 Data/             # Contains trade/account data files
│── 📁 Pages/            # XAML pages defining UI components
│    ├── AlgoExecutions.xaml       # Algorithmic trade execution tracking
│    ├── DashboardPage.xaml        # Main dashboard with analytics
│── 📁 Scripts/          # Application scripts
│    ├── DataScraper.js          # WebView2 data scraping logic
│    ├── loginScript.js          # Automates Tradovate login
│    ├── HiddenWindowHost.cs     # Manages background WebView2 instances
│    ├── Classes/
│    │   ├── AccountData.cs      # Data model for account information
│    │   ├── DashboardData.cs    # Data model for dashboard metrics
│── 📜 App.xaml          # Main app resource file
│── 📜 App.xaml.cs       # Application startup logic
│── 📜 MainWindow.xaml   # Main window layout
│── 📜 MainWindow.xaml.cs # Main window backend logic
│── 📜 splashScreen.xaml # Splash screen layout
│── 📜 splashScreen.xaml.cs # Splash screen behavior
│── 📜 V1.csproj         # C# project file
│── 📜 V1.sln            # Solution file for building the app
```

---

## 🛠️ Technologies Used

- **C#** - Core programming language
- **WPF (Windows Presentation Foundation)** - UI framework
- **WebView2** - Web content embedding & data scraping
- **.NET** - Application framework

---

## 🏗️ Installation & Usage

### 📥 Prerequisites
- **Windows 10 or later**
- **.NET 6+ installed**
- **WebView2 Runtime** (Required for scraping feature)

### 🚀 Running the Application

1. **Clone the repository**:
   ```sh
   git clone https://github.com/0-src/V1.git
   cd V1
   ```

2. **Open in Visual Studio**
   - Open `V1.sln` in **Visual Studio 2022+**.

3. **Build & Run**
   - Click on **Start (▶)** or run:
     ```sh
     dotnet build
     dotnet run
     ```

---

## 📦 Downloading Releases
- Official releases are available under the **Releases** section of the GitHub repository.
- To download the latest version:
  1. Go to **[Releases](https://github.com/0-src/V1/releases)**.
  2. Download the latest `.zip` or `.exe` file.
  3. Extract the `.zip` file and run `V1.exe`.
- Releases may include bug fixes, new features, and improvements.

---

## 🔍 How It Works

1. **TradingView alerts trigger trade execution**.
2. **WebView2 opens a Tradovate trading window** instead of relying on an API.
3. **Orders are executed instantly**, bypassing webhook delays.
4. **The UI updates in real-time** to reflect trade status and account metrics.
5. **Trade execution logs are stored and displayed**, providing insights into past trades.
6. **Users can filter and search trade logs** to review performance.
7. **Order validation checks** ensure incorrect trades are minimized.

---

## 📌 Roadmap & Future Enhancements

- ✅ **Current:** Trade execution through WebView2
- 🔜 **Planned:**
  - Improved **UI design**
  - Advanced **risk metrics**

---

## 📜 License

This project is licensed under the **MIT License**. See [LICENSE](LICENSE) for details.

---

## 🤝 Contributing

Contributions are welcome! If you'd like to contribute:
1. **Fork the repo** 🍴
2. **Create a new branch**: `git checkout -b feature-name`
3. **Commit your changes**: `git commit -m "Add new feature"`
4. **Push the branch**: `git push origin feature-name`
5. **Open a Pull Request** ✅

---

### ⭐ If you find this project useful, please give it a star! ⭐

