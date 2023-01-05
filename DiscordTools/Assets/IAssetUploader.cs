namespace MusicBeePlugin.DiscordTools.Assets
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  public interface IAssetUploader : IDisposable
  {
    bool IsAssetCached(AlbumCoverData assetData);
    Task<UploadResult> UploadAsset(AlbumCoverData assetData);
    Task<bool> DeleteAsset(AlbumCoverData assetData);
    Task<Dictionary<string, string>> GetAssets();
    UploaderHealthInfo GetHealth();
    Task<bool> Init();
  }
}
