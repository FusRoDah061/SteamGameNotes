using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SteamGameNotes.Controls
{
    public partial class GameItem : UserControl
    {
        public static readonly DependencyProperty GameAppIdProperty = DependencyProperty.Register(
            "GameAppId", 
            typeof(long), 
            typeof(GameItem), 
            new UIPropertyMetadata(0L, gameAppIdChangedCallback));
        public static readonly DependencyProperty GameNameProperty = DependencyProperty.Register(
            "GameName", 
            typeof(string), 
            typeof(GameItem),
            new UIPropertyMetadata("GameName"));

        public long GameAppId
        {
            get { return (long)GetValue(GameAppIdProperty); }
            set { SetValue(GameAppIdProperty, value); }
        }

        public string GameName
        {
            get { return (string)GetValue(GameNameProperty); }
            set { SetValue(GameNameProperty, value); }
        }

        public GameItem()
        {
            InitializeComponent();
        }

        private static void gameAppIdChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (GameItem)d;
            string imageUrl = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{control.GameAppId}/capsule_184x69.jpg";

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imageUrl, UriKind.Absolute);
            bitmap.EndInit();

            control.ImgGameImage.Source = bitmap;
        }

        private void root_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            root.Background = (SolidColorBrush)FindResource("ColorBackgroundDarker");
        }

        private void root_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            root.Background = (SolidColorBrush)FindResource("ColorBackground");
        }
    }
}
