using SteamGameNotes.DTO;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SteamGameNotes
{
    public partial class Notes : Window
    {
        public Notes()
        {
            InitializeComponent();
        }

        public Notes(SteamAppDto game)
        {
            InitializeComponent();

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
    }
}
