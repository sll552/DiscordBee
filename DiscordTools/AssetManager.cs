namespace MusicBeePlugin.DiscordTools
{
  using Newtonsoft.Json;
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Drawing;
  using System.Drawing.Drawing2D;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Linq;
  using System.Net.Cache;
  using System.Net.Http;
  using System.Net.Http.Headers;
  using System.Reflection;
  using System.Security.Cryptography;
  using System.Text;
  using System.Threading.Tasks;

  class AssetManager
  {
    public const string ASSET_LOGO = "logo";
    public const string ASSET_PLAY = "play";
    public const string ASSET_PAUSE = "pause";
    public const string ASSET_STOP = "stop";
    public const string DiscordApiUrl = "https://discord.com/api/v9/oauth2/applications";

    private const int MAX_ASSETS = 270;

    private readonly string _authToken;
    private readonly string _discordAppId;
    private readonly string _discordAssetApiUrl;
    // Needs to be something that produces at max 32 byte hashes, does not need to be cryptographically perfect
    private static readonly HashAlgorithm _hashAlgorithm = MD5.Create();
    private readonly HttpClient _httpClient = new HttpClient(new WebRequestHandler()
    {
      CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache)
    });
    private readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();
    private readonly ConcurrentDictionary<string, string> _uploadInProgress = new ConcurrentDictionary<string, string>();
    private readonly IReadOnlyDictionary<string, string> _baseAssets = new Dictionary<string, string>()
    {
      {ASSET_LOGO, "MusicBeePlugin.Resources.Icons.MusicBee_Logo.png" },
      {ASSET_PLAY, "MusicBeePlugin.Resources.Icons.play_white.png" },
      {ASSET_PAUSE, "MusicBeePlugin.Resources.Icons.pause_white.png" },
      {ASSET_STOP, "MusicBeePlugin.Resources.Icons.stop_white.png" },
    };
    public bool initialised;

    public AssetManager(string authToken, string discordAppId)
    {
      _authToken = authToken ?? throw new ArgumentNullException(nameof(authToken));
      _discordAppId = discordAppId;
      _discordAssetApiUrl = $"{DiscordApiUrl}/{_discordAppId}/assets";

      _httpClient.DefaultRequestHeaders.Clear();
      _httpClient.DefaultRequestHeaders.Add("Authorization", _authToken);
      _httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
      _httpClient.DefaultRequestHeaders.Add("origin", "discordapp.com");
      _httpClient.DefaultRequestHeaders.Add("cache-control", "no-cache");
      _httpClient.DefaultRequestHeaders.Add("pragma", "no-cache");
      _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
      {
        NoCache = true
      };

    }

    public async Task Init()
    {
      try
      {
        var assetsJson = await GetAssetJson();
        foreach (var asset in JsonConvert.DeserializeObject<List<DiscordAsset>>(assetsJson))
        {
          // Delete doubled uploads from previous sessions
          if (_cache.ContainsKey(asset.Name))
          {
            _ = _httpClient.DeleteAsync($"{_discordAssetApiUrl}/{asset.Id}");
          }
          _cache[asset.Name] = asset.Id;
        }
        Debug.WriteLine($"Asset manager initialised, found the following assets:\n{string.Join(Environment.NewLine, _cache)}", "DiscordBee");
        await EnsureBaseIcons();
      }
      catch
      {
        initialised = false;
      }
      initialised = true;
    }

    private async Task EnsureBaseIcons()
    {
      if (_baseAssets.Keys.All(_cache.ContainsKey))
      {
        Debug.WriteLine($"All required assets are already present, no need to upload them", "DiscordBee");
        return;
      }
      Debug.WriteLine($"Uploading required base assets", "DiscordBee");

      var assembly = Assembly.GetExecutingAssembly();
      var tasks = new List<Task>();

      foreach (var resourceEntry in _baseAssets)
      {
        using (var resStream = assembly.GetManifestResourceStream(resourceEntry.Value))
        {
          var image = Image.FromStream(resStream);
          tasks.Add(UploadAsset(ImageToBase64(image, image.RawFormat), resourceEntry.Key));
        }
      }

      await Task.WhenAll(tasks);
    }

    private Task<string> GetAssetJson()
    {
      return _httpClient.GetAsync(_discordAssetApiUrl).Result.Content.ReadAsStringAsync();
    }
    public static string GetHash(string input)
    {
      byte[] data = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
      var sBuilder = new StringBuilder();

      // Loop through each byte of the hashed data
      // and format each one as a hexadecimal string.
      for (int i = 0; i < data.Length; i++)
      {
        sBuilder.Append(data[i].ToString("x2"));
      }

      return sBuilder.ToString();
    }

    private Image Base64ToImage(string base64String)
    {
      byte[] imageBytes = Convert.FromBase64String(base64String);
      // Convert byte[] to Image
      var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
      return Image.FromStream(ms, true);
    }

    public string ImageToBase64(Image image, ImageFormat format)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        image.Save(ms, format);
        byte[] imageBytes = ms.ToArray();
        return Convert.ToBase64String(imageBytes);
      }
    }

    private async Task UploadAsset(string data, string hash = null)
    {
      if (hash == null)
      {
        hash = GetHash(data);
      }
      if (_cache.ContainsKey(hash))
      {
        Debug.WriteLine($"Skipping upload for asset {hash} because its in cache", "DiscordBee");
        return;
      }
      if (_uploadInProgress.ContainsKey(hash))
      {
        Debug.WriteLine($"Skipping upload for asset {hash} because its already being uploaded", "DiscordBee");
        return;
      }
      _uploadInProgress.TryAdd(hash, "");

      data = GetImageDataString(data);
      var payload = JsonConvert.SerializeObject(new
      {
        name = hash,
        type = "1",
        image = data,
      });
      var content = new StringContent(payload, Encoding.UTF8, "application/json");
      var response = await _httpClient.PostAsync(_discordAssetApiUrl, content);
      if (response.IsSuccessStatusCode)
      {
        string responseString = await response.Content.ReadAsStringAsync();
        DiscordAsset uploadedAsset = JsonConvert.DeserializeObject<DiscordAsset>(responseString);
        // This is required because Discord doesnt care about consistency in their own APIs (https://support.discord.com/hc/en-us/community/posts/360050294314-Assets-not-saving-in-Rich-Presence-tab and https://github.com/discord/discord-api-docs/discussions/3279)
        var available = await CheckAssetAvailable(uploadedAsset.Id);
        if (available)
        {
          _cache[hash] = uploadedAsset.Id;
          Debug.WriteLine($"Asset {hash} successfully uploaded: {responseString}", "DiscordBee");
        }
      }
      else
      {
        Debug.WriteLine($"Asset {hash} upload failed with status {response.StatusCode}", "DiscordBee");
      }
      _uploadInProgress.TryRemove(hash, out _);
    }

    private async Task<bool> CheckAssetAvailable(string id)
    {
      var checkHttpClient = new HttpClient();
      int retryCount = 0;
      bool available;

      checkHttpClient.DefaultRequestHeaders.Clear();
      checkHttpClient.DefaultRequestHeaders.Add("referrer", "https://discord.com/");

      do
      {
        retryCount++;
        // Backoff but max 1.5 min
        await Task.Delay(Math.Min(500 * retryCount, 90000));
        Debug.WriteLine($"Checking if asset {id} is actually available", "DiscordBee");
        // Assets are accessed by the client using a url like https://cdn.discordapp.com/app-assets/{app-id}/{asset-id}.png
        var response = await checkHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, $"https://cdn.discordapp.com/app-assets/{_discordAppId}/{id}.png"));
        available = response.IsSuccessStatusCode;
        if (!available) { continue; }
        // Because Discord is not able to update the list they are checking for the assets it is not enough that the asset is on the CDN but has to be on the asset list, this might take a few minutes (yes, minutes...)
        var assets = JsonConvert.DeserializeObject<List<DiscordAsset>>(await GetAssetJson());
        available = assets.Count(x => x.Id == id) > 0;
      }
      while (!available && retryCount < 3000);

      return available;
    }

    private string GetImageDataString(string data)
    {
      Image input = Base64ToImage(data);
      string dataType = "data:image/png;base64,";
      string imageData = "";

      if (input.Width > 1024 || input.Width < 512 || input.Height > 1024 || input.Height < 512 || input.Width != input.Height)
      {
        Debug.WriteLine($"Resizing image to fit asset requirements {data.Substring(0, 30)}", "DiscordBee");
        imageData = ImageToBase64(ResizeImage(input, 1024, 1024), ImageFormat.Png);
      }
      if (string.IsNullOrEmpty(imageData)) imageData = ImageToBase64(input, ImageFormat.Png);

      input.Dispose();

      return dataType + imageData;
    }
    public static Bitmap ResizeImage(Image image, int width, int height)
    {
      var destRect = new Rectangle(0, 0, width, height);
      var destImage = new Bitmap(width, height);

      destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

      using (var graphics = Graphics.FromImage(destImage))
      {
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using (var wrapMode = new ImageAttributes())
        {
          wrapMode.SetWrapMode(WrapMode.TileFlipXY);
          graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        }
      }

      return destImage;
    }

    public async Task<string> UploadAsset(string artworkData)
    {
      Debug.WriteLine($"Get key for asset string: {artworkData?.Substring(0, 30)}...", "DiscordBee");
      if (string.IsNullOrWhiteSpace(artworkData))
      {
        return null;
      }

      var hash = GetHash(artworkData);
      if (_cache.ContainsKey(hash))
      {
        Debug.WriteLine($"Key for asset {hash} from {artworkData.Substring(0, 30)}... returned from cache", "DiscordBee");
        return hash;
      }

      await CleanUpAssets();
      Debug.WriteLine($" --> Uploading asset {hash} from {artworkData.Substring(0, 30)}...", "DiscordBee");
      await UploadAsset(artworkData, hash);
      Debug.WriteLine($" <-- Uploading asset {hash} from {artworkData.Substring(0, 30)}... finished", "DiscordBee");
      if (_cache.ContainsKey(hash))
      {
        return hash;
      }
      return null;
    }

    private async Task CleanUpAssets()
    {
      if (_cache.Count > MAX_ASSETS)
      {
        Debug.WriteLine($"Max number of assets reached -> deleting old ones", "DiscordBee");
        var tasks = new List<Task>();
        foreach (var cacheEntry in _cache.OrderBy(x => x.Value).Take(20))
        {
          if (_baseAssets.ContainsKey(cacheEntry.Key))
          {
            continue;
          }
          tasks.Add(_httpClient.DeleteAsync($"{_discordAssetApiUrl}/{cacheEntry.Value}").ContinueWith(
              t =>
              {
                var response = t.Result;
                if (response.IsSuccessStatusCode)
                {
                  _cache.TryRemove(cacheEntry.Key, out _);
                  Debug.WriteLine($"Deleted asset {cacheEntry.Key}:{cacheEntry.Value}", "DiscordBee");
                }
              }
            )
          );
        }
        await Task.WhenAll(tasks);
      }
    }

    public string GetCachedAsset(string artworkData)
    {
      if (string.IsNullOrWhiteSpace(artworkData))
      {
        Debug.WriteLine($"Artwork is empty -> using default", "DiscordBee");
        return ASSET_LOGO;
      }
      var hash = GetHash(artworkData);

      if (_cache.ContainsKey(hash))
      {
        Debug.WriteLine($"Key for asset {hash} from {artworkData.Substring(0, 30)}... returned from cache", "DiscordBee");
        return hash;
      }

      return null;
    }
  }

  class DiscordAsset
  {
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
  }
}
