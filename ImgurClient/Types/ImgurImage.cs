namespace MusicBeePlugin.ImgurClient.Types
{
  using System.Text.Json.Serialization;

  public class ImgurImage
  {
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("datetime")]
    public int Datetime { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("animated")]
    public bool Animated { get; set; }
    [JsonPropertyName("width")]
    public int Width { get; set; }
    [JsonPropertyName("height")]
    public int Height { get; set; }
    [JsonPropertyName("size")]
    public int Size { get; set; }
    [JsonPropertyName("views")]
    public long Views { get; set; }
    [JsonPropertyName("bandwidth")]
    public long Bandwidth { get; set; }
    [JsonPropertyName("deletehash")]
    public string DeleteHash { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("section")]
    public string Section { get; set; }
    [JsonPropertyName("link")]
    public string Link { get; set; }
    [JsonPropertyName("gifv")]
    public string Gifv { get; set; }
    [JsonPropertyName("mp4")]
    public string Mp4 { get; set; }
    [JsonPropertyName("mp4_size")]
    public long Mp4Size { get; set; }
    [JsonPropertyName("looping")]
    public bool Looping { get; set; }
    [JsonPropertyName("favorite")]
    public bool Favorite { get; set; }
    [JsonPropertyName("nsfw	")]
    public bool Nsfw { get; set; }
    [JsonPropertyName("vote")]
    public string Vote { get; set; }
    [JsonPropertyName("in_gallery")]
    public bool InGallery { get; set; }
  }
}
