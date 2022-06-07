namespace MusicBeePlugin.DiscordTools
{
  using System.Security.Cryptography;
  using System.Text;

  public class AlbumCoverData
  {
    public string AlbumName { get; }
    public string ArtistName { get; }
    public string ImageB64 { get; }
    public bool HasCover { get => !string.IsNullOrEmpty(ImageB64); }
    private string _hash;
    private static readonly HashAlgorithm _hashAlgorithm = MD5.Create();

    public string Hash
    {
      get
      {
        if (!HasCover) return null;
        if (_hash == null) ComputeHash();
        return _hash;
      }
    }
    public AlbumCoverData(string albumName, string artistName, string imageB64)
    {
      AlbumName = albumName;
      ArtistName = artistName;
      ImageB64 = imageB64;
    }
    private void ComputeHash()
    {
      byte[] data = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(ImageB64));
      var sBuilder = new StringBuilder();

      // Loop through each byte of the hashed data
      // and format each one as a hexadecimal string.
      for (int i = 0; i < data.Length; i++)
      {
        sBuilder.Append(data[i].ToString("x2"));
      }

      _hash = sBuilder.ToString();
    }

    public override string ToString()
    {
      return $"{ArtistName}|{AlbumName}|{Hash}";
    }
  }
}
