using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using NHotkey.Wpf;
using System.Windows.Input;
using NHotkey;
using SteamGameNotes.Service;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace SteamGameNotes
{
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon;
        private SteamService _steamService = new SteamService();
        private string _steamFolder;

        public App()
        {
            HotkeyManager.Current.AddOrReplace("ShowWindowShiftTab", Key.Tab, ModifierKeys.Shift, false, OnToggleWindow);
            HotkeyManager.Current.AddOrReplace("HideWindowEsc", Key.Escape, ModifierKeys.None, false, OnHideWindow);
        }

        private void _findSteamPath()
        {
            Process[] steamCandidateProcesses = Process.GetProcessesByName("steam");

            if(steamCandidateProcesses.Length > 0)
            {
                _steamFolder = steamCandidateProcesses[0].MainModule.FileName.Replace(
                    steamCandidateProcesses[0].MainModule.ModuleName,
                    ""
                );
            }
            else
            {
                throw new InvalidOperationException("Steam is not running.");
            }
        }

        private List<string> _getLogFileLines (string filePath)
        {
            Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader streamReader = new StreamReader(stream);

            List<string> lines = new List<string>();
            string line = null;

            do
            {
                line = streamReader.ReadLine();

                if (line != null)
                    lines.Add(line);
            } while (line != null);

            return lines;
        }

        private bool _isOverlayOpen()
        {
            var gameOverlayLog = Path.Combine(_steamFolder, "GameOverlayUI.exe.log");
            var logLocked = false;
            var logReadTries = 0;

            do
            {
                if (logReadTries > 50)
                    break;

                logLocked = false;

                Thread.Sleep(200);

                try {
                    var logLines = _getLogFileLines(gameOverlayLog);
                    var lastLine = logLines[logLines.Count - 1];

                    return lastLine.ToLower().Contains("overlay enable");
                }
                catch (IOException ex)
                {
                    logLocked = true;
                    logReadTries++;
                    Thread.Sleep(100);
                }
            } while (logLocked);

            return false;
        }

        private void OnToggleWindow(object sender, HotkeyEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_steamFolder))
                    _findSteamPath();

                if (_isOverlayOpen())
                {
                    if (Current.Windows.Count == 0)
                    {
                        Current.MainWindow = new MainWindow();
                        Current.MainWindow.Show();
                    }
                }
                else
                {
                    if (Current.Windows.Count > 0)
                    {
                        for (int i = 0; i < Current.Windows.Count; i++)
                            Current.Windows[i].Close();
                    }
                }
            }
            catch(InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnHideWindow(object sender, HotkeyEventArgs e)
        {
            if (!_isOverlayOpen())
            {
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
                _findSteamPath();
            }
            catch (InvalidOperationException ex)
            {
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
