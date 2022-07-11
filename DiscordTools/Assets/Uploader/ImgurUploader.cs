namespace MusicBeePlugin.DiscordTools.Assets.Uploader
{
  using MusicBeePlugin.ImgurClient.Types;
  using Newtonsoft.Json;
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;

  public class ImgurUploader : IAssetUploader
  {
    private readonly ImgurClient.ImgurClient _client;
    private readonly string _albumSavePath;
    private ImgurAlbum _album;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public ImgurUploader(string albumSavePath, string imgurClientId)
    {
      _albumSavePath = albumSavePath ?? throw new ArgumentNullException(nameof(albumSavePath));
      _client = new ImgurClient.ImgurClient(imgurClientId);
    }

    public Task<bool> DeleteAsset(AlbumCoverData assetData)
    {
      throw new System.NotImplementedException();
    }

    public void Dispose()
    {
      _client.Dispose();
      if (!File.Exists(_albumSavePath) && _album != null)
      {
        File.WriteAllText(_albumSavePath, JsonConvert.SerializeObject(_album));
      }
    }

    public async Task<Dictionary<string, string>> GetAssets()
    {
      var ret = new Dictionary<string, string>();
      if (_album == null)
      {
        return ret;
      }
      var images = await _client.GetAlbumImages(_album.Id);

      foreach (var image in images)
      {
        ret[image.Title] = image.Link;
      }

      return ret;
    }

    public async Task<bool> Init()
    {
      Debug.WriteLine(" ---> Waiting for semaphore");
      await _semaphore.WaitAsync();
      Debug.WriteLine(" <--- Waiting for semaphore");
      try
      {
        if (_album != null)
        {
          return true;
        }
        Debug.WriteLine(" ---> Creating Album");
        await GetAlbum();
        Debug.WriteLine(" <--- Creating Album");
      }
      finally
      {
        Debug.WriteLine(" ---> Releasing semaphore");
        _semaphore.Release();
      }
      return _album != null;
    }

    private async Task GetAlbum()
    {
      ImgurAlbum tmpAlbum = null;

      if (File.Exists(_albumSavePath))
      {
        tmpAlbum = JsonConvert.DeserializeObject<ImgurAlbum>(File.ReadAllText(_albumSavePath));
        if (string.IsNullOrEmpty(tmpAlbum.DeleteHash))
        {
          File.Delete(_albumSavePath);
          tmpAlbum = null;
        }
        else
        {
          try
          {
            _ = await _client.GetAlbum(tmpAlbum.Id);
          }
          catch
          {
            Debug.WriteLine($"Album does not exist: {tmpAlbum} with id: {tmpAlbum.Id} -> creating new one");
            File.Delete(_albumSavePath);
            tmpAlbum = null;
          }
        }
      }
      else if (Path.GetDirectoryName(_albumSavePath) != null && !Directory.Exists(Path.GetDirectoryName(_albumSavePath)))
      {
        Directory.CreateDirectory(Path.GetDirectoryName(_albumSavePath) ?? throw new InvalidOperationException());
      }

      if (tmpAlbum == null)
      {
        tmpAlbum = await _client.CreateAlbum();
        Debug.WriteLine($"Created album: {tmpAlbum} with deleteHash: {tmpAlbum.DeleteHash}");
      }

      if (tmpAlbum != null)
      {
        _album = tmpAlbum;
        File.WriteAllText(_albumSavePath, JsonConvert.SerializeObject(_album));
      }
    }

    public bool IsAssetCached(AlbumCoverData assetData)
    {
      return false;
    }

    public bool IsHealthy()
    {
      return _client?.IsRateLimited() == false;
    }

    public async Task<UploadResult> UploadAsset(AlbumCoverData assetData)
    {
      if (_album == null)
      {
        return new UploadResult { Hash = assetData.Hash, Link = null };
      }
      var uploaded = await _client.UploadImage(assetData.Hash, assetData.ImageB64, _album.DeleteHash);
      return new UploadResult { Hash = assetData.Hash, Link = uploaded.Link };
    }
  }
}
