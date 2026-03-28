using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LoldoAllChamp.Models;

namespace LoldoAllChamp.Services
{
    public static class DataDragonService
    {
        private static readonly HttpClient _httpClient = new();

        public static async Task<(string version, List<Champion> champions)> FetchChampionsAsync()
        {
            string version = "15.6.1";

            try
            {
                // Get latest version
                var versionJson = await _httpClient.GetStringAsync("https://ddragon.leagueoflegends.com/api/versions.json");
                var versions = JsonConvert.DeserializeObject<List<string>>(versionJson);
                version = versions?.FirstOrDefault() ?? version;
            }
            catch
            {
                // Use fallback version
            }

            try
            {
                // Fetch champion list from API
                var url = $"https://ddragon.leagueoflegends.com/cdn/{version}/data/fr_FR/champion.json";
                var json = await _httpClient.GetStringAsync(url);
                var root = JObject.Parse(json);
                var data = root["data"] as JObject;

                if (data != null)
                {
                    var champions = new List<Champion>();
                    foreach (var property in data.Properties())
                    {
                        var champData = property.Value;
                        var champ = new Champion
                        {
                            Id = property.Name,
                            Name = champData?["name"]?.ToString() ?? property.Name
                        };
                        champ.SetVersion(version);
                        champions.Add(champ);
                    }

                    return (version, champions.OrderBy(c => c.Name).ToList());
                }
            }
            catch
            {
                // Fallback to hardcoded list
            }

            var fallbackChampions = ChampionService.GetAllChampions();
            foreach (var c in fallbackChampions)
                c.SetVersion(version);

            return (version, fallbackChampions);
        }
    }
}
