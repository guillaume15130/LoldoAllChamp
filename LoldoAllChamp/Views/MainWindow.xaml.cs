using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LoldoAllChamp.Models;
using LoldoAllChamp.Services;

namespace LoldoAllChamp.Views
{
    public partial class MainWindow : Window
    {
        private readonly string _playerName;
        private List<Champion> _allChampions = new();
        private HashSet<string> _completedChampions;

        public MainWindow(string playerName)
        {
            InitializeComponent();
            _playerName = playerName;
            _completedChampions = new HashSet<string>(SaveService.GetCompletedChampions(playerName));

            PlayerNameText.Text = playerName;
            LoadChampionsAsync();
        }

        private async void LoadChampionsAsync()
        {
            try
            {
                var (version, champions) = await DataDragonService.FetchChampionsAsync();
                _allChampions = champions;
            }
            catch
            {
                _allChampions = ChampionService.GetAllChampions();
                foreach (var c in _allChampions)
                    c.SetVersion("15.6.1");
            }

            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            var searchText = SearchBox?.Text?.Trim() ?? string.Empty;
            var showCompleted = ShowCompletedCheckBox?.IsChecked == true;

            var filtered = _allChampions.Where(c =>
            {
                var matchesSearch = string.IsNullOrEmpty(searchText) ||
                    c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);

                var isCompleted = _completedChampions.Contains(c.Id);

                if (showCompleted)
                    return matchesSearch;
                else
                    return matchesSearch && !isCompleted;
            }).ToList();

            ChampionsItemsControl.ItemsSource = filtered;

            // Update stats
            var total = _allChampions.Count;
            var done = _completedChampions.Count;
            var remaining = total - done;

            StatsText.Text = $"{done}/{total} champions joués ({remaining} restants)";
            ProgressBar.Maximum = total;
            ProgressBar.Value = done;
        }

        private void ChampionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string championId)
            {
                var champion = _allChampions.FirstOrDefault(c => c.Id == championId);
                if (champion == null) return;

                if (_completedChampions.Contains(championId))
                {
                    var result = MessageBox.Show(
                        $"Remettre {champion.Name} dans la liste ?",
                        "Annuler",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _completedChampions.Remove(championId);
                        SaveService.UnmarkChampion(_playerName, championId);
                    }
                }
                else
                {
                    var result = MessageBox.Show(
                        $"Tu as joué {champion.Name} ? GG !",
                        "Champion joué",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _completedChampions.Add(championId);
                        SaveService.MarkChampionCompleted(_playerName, championId);
                    }
                }

                RefreshDisplay();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshDisplay();
        }

        private void ShowCompletedCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            RefreshDisplay();
        }

        private void ChangePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            var selectionWindow = new PlayerSelectionWindow();
            selectionWindow.Show();
            Close();
        }
    }
}
