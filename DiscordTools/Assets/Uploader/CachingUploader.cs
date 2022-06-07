namespace MusicBeePlugin.DiscordTools.Assets.Uploader
{
  using Newtonsoft.Json;
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Threading.Tasks;

  public class CachingUploader : DelegatingUploader
  {
    private readonly string _cacheFilePath;
    private bool _initialized;
    private readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();

    public CachingUploader(string cacheFilePath, IAssetUploader assetUploader) : base(assetUploader)
    {
      _cacheFilePath = cacheFilePath ?? throw new ArgumentNullException(nameof(cacheFilePath));
    }

    public override Task<bool> DeleteAsset(AlbumCoverData assetData)
    {
      _cache.TryRemove(assetData.Hash, out _);
      return base.DeleteAsset(assetData);
    }

    public override void Dispose()
    {
      File.WriteAllText(_cacheFilePath, JsonConvert.SerializeObject(_cache));
      base.Dispose();
    }

    public override Task<Dictionary<string, string>> GetAssets()
    {
      return base.GetAssets().ContinueWith(res =>
      {
        foreach (var assetEntry in res.Result)
        {
          _cache.TryAdd(assetEntry.Key, assetEntry.Value);
        }
        return new Dictionary<string, string>(_cache);
      }, TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    public override async Task<bool> Init()
    {
      await base.Init();

      if (_initialized)
      {
        return true;
      }

      if (File.Exists(_cacheFilePath))
      {
        await Task.Run(
            () =>
            {
              foreach (var entry in JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_cacheFilePath)))
              {
                _cache[entry.Key] = entry.Value;
              }
            }
          );
      }
      else if (Path.GetDirectoryName(_cacheFilePath) != null && !Directory.Exists(Path.GetDirectoryName(_cacheFilePath)))
      {
        Directory.CreateDirectory(Path.GetDirectoryName(_cacheFilePath) ?? throw new InvalidOperationException());
      }

      _initialized = true;
      return true;
    }

    public override bool IsAssetCached(AlbumCoverData assetData)
    {
      return _cache.ContainsKey(assetData.Hash) || base.IsAssetCached(assetData);
    }

    public override bool IsHealthy()
    {
      return !_cache.IsEmpty || base.IsHealthy();
    }

    public override Task<UploadResult> UploadAsset(AlbumCoverData assetData)
    {
      if (_cache.ContainsKey(assetData.Hash))
      {
        Debug.WriteLine($"Returning link from cache for {assetData}");
        _cache.TryGetValue(assetData.Hash, out string ret);
        return Task.FromResult(new UploadResult { Hash = assetData.Hash, Link = ret });
      }

      Debug.WriteLine($"Not found in cache -> uploading {assetData}");
      return base.UploadAsset(assetData).ContinueWith(res =>
      {
        if (res.Result.Success)
        {
          _cache.TryAdd(assetData.Hash, res.Result.Link);
        }
        return res.Result;
      }, TaskContinuationOptions.OnlyOnRanToCompletion);
    }
  }
}
