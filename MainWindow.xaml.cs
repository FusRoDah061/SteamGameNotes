using SteamGameNotes.Controls;
using SteamGameNotes.DTO;
using SteamGameNotes.Service;
using System;
using System.Windows;
using System.Windows.Media;

namespace SteamGameNotes
{

    public partial class MainWindow : Window
    {
        private Brush _txtForeground = null;
        private GamesService _gamesService = new GamesService();

        public MainWindow()
        {
            InitializeComponent();
            _txtForeground = TxtSearchGame.Foreground;

            // Placeholder
            TxtSearchGame.Foreground = (SolidColorBrush)FindResource("ColorTypo");
            TxtSearchGame.Text = TxtSearchGame.Tag.ToString();
        }

        private GameItem _buildGameItem(SteamAppDto game)
        {
            GameItem item = new GameItem();
            item.GameName = game.name;
            item.GameAppId = game.appid;

            return item;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var steamService = new SteamService();

            var game = await steamService.GetSteamApp(TxtSearchGame.Text);

            if(game != null)
            {
                try
                {
                    await _gamesService.Create(game);
                    
                    PnlGameList.Children.Add(_buildGameItem(game));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var games = await _gamesService.ListGames();

            foreach (var game in games)
            {
                PnlGameList.Children.Add(_buildGameItem(game));
            }
        }
    }
}
