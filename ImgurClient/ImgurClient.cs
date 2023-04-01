namespace MusicBeePlugin.ImgurClient
{
  using MusicBeePlugin.ImgurClient.Types;
  using RestSharp;
  using RestSharp.Serializers.NewtonsoftJson;
  using System;
  using System.Net;
  using System.Threading;
  using System.Threading.Tasks;

  public class ImgurClient : IDisposable
  {
    public const string ImgurApiUrl = "https://api.imgur.com/3";
    private readonly RestClient _client;
    private RateLimitHandler _rateLimitHandler;

    public ImgurClient(string clientId)
    {
      var options = new RestClientOptions(ImgurApiUrl)
      {
        ThrowOnAnyError = true,
        FollowRedirects = true,
        PreAuthenticate = true,
        MaxTimeout = 20 * 1000,
        //Proxy = new WebProxy("http://localhost:8080"),
        ConfigureMessageHandler = orig =>
        {
          _rateLimitHandler = new RateLimitHandler(orig);
          return _rateLimitHandler;
        },
        Authenticator = new ImgurAuthenticator(clientId)
      };
      _client = new RestClient(options, configureSerialization: s => s.UseNewtonsoftJson());
    }

    public async Task<ImgurAlbum> CreateAlbum()
    {
      var request = new RestRequest("album");
      request.AddParameter("title", "DiscordBee");
      request.Method = Method.Post;
      try
      {
        var response = await _client.PostAsync<ImgurResponse<ImgurAlbum>>(request);
        return GetResponseData(response);
      }
      catch
      {
        return null;
      }
    }

    public async Task<ImgurImage> UploadImage(string title, string dataB64, string albumHash = null)
    {
      var request = new RestRequest("upload");

      request.AddParameter("image", dataB64);
      if (!string.IsNullOrEmpty(albumHash))
      {
        request.AddParameter("album", albumHash);
      }
      request.AddParameter("type", "base64");
      request.AddParameter("title", title);
      request.AddParameter("name", "cover.png");
      var response = await _client.PostAsync<ImgurResponse<ImgurImage>>(request);
      return GetResponseData(response);
    }

    public async Task<ImgurAlbum> GetAlbum(string albumHash)
    {
      var response = await _client.GetJsonAsync<ImgurResponse<ImgurAlbum>>("album/{albumHash}", new { albumHash });
      return GetResponseData(response);
    }

    public async Task<ImgurImage[]> GetAlbumImages(string albumHash)
    {
      var response = await _client.GetJsonAsync<ImgurResponse<ImgurImage[]>>("album/{albumHash}/images", new { albumHash });
      return GetResponseData(response);
    }

    private static T GetResponseData<T>(ImgurResponse<T> response)
    {
      if (response != null)
      {
        return response.Data;
      }
      return default;
    }

    public (bool status, string info) IsRateLimited()
    {
      return (_rateLimitHandler?.IsRateLimited == true, _rateLimitHandler?.RateLimitInfo);
    }

    public void Dispose()
    {
      _client?.Dispose();
      GC.SuppressFinalize(this);
    }
  }
}
