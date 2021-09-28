using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace SteamGameNotes.Helper
{
    public class SteamHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SteamHelper));

        public static string FindSteamPath()
        {
            log.Info("Getting steam folder");

            Process[] steamCandidateProcesses = Process.GetProcessesByName("steam");

            if (steamCandidateProcesses.Length > 0)
            {
                Process steamProcess = steamCandidateProcesses[0];

                log.Debug("Steam PID: " + steamProcess.Id);

                string steamFolder = steamProcess.MainModule.FileName.Replace(
                    steamProcess.MainModule.ModuleName,
                    ""
                );

                log.Info("Steam folder: " + steamFolder);

                return steamFolder;
            }
            else
            {
                throw new InvalidOperationException("Steam is not running.");
            }
        }

        public static List<string> GetGameOverlayLogLines (string steamPath)
        {
            var gameOverlayLog = Path.Combine(steamPath, "GameOverlayUI.exe.log");

            log.Info("Reading overlay log at " + gameOverlayLog);

            Stream stream = File.Open(gameOverlayLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader streamReader = new StreamReader(stream);

            List<string> lines = new List<string>();
            string line = null;

            do
            {
                line = streamReader.ReadLine();

                if (line != null)
                    lines.Add(line);
            } while (line != null);

            log.Debug($"Read {lines.Count} lines from overlay log");

            return lines;
        }

        public static long GetActiveGameAppId()
        {
            log.Info("Getting running game appId");

            Process[] overlayCandidateProcesses = Process.GetProcessesByName("gameoverlayui");

            if (overlayCandidateProcesses.Length > 0)
            {
                Process overlay = overlayCandidateProcesses[0];
                string launchCommand = overlay.GetCommandLine();

                log.Debug("Overlay launch command: " + launchCommand);

                var indexStart = launchCommand.IndexOf("-gameid") + 8;
                var length = launchCommand.IndexOf(" ", indexStart) - indexStart;
                var appId = launchCommand.Substring(indexStart, length);

                log.Debug("Found appId: " + appId);

                return long.Parse(appId);
            }
            else
            {
                throw new InvalidOperationException("Steam Overlay is not available. There's likely no game running.");
            }
        }   
    }

    public static class SteamHelperExtensions
    {
        public static string GetCommandLine(this Process process)
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
            using (ManagementObjectCollection objects = searcher.Get())
            {
                return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
            }

        }
    }
}
