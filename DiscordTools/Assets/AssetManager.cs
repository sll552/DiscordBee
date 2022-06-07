namespace MusicBeePlugin.DiscordTools.Assets
{
  using System;
  using System.Collections.Concurrent;
  using System.Diagnostics;
  using System.Threading;
  using System.Threading.Tasks;

  public class AssetManager : IDisposable
  {
    public const string ASSET_LOGO = "logo";
    public const string ASSET_PLAY = "play";
    public const string ASSET_PAUSE = "pause";
    public const string ASSET_STOP = "stop";

    private readonly ConcurrentDictionary<string, string> _uploadInProgress = new ConcurrentDictionary<string, string>();
    private readonly IAssetUploader _assetUploader;
    public bool initialised;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public AssetManager(IAssetUploader uploader)
    {
      _assetUploader = uploader;
    }

    public async void Init()
    {
      await _semaphore.WaitAsync();
      try
      {
        if (initialised)
        {
          return;
        }
        await _assetUploader.Init();
        await _assetUploader.GetAssets();
        initialised = true;
      }
      catch
      {
        initialised = false;
      }
      finally
      {
        _semaphore.Release();
      }
    }

    public async Task<UploadResult> UploadAsset(AlbumCoverData artworkData)
    {
      Debug.WriteLine($"Get key for asset string: {artworkData}...", "DiscordBee");
      if (!artworkData.HasCover)
      {
        return null;
      }

      Debug.WriteLine($" --> Uploading asset {artworkData} ...", "DiscordBee");
      if (_uploadInProgress.ContainsKey(artworkData.Hash))
      {
        Debug.WriteLine($"Skipping upload for asset {artworkData} because its already being uploaded", "DiscordBee");
        return null;
      }
      _uploadInProgress.TryAdd(artworkData.Hash, "");

      try
      {
        var result = await _assetUploader.UploadAsset(artworkData);
        Debug.WriteLine($" <-- Uploading asset {artworkData} ... finished", "DiscordBee");
        return result;
      }
      catch (Exception e)
      {
        Debug.WriteLine($" <-- Uploading asset {artworkData} ... FAILED with {e.Message}", "DiscordBee");
        return null;
      }
      finally
      {
        _uploadInProgress.TryRemove(artworkData.Hash, out _);
      }
    }

    public bool IsAssetCached(AlbumCoverData artworkData)
    {
      if (!artworkData.HasCover)
      {
        Debug.WriteLine("Artwork is empty -> using default", "DiscordBee");
        return false;
      }

      if (_assetUploader.IsAssetCached(artworkData))
      {
        Debug.WriteLine($"Key for asset {artworkData} returned from cache", "DiscordBee");
        return true;
      }

      return false;
    }

    public void Dispose()
    {
      initialised = false;
      _assetUploader.Dispose();
    }
  }
}
