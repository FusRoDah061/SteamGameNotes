using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using NHotkey.Wpf;
using System.Windows.Input;
using NHotkey;

namespace SteamGameNotes
{
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        public App()
        {
            HotkeyManager.Current.AddOrReplace("ShowWindowShiftTab", Key.Tab, ModifierKeys.Shift, OnToggleWindow);
        }

        private void OnToggleWindow(object sender, HotkeyEventArgs e)
        {
            if (Current.MainWindow == null)
            {
                Current.MainWindow = new MainWindow();
                Current.MainWindow.Show();
            }
            else
            {
                Current.MainWindow.Close();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            notifyIcon = (TaskbarIcon) FindResource("TrayIcon");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose();
            base.OnExit(e);
        }
    }
}
