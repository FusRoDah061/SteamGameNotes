using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using System.Windows.Input;
using SteamGameNotes.Service;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Threading;
using SteamGameNotes.Helper;
using log4net;

namespace SteamGameNotes
{
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        private TaskbarIcon _notifyIcon;
        private SteamService _steamService = new SteamService();
        private string _steamFolder;
        private bool _isOverlayOpen = false;

        public App()
        {
            SimpleHotkeyManager.AddHotkey(Key.LeftShift, Key.Tab, OnToggleWindow);
            SimpleHotkeyManager.AddHotkey(Key.Escape, OnHideWindow);
        }

        private bool _checkIfOverlayOpen()
        { 
            var retry = false;
            var retryCount = 0;

            log.Debug("Checking if steam overlay is open");

            string logLastLine = null;

            do
            {                
                retry = false;

                log.Debug("Sleeping 200ms before reading overlay log");
                Thread.Sleep(200);

                try {
                    var logLines = SteamHelper.GetGameOverlayLogLines(_steamFolder);
                    logLastLine = logLines[logLines.Count - 1];

                    log.Debug("Overlay log last line: " + logLastLine);

                    if (logLastLine.ToLower().Contains("overlay enable"))
                    {
                        return true;
                    }

                    retry = true;
                    retryCount++;
                }
                catch (IOException ex)
                {
                    log.Warn("Error reading steam overlay log: ", ex);
                    log.Debug($"Tried reading overlay log file {retryCount} times.");

                    retry = true;
                    retryCount++;
                }
            } while (retry && retryCount < 20);

            return false;
        }

        private void OnToggleWindow(object sender, EventArgs e)
        {
            log.Info("Toggling window");

            try
            {
                if (String.IsNullOrEmpty(_steamFolder))
                {
                    log.Info("Steam folder is undefined");
                    _steamFolder = SteamHelper.FindSteamPath();
                }

                if (_isOverlayOpen)
                {
                    log.Info("Overlay not open. Hiding notes");
                    log.Debug("Current window count: " + Current.Windows.Count);

                    _isOverlayOpen = false;

                    if (Current.Windows.Count > 0)
                    {
                        for (int i = 0; i < Current.Windows.Count; i++)
                            Current.Windows[i].Close();
                    }
                }
                else if (_checkIfOverlayOpen())
                {
                    log.Info("Overlay open. Showing notes");
                    log.Debug("Current window count: " + Current.Windows.Count);

                    _isOverlayOpen = true;

                    if (Current.Windows.Count == 0)
                    {
                        Current.MainWindow = new MainWindow();
                        Current.MainWindow.Show();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                log.Error("Error: ", ex);
                MessageBox.Show(ex.Message, "Steam Game Notes Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnHideWindow(object sender, EventArgs e)
        {
            log.Info("Hiding window");

            if (_isOverlayOpen)
            {
                log.Info("Hiding notes");
                log.Debug("Current window count: " + Current.Windows.Count);

                _isOverlayOpen = false;

                if (Current.Windows.Count > 0)
                {
                    for (int i = 0; i < Current.Windows.Count; i++)
                        Current.Windows[i].Close();
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _notifyIcon = (TaskbarIcon) FindResource("TrayIcon");

            try
            {
                _steamFolder = SteamHelper.FindSteamPath();
            }
            catch (InvalidOperationException ex)
            {
                log.Error("Error: ", ex);
                MessageBox.Show(ex.Message, "Steam Game Notes Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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
