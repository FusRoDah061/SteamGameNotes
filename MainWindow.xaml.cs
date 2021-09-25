using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(TxtSearchGame.Text);
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
