using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LoldoAllChamp.Services;

namespace LoldoAllChamp.Views
{
    public partial class PlayerSelectionWindow : Window
    {
        public PlayerSelectionWindow()
        {
            InitializeComponent();
            LoadExistingPlayers();
            PlayerNameBox.Focus();
        }

        private void LoadExistingPlayers()
        {
            var players = SaveService.GetAllPlayerNames();
            ExistingPlayersListBox.ItemsSource = players;

            if (players.Count == 0)
            {
                ExistingPlayersListBox.Visibility = Visibility.Collapsed;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void PlayerNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                StartGame();
        }

        private void ExistingPlayersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExistingPlayersListBox.SelectedItem is string playerName)
            {
                PlayerNameBox.Text = playerName;
                StartGame();
            }
        }

        private void ExistingPlayersListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            var playerName = PlayerNameBox.Text?.Trim();

            if (string.IsNullOrEmpty(playerName))
            {
                MessageBox.Show("Entre ton nom de joueur !", "Oups", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var mainWindow = new MainWindow(playerName);
            mainWindow.Show();
            Close();
        }
    }
}
