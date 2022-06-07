namespace MusicBeePlugin.ImgurClient.Types
{
  using System.Text.Json.Serialization;

  public class ImgurAlbum
  {
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("datetime")]
    public int Datetime { get; set; }
    [JsonPropertyName("cover")]
    public string Cover { get; set; }
    [JsonPropertyName("cover_width")]
    public int CoverWidth { get; set; }
    [JsonPropertyName("cover_height")]
    public int CoverHeight { get; set; }
    [JsonPropertyName("account_url")]
    public string AccountUrl { get; set; }
    [JsonPropertyName("account_id")]
    public int AccountId { get; set; }
    [JsonPropertyName("privacy")]
    public string Privacy { get; set; }
    [JsonPropertyName("layout")]
    public string Layout { get; set; }
    [JsonPropertyName("views")]
    public long Views { get; set; }
    [JsonPropertyName("link")]
    public string Link { get; set; }
    [JsonPropertyName("favorite")]
    public bool Favorite { get; set; }
    [JsonPropertyName("nsfw")]
    public bool Nsfw { get; set; }
    [JsonPropertyName("section")]
    public string Section { get; set; }
    [JsonPropertyName("order")]
    public int Order { get; set; }
    [JsonPropertyName("deletehash")]
    public string DeleteHash { get; set; }
    [JsonPropertyName("images_count")]
    public long ImagesCount { get; set; }
    [JsonPropertyName("images")]
    public ImgurImage[] Images { get; set; }
    [JsonPropertyName("in_gallery")]
    public bool InGallery { get; set; }
  }
}
