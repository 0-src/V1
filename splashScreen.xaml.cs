using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace V1
{
    public partial class splashScreen : Window
    {
        public string SelectedUsername { get; private set; }
        public string SelectedPassword { get; private set; }
        public bool IsLoginConfirmed { get; private set; } = false;

        public event Action LoginConfirmed;

        private class Account
        {
            public string Name { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private List<Account> accounts = new();
        private string configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "V1_TradingApp",
            "config.json"
        );

        public splashScreen()
        {
            InitializeComponent();
            LoadAccounts();

            Loaded += (s, e) =>
            {
                var fadeIn = new DoubleAnimation { From = 0, To = 1, Duration = TimeSpan.FromSeconds(.6) };
                BeginAnimation(OpacityProperty, fadeIn);
            };
        }

        private void LoadAccounts()
        {
            try
            {
                // ✅ Ensure the config directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(configPath));

                if (!File.Exists(configPath))
                {
                    File.WriteAllText(configPath, "[]"); // ✅ Create an empty config file if missing
                }

                string json = File.ReadAllText(configPath);
                accounts = JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();

                accountDropdown.ItemsSource = accounts;
                accountDropdown.DisplayMemberPath = "Name";

                if (accounts.Count > 0)
                {
                    accountDropdown.SelectedIndex = 0; // ✅ Default to first account
                    ShowSection(loginSection, addAccountSection);
                }
                else
                {
                    ShowSection(addAccountSection, loginSection);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load accounts: " + ex.Message);
            }
        }

        public void UpdateStatus(string message)
        {
            txtStatus.Text = message;
        }

        private void accountDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (accountDropdown.SelectedItem is Account selectedAccount)
            {
                SelectedUsername = selectedAccount.Username;
                SelectedPassword = selectedAccount.Password;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedUsername) || string.IsNullOrEmpty(SelectedPassword))
            {
                MessageBox.Show("Please select an account before logging in.");
                return;
            }

            IsLoginConfirmed = true;
            LoginConfirmed?.Invoke();

            FadeOutElement(accountDropdown);
            FadeOutElement(sender as Button);
            FadeOutElement(AddAccountButton);
        }
        private void ShowSection(UIElement sectionToShow, UIElement sectionToHide)
        {
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
            fadeOut.Completed += (s, e) =>
            {
                sectionToHide.Visibility = Visibility.Collapsed;
                sectionToShow.Visibility = Visibility.Visible;

                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
                sectionToShow.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            sectionToHide.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        // Example usage:
        private void ShowAddAccountSection(object sender, RoutedEventArgs e)
        {
            ShowSection(addAccountSection, loginSection);
        }

        private void txtAccountName_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtAccountNamePlaceholder.Visibility = string.IsNullOrWhiteSpace(txtAccountName.Text)
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtUserNamePlaceholder.Visibility = string.IsNullOrWhiteSpace(txtUserName.Text)
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtPasswordPlaceholder.Visibility = string.IsNullOrWhiteSpace(txtPassword.Password)
                ? Visibility.Visible : Visibility.Collapsed;
        }


        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            string name = txtAccountName.Text.Trim();
            string username = txtUserName.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            accounts.Add(new Account { Name = name, Username = username, Password = password });

            // ✅ Save the updated accounts list
            File.WriteAllText(configPath, JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true }));

            MessageBox.Show("✅ Account Added Successfully!");

            // Refresh dropdown with new accounts
            LoadAccounts();
        }


        private void FadeOutElement(UIElement element)
        {
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(0.1)),
                EasingFunction = new QuadraticEase()
            };

            fadeOut.Completed += (s, e) => element.Visibility = Visibility.Collapsed;
            element.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }
    }
}
