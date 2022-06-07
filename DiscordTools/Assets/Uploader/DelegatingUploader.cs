namespace MusicBeePlugin.DiscordTools.Assets.Uploader
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  public abstract class DelegatingUploader : IAssetUploader
  {
    private readonly IAssetUploader _innerUploader;

    protected DelegatingUploader(IAssetUploader innerUploader)
    {
      _innerUploader = innerUploader ?? throw new ArgumentNullException(nameof(innerUploader));
    }

    public virtual Task<bool> DeleteAsset(AlbumCoverData assetData)
    {
      return (_innerUploader?.DeleteAsset(assetData)) ?? Task.FromResult(false);
    }

    public virtual void Dispose()
    {
      _innerUploader?.Dispose();
    }

    public virtual Task<Dictionary<string, string>> GetAssets()
    {
      return _innerUploader?.GetAssets() ?? Task.FromResult(new Dictionary<string, string>());
    }

    public virtual async Task<bool> Init()
    {
      return await _innerUploader?.Init();
    }

    public virtual bool IsAssetCached(AlbumCoverData assetData)
    {
      return _innerUploader?.IsAssetCached(assetData) == true;
    }

    public virtual bool IsHealthy()
    {
      return _innerUploader?.IsHealthy() == true;
    }

    public virtual Task<UploadResult> UploadAsset(AlbumCoverData assetData)
    {
      if (_innerUploader?.IsHealthy() == true)
      {
        return _innerUploader?.UploadAsset(assetData) ?? Task.FromResult<UploadResult>(new UploadResult { Hash = assetData.Hash, Link = null });
      }
      else
      {
        return Task.FromResult(new UploadResult { Hash = assetData.Hash, Link = null });
      }
    }
  }
}
