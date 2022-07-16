namespace MusicBeePlugin
{
  using DiscordRPC;
  using MusicBeePlugin.DiscordTools;
  using MusicBeePlugin.DiscordTools.Assets;
  using MusicBeePlugin.DiscordTools.Assets.Uploader;
  using MusicBeePlugin.UI;
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;
  using System.Timers;
  using System.Windows.Forms;
  using Button = DiscordRPC.Button;
  using Timer = System.Timers.Timer;

  public partial class Plugin
  {
    private MusicBeeApiInterface _mbApiInterface;
    private readonly PluginInfo _about = new PluginInfo();
    private readonly DiscordClient _discordClient = new DiscordClient();
    private UploaderHealth _uploaderStatusWindow;
    private LayoutHandler _layoutHandler;
    private Settings _settings;
    private SettingsWindow _settingsWindow;
    private readonly Timer _updateTimer = new Timer(300);
    private string _imgurAssetCachePath;
    private string _imgurAlbum;

    public Plugin()
    {
      AppDomain.CurrentDomain.AssemblyResolve += (object _, ResolveEventArgs args) =>
      {
        string assemblyFile = args.Name.Contains(",")
            ? args.Name.Substring(0, args.Name.IndexOf(','))
            : args.Name;

        assemblyFile += ".dll";

        string absoluteFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        string targetPath = Path.Combine(absoluteFolder, "DiscordBee", assemblyFile);

        try
        {
          Debug.WriteLine($"Trying to load assembly {targetPath}");
          return Assembly.LoadFile(targetPath);
        }
        catch (Exception ex)
        {
          Debug.WriteLine($"Failed to load assembly {targetPath}: {ex.Message}");
          return null;
        }
      };
    }

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
      _about.VersionMajor = 3;  // your plugin version
      _about.VersionMinor = 0;
      _about.Revision = 1;
      _about.MinInterfaceVersion = MinInterfaceVersion;
      _about.MinApiRevision = MinApiRevision;
      _about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents;
      _about.ConfigurationPanelHeight = 0;   // height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function

      var workingDir = _mbApiInterface.Setting_GetPersistentStoragePath() + _about.Name;
      var settingsFilePath = $"{workingDir}\\{_about.Name}.settings";
      _imgurAssetCachePath = $"{workingDir}\\{_about.Name}-Imgur.cache";
      _imgurAlbum = $"{workingDir}\\{_about.Name}-Imgur.album";

      _settings = Settings.GetInstance(settingsFilePath);
      _settings.SettingChanged += SettingChangedCallback;
      _settingsWindow = new SettingsWindow(this, _settings);

      _discordClient.ArtworkUploadEnabled = _settings.UploadArtwork;
      _discordClient.DiscordId = _settings.DiscordAppId;
      UpdateAssetManager(_imgurAssetCachePath, new ImgurUploader(_imgurAlbum, _settings.ImgurClientId));
      ToolStripMenuItem mainMenuItem = (ToolStripMenuItem)_mbApiInterface.MB_AddMenuItem($"mnuTools/{_about.Name}", null, null);
      mainMenuItem.DropDown.Items.Add("Uploader Health", null, ShowUploaderHealth);

      // Match least number of chars possible but min 1
      _layoutHandler = new LayoutHandler(new Regex("\\[([^[]+?)\\]"));

      _updateTimer.Elapsed += UpdateTimerElapsedCallback;
      _updateTimer.AutoReset = false;
      _updateTimer.Stop();

      Debug.WriteLine(_about.Name + " loaded");

      return _about;
    }

    private void UpdateAssetManager(string cachePath, IAssetUploader actualUploader)
    {
      _discordClient.AssetManager = new AssetManager(new ResizingUploader(new CachingUploader(cachePath, actualUploader)));
      _uploaderStatusWindow?.Dispose();
      _uploaderStatusWindow = new UploaderHealth(new List<IAssetUploader> { actualUploader });
    }

    private void ShowUploaderHealth(object sender, EventArgs e)
    {
      _uploaderStatusWindow?.Show();
    }

    private void UpdateTimerElapsedCallback(object sender, ElapsedEventArgs e)
    {
      UpdateDiscordPresence(_mbApiInterface.Player_GetPlayState());
      _updateTimer.Stop();
    }

    private void SettingChangedCallback(object sender, Settings.SettingChangedEventArgs e)
    {
      if (e.SettingProperty.Equals("DiscordAppId"))
      {
        _discordClient.DiscordId = _settings.DiscordAppId;
      }
      if (e.SettingProperty.Equals("UploadArtwork"))
      {
        _discordClient.ArtworkUploadEnabled = _settings.UploadArtwork;
      }
      if (e.SettingProperty.Equals("ImgurClientId"))
      {
        UpdateAssetManager(_imgurAssetCachePath, new ImgurUploader(_imgurAlbum, _settings.ImgurClientId));
      }
    }

    public string GetVersionString()
    {
      return $"{_about.VersionMajor}.{_about.VersionMinor}.{_about.Revision}";
    }

    public bool Configure(IntPtr _)
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
    public void Close(PluginCloseReason _)
    {
      _discordClient.Dispose();
    }

    // uninstall this plugin - clean up any persisted files
    public void Uninstall()
    {
      _settings.Delete();
    }

    // receive event notifications from MusicBee
    // you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
    public void ReceiveNotification(string _, NotificationType type)
    {
      Debug.WriteLine("DiscordBee: Received Notification {0}", type);
      Debug.WriteLine("    DiscordRpcClient IsConnected: {0}", _discordClient.IsConnected);

      // perform some action depending on the notification type
      switch (type)
      {
        case NotificationType.PluginStartup:
          var playState = _mbApiInterface.Player_GetPlayState();
          // assuming MusicBee wasn't closed and started again in the same Discord session
          if (_settings.UpdatePresenceWhenStopped || (playState != PlayState.Paused && playState != PlayState.Stopped))
          {
            UpdateDiscordPresence(playState);
          }
          break;
        case NotificationType.TrackChanged:
        case NotificationType.PlayStateChanged:
        // When changing the volume this event is fired for every change so with high frequency, we need to deal with that because the UI thread blocks as long as this handler is running
        case NotificationType.VolumeLevelChanged:
          if (!_updateTimer.Enabled)
          {
            _updateTimer.Start();
          }
          break;
      }
    }

    private AlbumCoverData GetAlbumCoverData(string artworkData, Dictionary<string, string> metaData = null)
    {
      if (metaData == null)
      {
        metaData = GenerateMetaDataDictionary();
      }

      if (!metaData.TryGetValue(nameof(MetaDataType.Artist), out string artist))
      {
        metaData.TryGetValue(nameof(MetaDataType.AlbumArtist), out artist);
      }
      metaData.TryGetValue(nameof(MetaDataType.Album), out string album);

      return new AlbumCoverData(album, artist, artworkData);
    }

    public Dictionary<string, string> GenerateMetaDataDictionary(string fileUrl = null)
    {
      var ret = new Dictionary<string, string>(Enum.GetNames(typeof(MetaDataType)).Length + Enum.GetNames(typeof(FilePropertyType)).Length);

      foreach (MetaDataType elem in Enum.GetValues(typeof(MetaDataType)))
      {
        ret.Add(elem.ToString(), string.IsNullOrWhiteSpace(fileUrl) ? _mbApiInterface.NowPlaying_GetFileTag(elem) : _mbApiInterface.Library_GetFileTag(fileUrl, elem));
      }
      foreach (FilePropertyType elem in Enum.GetValues(typeof(FilePropertyType)))
      {
        ret.Add(elem.ToString(), string.IsNullOrWhiteSpace(fileUrl) ? _mbApiInterface.NowPlaying_GetFileProperty(elem) : _mbApiInterface.Library_GetFileProperty(fileUrl, elem));
      }
      ret.Add("Extension", Path.GetExtension(string.IsNullOrWhiteSpace(fileUrl) ? _mbApiInterface.NowPlaying_GetFileUrl() : fileUrl).TrimStart('.').ToUpper());
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
      // so add some exotic space (Mongolian vowel separator) to the string if it is smaller
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

      // Button Functionality
      if (_settings.ShowButton)
      {
        var uri = _layoutHandler.RenderUrl(_settings.ButtonUrl, metaDataDict);
        Debug.WriteLine($"Url: {uri}");

        // Validate the URL again.
        if (ValidationHelpers.ValidateUri(uri))
        {
          _discordPresence.Buttons = new Button[]
          {
            new Button
            {
              Label = padString(_settings.ButtonLabel),
              Url = uri
            }
          };
        }
      }

      void SetImage(string name, bool forceHideSmallImage = false)
      {
        if (_settings.TextOnly)
        {
          _discordPresence.Assets.LargeImageKey = null;
          _discordPresence.Assets.LargeImageText = null;
          _discordPresence.Assets.SmallImageKey = null;
          _discordPresence.Assets.SmallImageText = null;
        }
        else
        {
          _discordPresence.Assets.LargeImageKey = AssetManager.ASSET_LOGO;
          _discordPresence.Assets.LargeImageText = padString(_layoutHandler.Render(_settings.LargeImageText, metaDataDict, _settings.Separator));

          if (_settings.ShowPlayState && !forceHideSmallImage)
          {
            _discordPresence.Assets.SmallImageKey = padString(name);
            _discordPresence.Assets.SmallImageText = padString(_layoutHandler.Render(_settings.SmallImageText, metaDataDict, _settings.Separator));
          }
          else
          {
            _discordPresence.Assets.SmallImageKey = null;
            _discordPresence.Assets.SmallImageText = null;
          }
        }
      }

      _discordPresence.State = padString(_layoutHandler.Render(_settings.PresenceState, metaDataDict, _settings.Separator));

      var t = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1));

      if (_settings.ShowTime)
      {
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
      }

      switch (playerGetPlayState)
      {
        case PlayState.Playing:
          SetImage(AssetManager.ASSET_PLAY, _settings.ShowOnlyNonPlayingState);
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

      _discordPresence.Details = padString(_layoutHandler.Render(_settings.PresenceDetails, metaDataDict, _settings.Separator));

      var trackcnt = -1;
      var trackno = -1;
      try
      {
        trackcnt = int.Parse(_layoutHandler.Render(_settings.PresenceTrackCnt, metaDataDict, _settings.Separator));
        trackno = int.Parse(_layoutHandler.Render(_settings.PresenceTrackNo, metaDataDict, _settings.Separator));
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
        Task.Run(() => _discordClient.SetPresence(_discordPresence, GetAlbumCoverData(_mbApiInterface.NowPlaying_GetArtwork(), metaDataDict)));
      }
    }
  }
}
