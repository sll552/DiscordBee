namespace MusicBeePlugin.DiscordTools.Assets
{
  using System.Collections.Generic;

  public class UploaderHealthInfo
  {
    public List<string> AdditionalInfo { get; }
    public bool IsHealthy { get; set; }

    public UploaderHealthInfo(UploaderHealthInfo baseHealth)
    {
      AdditionalInfo = baseHealth.AdditionalInfo;
      IsHealthy = baseHealth.IsHealthy;
    }

    public UploaderHealthInfo()
    {
      AdditionalInfo = new List<string>();
    }

    public void AddInfo(string info)
    {
      if (!string.IsNullOrEmpty(info))
      {
        AdditionalInfo.Add(info);
      }
    }
  }
}
