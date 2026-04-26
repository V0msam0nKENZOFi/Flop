namespace AuthApp.Models
{
    public class DownloadableApp
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DirectUrl { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
    }
}