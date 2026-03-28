using System.Collections.Generic;

namespace LoldoAllChamp.Models
{
    public class PlayerProgress
    {
        public string PlayerName { get; set; } = string.Empty;
        public List<string> CompletedChampionIds { get; set; } = new();
    }
}
