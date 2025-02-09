using System;
using System.Windows;
using System.Windows.Interop;

namespace V1.Scripts
{
    public class HiddenWindowHost
    {
        public Window HiddenWindow { get; private set; }
        public nint WindowHandle { get; private set; }

        /// <summary>
        /// Constructs the hidden window host.
        /// When debugMode is true, the window will be visible for inspection.
        /// </summary>
        public HiddenWindowHost(bool debugMode = false)
        {
            CreateHiddenWindow(debugMode);
        }

        private void CreateHiddenWindow(bool debugMode)
        {
            // If in debug mode, create a standard window that is visible.
            // Otherwise, create a minimal window off-screen.
            if (debugMode)
            {
                HiddenWindow = new Window
                {
                    Title = "Debug WebView2 Host",
                    Width = 800,
                    Height = 600,
                    Left = -10000,
                    Top = -10000,
                    ShowInTaskbar = false,
                    WindowStyle = WindowStyle.None
                };
            }
            else
            {
                HiddenWindow = new Window
                {
                    Width = 1,
                    Height = 1,
                    Left = -10000,
                    Top = -10000,
                    ShowInTaskbar = false,
                    WindowStyle = WindowStyle.None
                };
            }

            HiddenWindow.Show();
            WindowHandle = new WindowInteropHelper(HiddenWindow).Handle;
        }

        public void Close()
        {
            if (HiddenWindow != null)
            {
                HiddenWindow.Close();
                HiddenWindow = null;
            }
        }
    }
}
