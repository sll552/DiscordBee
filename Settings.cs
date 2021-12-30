namespace MusicBeePlugin
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.Serialization;
  using System.Xml;

  [DataContract]
  public class Settings
  {
    private string FilePath { get; set; }
    public bool IsDirty { get; private set; }

    public static readonly IReadOnlyDictionary<string, string> defaults = new Dictionary<string, string>()
    {
      {"Seperator", "./-_"},
      {"LargeImageText", "MusicBee"},
      {"SmallImageText", "[Volume]%"},
      {"PresenceState", "[TrackTitle]"},
      {"PresenceDetails", "[Artist] - [Album]"},
      {"PresenceTrackCnt", "[TrackCount]"},
      {"PresenceTrackNo", "[TrackNo]"},
      {"DiscordAppId", "409394531948298250"}, // prod
      //{"DiscordAppId", "408977077799354379"}, // dev
    };

    public event EventHandler<SettingChangedEventArgs> SettingChanged;

    // Don't serialize properties so only user set changes are serialized and not default values

    #region Settings

    [DataMember] private string _seperator;

    public string Seperator
    {
      get => _seperator == null ? defaults["Seperator"] : _seperator;
      set => setIfChanged("_seperator", value);
    }

    [DataMember] private string _largeImageText;

    public string LargeImageText
    {
      get => _largeImageText == null ? defaults["LargeImageText"] : _largeImageText;
      set => setIfChanged("_largeImageText", value);
    }

    [DataMember] private string _smallImageText;

    public string SmallImageText
    {
      get => _smallImageText == null ? defaults["SmallImageText"] : _smallImageText;
      set => setIfChanged("_smallImageText", value);
    }

    [DataMember] private string _presenceState;

    public string PresenceState
    {
      get => _presenceState == null ? defaults["PresenceState"] : _presenceState;
      set => setIfChanged("_presenceState", value);
    }

    [DataMember] private string _presenceDetails;

    public string PresenceDetails
    {
      get => _presenceDetails == null ? defaults["PresenceDetails"] : _presenceDetails;
      set => setIfChanged("_presenceDetails", value);
    }

    [DataMember] private string _presenceTrackCnt;

    public string PresenceTrackCnt
    {
      get => _presenceTrackCnt == null ? defaults["PresenceTrackCnt"] : _presenceTrackCnt;
      set => setIfChanged("_presenceTrackCnt", value);
    }

    [DataMember] private string _presenceTrackNo;

    public string PresenceTrackNo
    {
      get => _presenceTrackNo == null ? defaults["PresenceTrackNo"] : _presenceTrackNo;
      set => setIfChanged("_presenceTrackNo", value);
    }

    [DataMember] private string _discordAppId;

    public string DiscordAppId
    {
      get => _discordAppId == null ? defaults["DiscordAppId"] : _discordAppId;
      set
      {
        if (value != null && value.Equals(defaults["DiscordAppId"]))
        {
          _discordAppId = null;
          return;
        }
        setIfChanged("_discordAppId", value);
      }
    }

    [DataMember] private bool? _updatePresenceWhenStopped;

    public bool UpdatePresenceWhenStopped
    {
      get => !_updatePresenceWhenStopped.HasValue || _updatePresenceWhenStopped.Value;
      set => setIfChanged("_updatePresenceWhenStopped", value);
    }

    [DataMember] private bool? _showTime;

    public bool ShowTime
    {
      get => _showTime.HasValue && _showTime.Value;
      set => setIfChanged("_showTime", value);
    }

    [DataMember] private bool? _showRemainingTime;

    public bool ShowRemainingTime
    {
      get => _showRemainingTime.HasValue && _showRemainingTime.Value;
      set => setIfChanged("_showRemainingTime", value);
    }

    [DataMember] private bool? _textOnly;

    public bool TextOnly
    {
      get => _textOnly.HasValue && _textOnly.Value;
      set => setIfChanged("_textOnly", value);
    }

    [DataMember] private bool? _showPlayState;

    public bool ShowPlayState
    {
      get => _showPlayState.HasValue && _showPlayState.Value;
      set => setIfChanged("_showPlayState", value);
    }

    [DataMember] private bool? _showOnlyNonPlayingState;

    public bool ShowOnlyNonPlayingState
    {
      get => _showOnlyNonPlayingState.HasValue && _showOnlyNonPlayingState.Value;
      set => setIfChanged("_showOnlyNonPlayingState", value);
    }

    [DataMember] private bool? _uploadArtwork;

    public bool UploadArtwork
    {
      get => _uploadArtwork.HasValue && _uploadArtwork.Value;
      set
      {
        if (!DiscordAppId.Equals(defaults["DiscordAppId"]))
        {
          setIfChanged("_uploadArtwork", value);
        }
        else
        {
          var eventArgs = new SettingChangedEventArgs();
          eventArgs.SettingProperty = "UploadArtwork";
          eventArgs.OldValue = _uploadArtwork;
          eventArgs.NewValue = null;
          _uploadArtwork = null;
          IsDirty = true;
          OnSettingChanged(eventArgs);
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

    private void setIfChanged<T>(string fieldName, T value)
    {
      FieldInfo target = GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

      if (target != null)
      {
        PropertyInfo targetProp = GetType().GetProperty(getPropertyNameForField(target.Name), BindingFlags.Instance | BindingFlags.Public);
        object old = targetProp.GetValue(this, null);
        if (targetProp != null && !old.Equals(value))
        {
          target.SetValue(this, value);
          IsDirty = true;
          var eventArgs = new SettingChangedEventArgs();
          eventArgs.SettingProperty = targetProp.Name;
          eventArgs.OldValue = old;
          eventArgs.NewValue = value;
          OnSettingChanged(eventArgs);
        }
      }
    }

    private string getPropertyNameForField(string field)
    {
      if (field.StartsWith("_"))
      {
        string tmp = field.Remove(0, 1);
        return tmp.First().ToString().ToUpper() + tmp.Substring(1);
      }
      return null;
    }

    public void Save()
    {
      if (!IsDirty) return;
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

    public void Clear()
    {
      foreach (var propertyInfo in GetType().GetProperties())
      {
        if (propertyInfo.PropertyType == typeof(string) && propertyInfo.Name != "FilePath")
        {
          propertyInfo.SetValue(this, null, null);
        }
      }

      // field is used for boolean settings because nullable is used internally and property would be non-nullable
      foreach (var fieldInfo in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
      {
        if (!fieldInfo.Name.StartsWith("_")) continue;
        if (fieldInfo.FieldType == typeof(bool?))
        {
          fieldInfo.SetValue(this, null);
        }
      }

      IsDirty = false;
    }

    protected virtual void OnSettingChanged(SettingChangedEventArgs e)
    {
      EventHandler<SettingChangedEventArgs> handler = SettingChanged;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    public class SettingChangedEventArgs : EventArgs
    {
      public string SettingProperty { get; set; }
      public object OldValue { get; set; }
      public object NewValue { get; set; }
    }
  }
}
