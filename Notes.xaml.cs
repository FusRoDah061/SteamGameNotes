using log4net;
using SteamGameNotes.DTO;
using SteamGameNotes.Service;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SteamGameNotes
{
    public partial class Notes : BaseWindow
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Notes));

        private GamesService _gameService = new GamesService();
        private SteamAppDto _game = null;

        public Notes()
        {
            InitializeComponent();
        }

        public Notes(SteamAppDto game)
        {
            InitializeComponent();

            _game = game;

            LblGameName.Text = game.name;

            string imageUrl = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{game.appid}/capsule_184x69.jpg";

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imageUrl, UriKind.Absolute);
            bitmap.EndInit();

            ImgGameImage.Source = bitmap;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Left = this.Left;
            mainWindow.Top = this.Top;
            mainWindow.Show();
            Close();
        }

        private async void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            await _gameService.SaveNotes(_game.appid, TxtNotes.Text);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.OnWindowLoaded();

            try
            {
                string notes = await _gameService.GetNotes(_game.appid);
                TxtNotes.Text = notes;
            }
            catch(Exception ex)
            {
                log.Warn("Error getting notes: ", ex);
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await _gameService.SaveNotes(_game.appid, TxtNotes.Text);
        }

        private void TxtNotes_GotFocus(object sender, RoutedEventArgs e)
        {
            base.RequestActivateWindow();
        }
    }
}
