using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using NHotkey.Wpf;
using System.Windows.Input;
using NHotkey;
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

        public App()
        {
            HotkeyManager.Current.AddOrReplace("ShowWindowShiftTab", Key.Tab, ModifierKeys.Shift, false, OnToggleWindow);
            HotkeyManager.Current.AddOrReplace("HideWindowEsc", Key.Escape, ModifierKeys.None, false, OnHideWindow);
        }

        private bool _isOverlayOpen()
        { 
            var logLocked = false;
            var logReadTries = 0;

            log.Debug("Checking if steam overlay is open");

            do
            {                
                if (logReadTries > 50)
                    break;

                logLocked = false;

                log.Debug("Sleeping 200ms before reading overlay log");
                Thread.Sleep(200);

                try {
                    var logLines = SteamHelper.GetGameOverlayLogLines(_steamFolder);
                    var lastLine = logLines[logLines.Count - 1];

                    log.Debug("Overlay log last line: " + lastLine);

                    return lastLine.ToLower().Contains("overlay enable");
                }
                catch (IOException ex)
                {
                    log.Warn("Error reading steam overlay log: ", ex);
                    log.Debug($"Tried reading overlay log file {logReadTries} times.");

                    logLocked = true;
                    logReadTries++;
                }
            } while (logLocked);

            return false;
        }

        private void OnToggleWindow(object sender, HotkeyEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_steamFolder))
                {
                    log.Info("Steam folder is undefined");
                    _steamFolder = SteamHelper.FindSteamPath();
                }

                if (_isOverlayOpen())
                {
                    log.Info("Showing notes");
                    log.Debug("Current window count: " + Current.Windows.Count);

                    if (Current.Windows.Count == 0)
                    {
                        Current.MainWindow = new MainWindow();
                        Current.MainWindow.Show();
                    }
                }
                else
                {
                    log.Info("Hiding notes");
                    log.Debug("Current window count: " + Current.Windows.Count);

                    if (Current.Windows.Count > 0)
                    {
                        for (int i = 0; i < Current.Windows.Count; i++)
                            Current.Windows[i].Close();
                    }
                }
            }
            catch(InvalidOperationException ex)
            {
                log.Error("Error: ", ex);
                MessageBox.Show(ex.Message, "Steam Game Notes Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnHideWindow(object sender, HotkeyEventArgs e)
        {
            if (!_isOverlayOpen())
            {
                log.Info("Hiding notes");

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
