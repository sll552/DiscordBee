namespace MusicBeePlugin.ImgurClient.Types
{
  using System.Text.Json.Serialization;

  public class ImgurResponse<T>
  {
    [JsonPropertyName("data")]
    public T Data { get; set; }
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("status")]
    public int Status { get; set; }
  }
}
