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
        public static string FindSteamPath()
        {
            Process[] steamCandidateProcesses = Process.GetProcessesByName("steam");

            if (steamCandidateProcesses.Length > 0)
            {
                return steamCandidateProcesses[0].MainModule.FileName.Replace(
                    steamCandidateProcesses[0].MainModule.ModuleName,
                    ""
                );
            }
            else
            {
                throw new InvalidOperationException("Steam is not running.");
            }
        }

        public static List<string> GetGameOverlayLogLines (string steamPath)
        {
            var gameOverlayLog = Path.Combine(steamPath, "GameOverlayUI.exe.log");

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

            return lines;
        }

        public static long GetActiveGameAppId()
        {
            Process[] overlayCandidateProcesses = Process.GetProcessesByName("gameoverlayui");

            if (overlayCandidateProcesses.Length > 0)
            {
                Process overlay = overlayCandidateProcesses[0];
                string launchCommand = overlay.GetCommandLine();

                var indexStart = launchCommand.IndexOf("-gameid") + 8;
                var length = launchCommand.IndexOf(" ", indexStart) - indexStart;
                var appId = launchCommand.Substring(indexStart, length);
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
