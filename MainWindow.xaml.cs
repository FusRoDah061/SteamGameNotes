﻿using SteamGameNotes.Controls;
using SteamGameNotes.DTO;
using SteamGameNotes.Helper;
using SteamGameNotes.Service;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SteamGameNotes
{

    public partial class MainWindow : Window
    {
        private Brush _txtForeground = null;
        private GamesService _gamesService = new GamesService();
        private long _steamActiveGameId = 0L;

        public MainWindow()
        {
            InitializeComponent();
            _txtForeground = TxtSearchGame.Foreground;

            // Placeholder
            TxtSearchGame.Foreground = (SolidColorBrush)FindResource("ColorIcons");
            TxtSearchGame.Text = TxtSearchGame.Tag.ToString();

            try
            {
                _steamActiveGameId = SteamHelper.GetActiveGameAppId();
            }
            catch(Exception ex) { }
        }

        private GameItem _buildGameItem(SteamAppDto game)
        {
            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem();
            menuItem.Header = "Remove game";
            menuItem.Click += OnDeleteGame;
            contextMenu.Items.Add(menuItem);

            GameItem item = new GameItem();
            item.GameName = game.name;
            item.AppId = game.appid;
            item.IsPlaying = game.appid.Equals(_steamActiveGameId);
            item.ContextMenu = contextMenu;

            item.MouseLeftButtonUp += OnGameItemClick;

            return item;
        }

        private async void OnDeleteGame(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var gameItem = (GameItem)contextMenu.PlacementTarget;

            await _gamesService.Delete(gameItem.AppId);

            await _refreshGameList();
        }

        private async Task _refreshGameList()
        {
            var games = await _gamesService.ListGames();

            PnlGameList.Children.Clear();
            foreach (var game in games)
            {
                PnlGameList.Children.Add(_buildGameItem(game));
            }
        }

        private void OnGameItemClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var gameItem = (GameItem)sender;

            Notes notes = new Notes(new SteamAppDto(gameItem.AppId, gameItem.GameName));
            notes.Left = this.Left;
            notes.Top = this.Top;
            notes.Show();
            Close();
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
                TxtSearchGame.Foreground = (SolidColorBrush)FindResource("ColorIcons");
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
            await _refreshGameList();
        }
    }
}
