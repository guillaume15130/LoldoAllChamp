namespace LoldoAllChamp.Models
{
    public class Champion
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public void SetVersion(string version)
        {
            ImageUrl = $"https://ddragon.leagueoflegends.com/cdn/{version}/img/champion/{Id}.png";
        }
    }
}
