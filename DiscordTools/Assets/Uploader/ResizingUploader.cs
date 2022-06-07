namespace MusicBeePlugin.DiscordTools.Assets.Uploader
{
  using System.Drawing.Imaging;
  using System.IO;
  using System;
  using System.Drawing;
  using System.Drawing.Drawing2D;
  using System.Threading.Tasks;

  public class ResizingUploader : DelegatingUploader
  {
    public ResizingUploader(IAssetUploader innerUploader) : base(innerUploader)
    {
    }

    private Image Base64ToImage(string base64String)
    {
      byte[] imageBytes = Convert.FromBase64String(base64String);
      // Convert byte[] to Image
      var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
      return Image.FromStream(ms, true);
    }

    private string ImageToBase64(Image image, ImageFormat format)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        image.Save(ms, format);
        byte[] imageBytes = ms.ToArray();
        return Convert.ToBase64String(imageBytes);
      }
    }
    private static Bitmap ResizeImage(Image image, int maxWidth, int maxHeight)
    {
      var ratioX = maxWidth / (double)image.Width;
      var ratioY = maxHeight / (double)image.Height;
      var ratio = ratioX < ratioY ? ratioX : ratioY;

      var width = Convert.ToInt32(image.Width * ratio);
      var height = Convert.ToInt32(image.Height * ratio);

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

    public override Task<UploadResult> UploadAsset(AlbumCoverData assetData)
    {
      // As this uploader changes the Hash of the image for all following uploaders, we need to restore the original hash on completion
      // because otherwise the caller would receive an upload result for a seemingly different image
      return base.UploadAsset(GetNormalizeAlbumData(assetData)).ContinueWith(
          t => new UploadResult { Hash = assetData.Hash, Link = t.Result.Link },
          TaskContinuationOptions.OnlyOnRanToCompletion
        );
    }

    public override bool IsAssetCached(AlbumCoverData assetData)
    {
      return base.IsAssetCached(GetNormalizeAlbumData(assetData));
    }

    private AlbumCoverData GetNormalizeAlbumData(AlbumCoverData assetData)
    {
      Image input = Base64ToImage(assetData.ImageB64);
      string imageData = ImageToBase64(ResizeImage(input, 1024, 1024), ImageFormat.Png);

      if (string.IsNullOrEmpty(imageData))
      {
        imageData = ImageToBase64(input, ImageFormat.Png);
      }

      input.Dispose();

      return new AlbumCoverData(assetData.AlbumName, assetData.ArtistName, imageData);
    }
  }
}
