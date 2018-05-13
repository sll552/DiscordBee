using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;

namespace MusicBeePlugin
{
  [DataContract]
  public class Settings
  {
    private string FilePath { get; set; }

    // Don't serialize properties so only user set changes are serialized and not default values

    #region Settings

    [DataMember] private string _seperator;

    public string Seperator
    {
      get => string.IsNullOrEmpty(_seperator) ? "./-_" : _seperator;
      set => _seperator = value;
    }

    [DataMember] private string _imagetext;

    public string ImageText
    {
      get => string.IsNullOrEmpty(_imagetext) ? "MusicBee" : _imagetext;
      set => _imagetext = value;
    }

    [DataMember] private string _presenceState;

    public string PresenceState
    {
      get => string.IsNullOrEmpty(_presenceState) ? "[TrackTitle]" : _presenceState;
      set => _presenceState = value;
    }

    [DataMember] private string _presenceDetails;

    public string PresenceDetails
    {
      get => string.IsNullOrEmpty(_presenceDetails) ? "[Artist] - [Album]" : _presenceDetails;
      set => _presenceDetails = value;
    }

    [DataMember] private string _presenceTrackCnt;

    public string PresenceTrackCnt
    {
      get => string.IsNullOrEmpty(_presenceTrackCnt) ? "[TrackCount]" : _presenceTrackCnt;
      set => _presenceTrackCnt = value;
    }

    [DataMember] private string _presenceTrackNo;

    public string PresenceTrackNo
    {
      get => string.IsNullOrEmpty(_presenceTrackNo) ? "[TrackNo]" : _presenceTrackNo;
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

    public bool IsDirty()
    {
      var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

      foreach (var fieldInfo in fields)
      {
        if (fieldInfo.FieldType != typeof(string) || !fieldInfo.Name.StartsWith("_")) continue;
        if (!string.IsNullOrEmpty(fieldInfo.GetValue(this) as string))
        {
          return true;
        }
      }

      return false;
    }

    public void Save()
    {
      if (!IsDirty()) return;
      if (Path.GetDirectoryName(FilePath) != null && !Directory.Exists(Path.GetDirectoryName(FilePath)))
      {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? throw new InvalidOperationException());
      }

      using (var writer = XmlWriter.Create(FilePath))
      {
        var serializer = new DataContractSerializer(GetType());
        serializer.WriteObject(writer, this);
        writer.Flush();
      }
    }

    private static Settings Load(string filePath)
    {
      using (var stream = File.OpenRead(filePath))
      {
        var serializer = new DataContractSerializer(typeof(Settings));
        return serializer.ReadObject(stream) as Settings;
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