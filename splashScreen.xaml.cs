using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace V1
{
    public partial class splashScreen : Window
    {
        public string SelectedUsername { get; private set; }
        public string SelectedPassword { get; private set; }
        public bool IsLoginConfirmed { get; private set; } = false; // Tracks if the user clicked login

        public event Action LoginConfirmed;

        private class Account
        {
            public string Name { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private List<Account> accounts = new();

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

        public void UpdateStatus(string message)
        {
            txtStatus.Text = message;
        }

        public void CloseSplash()
        {
            Dispatcher.InvokeAsync(() =>
            {
                var fadeOut = new DoubleAnimation { From = 1, To = 0, Duration = TimeSpan.FromSeconds(0.3) };
                fadeOut.Completed += (s, e) => Close();
                BeginAnimation(OpacityProperty, fadeOut);
            });
        }

        private void LoadAccounts()
        {
            try
            {
                string json = GetEmbeddedResource("V1.Data.config.json"); // Load config.json like a script
                if (!string.IsNullOrEmpty(json))
                {
                    accounts = JsonSerializer.Deserialize<List<Account>>(json);
                    accountDropdown.ItemsSource = accounts;
                    accountDropdown.DisplayMemberPath = "Name";
                }
                else
                {
                    MessageBox.Show("Failed to load accounts: Empty config.json");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load accounts: " + ex.Message);
            }
        }

        private void accountDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (accountDropdown.SelectedItem is Account selectedAccount)
            {
                SelectedUsername = selectedAccount.Username;
                SelectedPassword = selectedAccount.Password;
            }
        }

        private void FadeOutElement(UIElement element)
        {
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(0.1)), // Half-second fade
                EasingFunction = new QuadraticEase() // Smooth transition
            };

            fadeOut.Completed += (s, e) => element.Visibility = Visibility.Collapsed; // Hide after animation

            element.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedUsername) || string.IsNullOrEmpty(SelectedPassword))
            {
                MessageBox.Show("Please select an account before logging in.");
                return;
            }

            IsLoginConfirmed = true; // Set flag that login was confirmed
            LoginConfirmed?.Invoke();

            // Smoothly fade out the button and dropdown before hiding them
            FadeOutElement(accountDropdown);
            FadeOutElement(sender as Button);

            // Create a ColorAnimation for smooth transition
            ColorAnimation colorAnimation = new ColorAnimation
            {
                From = ((SolidColorBrush)icon.Fill).Color, // Get current color
                To = (Color)ColorConverter.ConvertFromString("#3B82F6"), // Target color
                Duration = new Duration(TimeSpan.FromSeconds(1)), // Animation duration
                EasingFunction = new QuadraticEase() // Adds smooth easing
            };

            // Apply animation to the Ellipse Fill
            SolidColorBrush animatedBrush = new SolidColorBrush(((SolidColorBrush)icon.Fill).Color);
            icon.Fill = animatedBrush; // Ensure the Fill can be animated
            animatedBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }

        private string GetEmbeddedResource(string resourceName)
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
    }
}
