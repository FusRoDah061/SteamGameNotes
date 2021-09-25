using SteamGameNotes.Service;
using System;
using System.Windows;
using System.Windows.Media;

namespace SteamGameNotes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Brush _txtForeground = null;

        public MainWindow()
        {
            InitializeComponent();
            _txtForeground = TxtSearchGame.Foreground;

            TxtSearchGame.Foreground = (SolidColorBrush)FindResource("ColorTypo");
            TxtSearchGame.Text = TxtSearchGame.Tag.ToString();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var steamService = new SteamService();

            var game = await steamService.GetSteamApp(TxtSearchGame.Text);

            if(game != null)
            {
                MessageBox.Show(game.appid.ToString());
            }
        }

        private void TxtSearchGame_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TxtSearchGame.Text))
            {
                TxtSearchGame.Text = TxtSearchGame.Tag.ToString();
                TxtSearchGame.Foreground = (SolidColorBrush)FindResource("ColorTypo");
            }
        }

        private void TxtSearchGame_GotFocus(object sender, RoutedEventArgs e)
        {
            TxtSearchGame.Foreground = _txtForeground;
            if (TxtSearchGame.Text.Equals(TxtSearchGame.Tag))
            {
                TxtSearchGame.Text = null;
            }
        }
    }
}
