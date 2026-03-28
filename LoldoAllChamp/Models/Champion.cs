namespace LoldoAllChamp.Models
{
    public class Champion
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl => $"https://ddragon.leagueoflegends.com/cdn/14.6.1/img/champion/{Id}.png";
    }
}
