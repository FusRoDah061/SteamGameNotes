using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SteamGameNotes
{
    public class BaseWindow : Window
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        protected void RequestDeactivateWindow()
        {
            WindowInteropHelper windowHelper = new WindowInteropHelper(this);
            SetWindowLong(windowHelper.Handle, GWL_EXSTYLE, GetWindowLong(windowHelper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }

        protected void RequestActivateWindow()
        {
            WindowInteropHelper windowHelper = new WindowInteropHelper(this);
            SetForegroundWindow(windowHelper.Handle);
        }
    }
}
