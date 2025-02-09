# V1 - Algorithmic Trading Monitor

![V1 App](https://github.com/0-src/V1/blob/main/assets/screenshot.png) <!-- Replace with an actual image URL if available -->

**V1** is a **WPF-based desktop application** designed for executing trades based off of tradingview alerts without the need for webhooks to allow for faster execution into proprietary tradovate accounts

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

### 🎨 Modern WPF UI
- Built with **Windows Presentation Foundation (WPF)** for a clean, modern, and user-friendly interface.

## 🔹 Planned Features

### Notifications
 - Add Windows Push Notifications
 - Add Email Notifications for Executions
 
### Auto Refresh Data
 - Automatically Refresh the Dashboard Data 

### Dashboard Improvements
 - Add graphs for profit/loses
 - Add more analytical tools


---

## 📂 Project Structure

```
📁 V1/
│── 📁 Data/             # Contains trade/account data files
│── 📁 Pages/            # XAML pages defining UI components
│── 📁 Scripts/          # Application scripts
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

## 📜 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

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
