using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MusicBeePlugin
{
  public class Settings
  {
    [XmlIgnore] private string FilePath { get; set; }

    // Don't serialize properties so only user set changes are serialized and not default values
    #region Settings
    private string _seperator;
    [XmlIgnore]
    public string Seperator
    {
      get => string.IsNullOrEmpty(_seperator) ? "./-_" : _seperator;
      set => _seperator = value;
    }
    private string _imagetext;
    [XmlIgnore]
    public string ImageText
    {
      get => string.IsNullOrEmpty(_imagetext) ? "MusicBee" : _imagetext;
      set => _imagetext = value;
    }
    private string _presenceState;
    [XmlIgnore]
    public string PresenceState
    {
      get => string.IsNullOrEmpty(_presenceState) ? "{TrackTitle}" : _presenceState;
      set => _presenceState = value;
    }
    private string _presenceDetails;
    [XmlIgnore]
    public string PresenceDetails
    {
      get => string.IsNullOrEmpty(_presenceDetails) ? "{Artist} - {Album}" : _presenceDetails;
      set => _presenceDetails = value;
    }
    private string _presenceTrackCnt;
    [XmlIgnore]
    public string PresenceTrackCnt
    {
      get => string.IsNullOrEmpty(_presenceTrackCnt) ? "{TrackCount}" : _presenceTrackCnt;
      set => _presenceTrackCnt = value;
    }
    private string _presenceTrackNo;
    [XmlIgnore]
    public string PresenceTrackNo
    {
      get => string.IsNullOrEmpty(_presenceTrackNo) ? "{TrackNo}" : _presenceTrackNo;
      set => _presenceTrackNo = value;
    }
    #endregion

    public static Settings GetInstance(string filePath)
    {
      Settings newSettings;

      try
      {
        newSettings = Load(filePath);
      }
      catch (Exception e) when (e is IOException || e is XmlException || e is InvalidOperationException)
      {
        newSettings = new Settings();
      }
      newSettings.FilePath = filePath;

      return newSettings;
    }

    public void Save()
    {
      using (var writer = new StreamWriter(FilePath))
      {
        var serializer = new XmlSerializer(GetType());
        serializer.Serialize(writer, this);
        writer.Flush();
      }
    }

    private static Settings Load(string filePath)
    {
      using (var stream = File.OpenRead(filePath))
      {
        var serializer = new XmlSerializer(typeof(Settings));
        return serializer.Deserialize(stream) as Settings;
      }
    }

    public void Delete()
    {
      if (File.Exists(FilePath))
      {
        File.Delete(FilePath);
      }
      if (Path.GetDirectoryName(FilePath) != null && Directory.Exists(Path.GetDirectoryName(FilePath)))
      {
        Directory.Delete(Path.GetDirectoryName(FilePath) ?? throw new InvalidOperationException());
      }
      Clear();
    }

    private void Clear()
    {
      var properties = GetType().GetProperties();

      foreach (var propertyInfo in properties)
      {
        if (propertyInfo.PropertyType == typeof(string) && propertyInfo.Name != "FilePath")
        {
          propertyInfo.SetValue(this, string.Empty, null);
        }
      }
    }
  }
}
