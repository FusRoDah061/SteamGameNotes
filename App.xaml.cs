using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using NHotkey.Wpf;
using System.Windows.Input;
using NHotkey;
using SteamGameNotes.Service;
using System.Threading.Tasks;

namespace SteamGameNotes
{
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon;
        private SteamService _steamService = new SteamService();

        public App()
        {
            HotkeyManager.Current.AddOrReplace("ShowWindowShiftTab", Key.Tab, ModifierKeys.Shift, true, OnToggleWindow);
        }

        private void OnToggleWindow(object sender, HotkeyEventArgs e)
        {
            if (Current.Windows.Count == 0)
            {
                Current.MainWindow = new MainWindow();
                Current.MainWindow.Show();
            }
            else
            {
                for (int i = 0; i < Current.Windows.Count; i++)
                    Current.Windows[i].Close();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _notifyIcon = (TaskbarIcon) FindResource("TrayIcon");

            Task.Run(async () => {
                await _steamService.CreateSteamAppsCache();
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();
            _steamService.InvalidateCache();
            base.OnExit(e);
        }
    }
}
