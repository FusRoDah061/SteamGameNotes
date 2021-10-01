using SteamGameNotes.Helper;
using System.Windows;
using System.Windows.Input;

namespace SteamGameNotes
{
    public class TrayIconViewModel
    {
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        var mainScreen = ScreenHandler.GetMainScreen();
                        var mainWindow = new MainWindow();
                        mainWindow.Left = (mainScreen.WorkingArea.Right / 2) - (mainWindow.Width / 2);
                        mainWindow.Top = (mainScreen.WorkingArea.Bottom / 2) - (mainWindow.Height / 2);
                        Application.Current.MainWindow = mainWindow;
                        Application.Current.MainWindow.Show();
                    }
                };
            }
        }

        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }
    }
}
