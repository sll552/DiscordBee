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
      get => _seperator == null ? "./-_" : _seperator;
      set => _seperator = value;
    }

    [DataMember] private string _largeImageText;

    public string LargeImageText
    {
      get => _largeImageText == null ? "MusicBee" : _largeImageText;
      set => _largeImageText = value;
    }

    [DataMember] private string _smallImageText;

    public string SmallImageText
    {
      get => _smallImageText == null ? "[Volume]%" : _smallImageText;
      set => _smallImageText = value;
    }

    [DataMember] private string _presenceState;

    public string PresenceState
    {
      get => _presenceState == null ? "[TrackTitle]" : _presenceState;
      set => _presenceState = value;
    }

    [DataMember] private string _presenceDetails;

    public string PresenceDetails
    {
      get => _presenceDetails == null ? "[Artist] - [Album]" : _presenceDetails;
      set => _presenceDetails = value;
    }

    [DataMember] private string _presenceTrackCnt;

    public string PresenceTrackCnt
    {
      get => _presenceTrackCnt == null ? "[TrackCount]" : _presenceTrackCnt;
      set => _presenceTrackCnt = value;
    }

    [DataMember] private string _presenceTrackNo;

    public string PresenceTrackNo
    {
      get => _presenceTrackNo == null ? "[TrackNo]" : _presenceTrackNo;
      set => _presenceTrackNo = value;
    }

    [DataMember] private bool? _updatePresenceWhenStopped;

    public bool UpdatePresenceWhenStopped
    {
      get => !_updatePresenceWhenStopped.HasValue || _updatePresenceWhenStopped.Value;
      set
      {
        // preserve deserialized value or null when no change
        if (UpdatePresenceWhenStopped != value)
        {
          _updatePresenceWhenStopped = value;
        }
      }
    }

    [DataMember] private bool? _showRemainingTime;

    public bool ShowRemainingTime
    {
      get => _showRemainingTime.HasValue && _showRemainingTime.Value;
      set
      {
        // preserve deserialized value or null when no change
        if (ShowRemainingTime != value)
        {
          _showRemainingTime = value;
        }
      }
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

        if (!fieldInfo.Name.StartsWith("_")) continue;
        if (fieldInfo.FieldType == typeof(string) && !(fieldInfo.GetValue(this) as string == null))
        {
          return true;
        }
        if (fieldInfo.FieldType == typeof(bool?) && fieldInfo.GetValue(this) != null)
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
          propertyInfo.SetValue(this, null, null);
        }
      }

      // field is used for boolean settings because nullable is used internally and property would be non-nullable
      var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

      foreach (var fieldInfo in fields)
      {
        if (!fieldInfo.Name.StartsWith("_")) continue;
        if (fieldInfo.FieldType == typeof(bool?))
        {
          fieldInfo.SetValue(this, null);
        }
      }
    }
  }
}