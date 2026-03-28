using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using LoldoAllChamp.Models;

namespace LoldoAllChamp.Services
{
    public static class SaveService
    {
        private static readonly string SaveDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LoldoAllChamp");

        private static readonly string SaveFilePath = Path.Combine(SaveDirectory, "progress.json");

        private static List<PlayerProgress> LoadAll()
        {
            if (!File.Exists(SaveFilePath))
                return new List<PlayerProgress>();

            var json = File.ReadAllText(SaveFilePath);
            return JsonConvert.DeserializeObject<List<PlayerProgress>>(json) ?? new List<PlayerProgress>();
        }

        private static void SaveAll(List<PlayerProgress> allProgress)
        {
            Directory.CreateDirectory(SaveDirectory);
            var json = JsonConvert.SerializeObject(allProgress, Formatting.Indented);
            File.WriteAllText(SaveFilePath, json);
        }

        public static List<string> GetCompletedChampions(string playerName)
        {
            var allProgress = LoadAll();
            var player = allProgress.FirstOrDefault(p =>
                p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));
            return player?.CompletedChampionIds ?? new List<string>();
        }

        public static void MarkChampionCompleted(string playerName, string championId)
        {
            var allProgress = LoadAll();
            var player = allProgress.FirstOrDefault(p =>
                p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));

            if (player == null)
            {
                player = new PlayerProgress { PlayerName = playerName };
                allProgress.Add(player);
            }

            if (!player.CompletedChampionIds.Contains(championId))
            {
                player.CompletedChampionIds.Add(championId);
            }

            SaveAll(allProgress);
        }

        public static void UnmarkChampion(string playerName, string championId)
        {
            var allProgress = LoadAll();
            var player = allProgress.FirstOrDefault(p =>
                p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));

            if (player != null)
            {
                player.CompletedChampionIds.Remove(championId);
                SaveAll(allProgress);
            }
        }

        public static List<string> GetAllPlayerNames()
        {
            return LoadAll().Select(p => p.PlayerName).ToList();
        }
    }
}
