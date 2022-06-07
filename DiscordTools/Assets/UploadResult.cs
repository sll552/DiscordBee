namespace MusicBeePlugin.DiscordTools.Assets
{
  public class UploadResult
  {
    public string Hash { get; set; }
    public string Link { get; set; }
    public bool Success => !string.IsNullOrEmpty(Link);
  }
}
