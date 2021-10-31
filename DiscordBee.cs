namespace MusicBeePlugin
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Text.RegularExpressions;
  using System.Text;
  using DiscordRPC;
  using MusicBeePlugin.DiscordTools;
  using System.Timers;

  public partial class Plugin
  {
    private MusicBeeApiInterface _mbApiInterface;
    private readonly PluginInfo _about = new PluginInfo();
    private DiscordClient _discordClient = new DiscordClient();
    private LayoutHandler _layoutHandler;
    private Settings _settings;
    private SettingsWindow _settingsWindow;
    private Timer _updateTimer = new Timer(500);


    public PluginInfo Initialise(IntPtr apiInterfacePtr)
    {
      _mbApiInterface = new MusicBeeApiInterface();
      _mbApiInterface.Initialise(apiInterfacePtr);
      _about.PluginInfoVersion = PluginInfoVersion;
      _about.Name = "DiscordBee";
      _about.Description = "Update your Discord Profile with the currently playing track";
      _about.Author = "Stefan Lengauer";
      _about.TargetApplication = "";   // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
      _about.Type = PluginType.General;
      _about.VersionMajor = 2;  // your plugin version
      _about.VersionMinor = 0;
      _about.Revision = 0;
      _about.MinInterfaceVersion = MinInterfaceVersion;
      _about.MinApiRevision = MinApiRevision;
      _about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
      _about.ConfigurationPanelHeight = 0;   // height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function

      var settingsFilePath = _mbApiInterface.Setting_GetPersistentStoragePath() + _about.Name + "\\" + _about.Name + ".settings";

      _settings = Settings.GetInstance(settingsFilePath);
      _settings.SettingChanged += _SettingChangedCallback;
      _settingsWindow = new SettingsWindow(this, _settings);

      _discordClient.ArtworkUploadEnabled = _settings.UploadArtwork;
      _discordClient.DiscordId = _settings.DiscordAppId;

      // Match least number of chars possible but min 1
      _layoutHandler = new LayoutHandler(new Regex("\\[(.+?)\\]"));

      _updateTimer.Elapsed += _updateTimer_Elapsed;
      _updateTimer.AutoReset = false;
      _updateTimer.Stop();

      Debug.WriteLine(_about.Name + " loaded");

      return _about;
    }

    private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      UpdateDiscordPresence(_mbApiInterface.Player_GetPlayState());
      _updateTimer.Stop();
    }

    private void _SettingChangedCallback(object sender, Settings.SettingChangedEventArgs e)
    {
      if (e.SettingProperty.Equals("DiscordAppId"))
      {
        _discordClient.DiscordId = _settings.DiscordAppId;
      }
      if (e.SettingProperty.Equals("UploadArtwork"))
      {
        _discordClient.ArtworkUploadEnabled = _settings.UploadArtwork;
      }
    }

    public string GetVersionString()
    {
      return $"{_about.VersionMajor}.{_about.VersionMinor}.{_about.Revision}";
    }

    public bool Configure(IntPtr panelHandle)
    {
      _settingsWindow.Show();
      return true;
    }

    // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
    // its up to you to figure out whether anything has changed and needs updating
    public void SaveSettings()
    {
      _settings.Save();
    }

    // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
    public void Close(PluginCloseReason reason)
    {
      _discordClient.Close();
    }

    // uninstall this plugin - clean up any persisted files
    public void Uninstall()
    {
      _settings.Delete();
    }

    // receive event notifications from MusicBee
    // you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
    public void ReceiveNotification(string sourceFileUrl, NotificationType type)
    {
      Debug.WriteLine("DiscordBee: Recieved Notification {0}", type);
      Debug.WriteLine("    DiscordRpcClient IsConnected: {0}", _discordClient.IsConnected);

      // perform some action depending on the notification type
      switch (type)
      {
        case NotificationType.PluginStartup:
          var playState = _mbApiInterface.Player_GetPlayState();
          // assuming MusicBee wasn't closed and started again in the same Discord session
          if (_settings.UpdatePresenceWhenStopped || playState != PlayState.Paused && playState != PlayState.Stopped)
          {
            UpdateDiscordPresence(playState);
          }
          break;
        case NotificationType.TrackChanged:
        case NotificationType.PlayStateChanged:
          UpdateDiscordPresence(_mbApiInterface.Player_GetPlayState());
          break;
        // When changing the volume this event is fired for every change so with high frequency, we need to deal with that because the UI thread blocks as long as this handler is running
        case NotificationType.VolumeLevelChanged:
          if (!_updateTimer.Enabled)
          {
            _updateTimer.Start();
          }
          break;
      }
    }

    public Dictionary<string, string> GenerateMetaDataDictionary()
    {
      var ret = new Dictionary<string, string>(Enum.GetNames(typeof(MetaDataType)).Length);

      foreach (MetaDataType elem in Enum.GetValues(typeof(MetaDataType)))
      {
        ret.Add(elem.ToString(), _mbApiInterface.NowPlaying_GetFileTag(elem));
      }
      ret.Add("PlayState", _mbApiInterface.Player_GetPlayState().ToString());
      ret.Add("Volume", Convert.ToInt32(_mbApiInterface.Player_GetVolume() * 100.0f).ToString());

      return ret;
    }

    private void UpdateDiscordPresence(PlayState playerGetPlayState)
    {
      Debug.WriteLine("DiscordBee: Updating Presence with PlayState {0}...", playerGetPlayState);
      var metaDataDict = GenerateMetaDataDictionary();
      RichPresence _discordPresence = new RichPresence
      {
        Assets = new Assets(),
        Party = new Party(),
        Timestamps = new Timestamps()
      };

      // Discord allows only strings with a min length of 2 or the update fails
      // so add some exotic space (Mongolian vovel seperator) to the string if it is smaller
      // Discord also disallows strings bigger than 128bytes so handle that as well
      string padString(string input)
      {
        if (!string.IsNullOrEmpty(input))
        {
          if (input.Length < 2)
          {
            return input + "\u180E";
          }
          if (Encoding.UTF8.GetBytes(input).Length > 128)
          {
            byte[] buffer = new byte[128];
            char[] inputChars = input.ToCharArray();
            Encoding.UTF8.GetEncoder().Convert(
                chars: inputChars,
                charIndex: 0,
                charCount: inputChars.Length,
                bytes: buffer,
                byteIndex: 0,
                byteCount: buffer.Length,
                flush: false,
                charsUsed: out _,
                bytesUsed: out int bytesUsed,
                completed: out _);
            return Encoding.UTF8.GetString(buffer, 0, bytesUsed);
          }
        }
        return input;
      }

      void SetImage(string name)
      {
        if (_settings.TextOnly)
        {
          _discordPresence.Assets.LargeImageKey = null;
          _discordPresence.Assets.LargeImageText = null;
          _discordPresence.Assets.SmallImageKey = null;
          _discordPresence.Assets.SmallImageText = null;
          return;
        }
        _discordPresence.Assets.LargeImageKey = AssetManager.ASSET_LOGO;
        _discordPresence.Assets.LargeImageText = padString(_layoutHandler.Render(_settings.LargeImageText, metaDataDict, _settings.Seperator));
        _discordPresence.Assets.SmallImageKey = padString(name);
        _discordPresence.Assets.SmallImageText = padString(_layoutHandler.Render(_settings.SmallImageText, metaDataDict, _settings.Seperator)); ;
      }

      _discordPresence.State = padString(_layoutHandler.Render(_settings.PresenceState, metaDataDict, _settings.Seperator));

      var t = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1));

      if (_settings.ShowRemainingTime)
      {
        // show remaining time
        // subtract current track position from total duration for position seeking
        var totalRemainingDuration = _mbApiInterface.NowPlaying_GetDuration() - _mbApiInterface.Player_GetPosition();
        _discordPresence.Timestamps.EndUnixMilliseconds = (ulong)(Math.Round(t.TotalSeconds) + Math.Round(totalRemainingDuration / 1000.0));
      }
      else
      {
        // show elapsed time
        _discordPresence.Timestamps.StartUnixMilliseconds = (ulong)(Math.Round(t.TotalSeconds) - Math.Round(_mbApiInterface.Player_GetPosition() / 1000.0));
      }

      switch (playerGetPlayState)
      {
        case PlayState.Playing:
          SetImage(AssetManager.ASSET_PLAY);
          break;
        case PlayState.Stopped:
          SetImage(AssetManager.ASSET_STOP);
          _discordPresence.Timestamps.Start = null;
          _discordPresence.Timestamps.End = null;
          break;
        case PlayState.Paused:
          SetImage(AssetManager.ASSET_PAUSE);
          _discordPresence.Timestamps.Start = null;
          _discordPresence.Timestamps.End = null;
          break;
        case PlayState.Undefined:
        case PlayState.Loading:
          break;
      }

      _discordPresence.Details = padString(_layoutHandler.Render(_settings.PresenceDetails, metaDataDict, _settings.Seperator));

      var trackcnt = -1;
      var trackno = -1;
      try
      {
        trackcnt = int.Parse(_layoutHandler.Render(_settings.PresenceTrackCnt, metaDataDict, _settings.Seperator));
        trackno = int.Parse(_layoutHandler.Render(_settings.PresenceTrackNo, metaDataDict, _settings.Seperator));
      }
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
      catch (Exception)
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.
      {
        // ignored
      }

      if (trackcnt < trackno || trackcnt <= 0 || trackno <= 0)
      {
        _discordPresence.Party = null;
      }
      else
      {
        _discordPresence.Party = new Party
        {
          ID = "aaaa",
          Max = trackcnt,
          Size = trackno
        };
      }

      if (!_settings.UpdatePresenceWhenStopped && (playerGetPlayState == PlayState.Paused || playerGetPlayState == PlayState.Stopped))
      {
        Debug.WriteLine("Clearing Presence...", "DiscordBee");
        _discordClient.ClearPresence();
      }
      else
      {
        Debug.WriteLine("Setting new Presence...", "DiscordBee");
        _discordClient.SetPresence(_discordPresence, _mbApiInterface.NowPlaying_GetArtwork());
      }
    }
  }
}
