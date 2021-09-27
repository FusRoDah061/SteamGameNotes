using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SteamGameNotes.Controls
{
    public partial class GameItem : UserControl
    {
        public static readonly DependencyProperty AppIdProperty = DependencyProperty.Register(
            "AppId", 
            typeof(long), 
            typeof(GameItem), 
            new UIPropertyMetadata(0L, gameAppIdChangedCallback));

        public static readonly DependencyProperty GameNameProperty = DependencyProperty.Register(
            "GameName", 
            typeof(string), 
            typeof(GameItem),
            new UIPropertyMetadata("GameName"));

        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(
            "IsPlaying",
            typeof(bool),
            typeof(GameItem),
            new UIPropertyMetadata(false, gameIsPlayingChangedCallback));

        public long AppId
        {
            get { return (long)GetValue(AppIdProperty); }
            set { SetValue(AppIdProperty, value); }
        }

        public string GameName
        {
            get { return (string)GetValue(GameNameProperty); }
            set { SetValue(GameNameProperty, value); }
        }

        public bool IsPlaying 
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        public GameItem()
        {
            InitializeComponent();
        }

        private static void gameAppIdChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (GameItem)d;
            string imageUrl = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{control.AppId}/capsule_184x69.jpg";

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imageUrl, UriKind.Absolute);
            bitmap.EndInit();

            control.ImgGameImage.Source = bitmap;
        }

        private static void gameIsPlayingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (GameItem)d;
            if (control.IsPlaying)
            {
                control.LblGameName.Foreground = (SolidColorBrush)control.FindResource("ColorTextGreen");
            }
            else
            {
                control.LblGameName.Foreground = (SolidColorBrush)control.FindResource("ColorTextBlue");
            }
        }

        private void root_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsPlaying)
            {
                root.Background = (SolidColorBrush)FindResource("ColorBackgroundGreenHover");
            }
            else
            {
                root.Background = (SolidColorBrush)FindResource("ColorBackgroundBlueHover");
            }
        }

        private void root_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            root.Background = (SolidColorBrush)FindResource("ColorBackground");
        }
    }
}
