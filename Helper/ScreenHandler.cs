using log4net;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace SteamGameNotes.Helper
{
    public class ScreenHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ScreenHandler));

        public static Screen GetCurrentScreen(Window window)
        {
            var parentArea = new Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height);
            return Screen.FromRectangle(parentArea);
        }

        public static Screen GetMainScreen()
        {
            var screens = Screen.AllScreens;
            var mainScreen = screens[0];

            foreach (var screen in screens) {
                if (screen.Primary) mainScreen = screen;
            }

            log.Debug("Main screen: " + mainScreen.DeviceName);

            return mainScreen;
    }
    }
}
