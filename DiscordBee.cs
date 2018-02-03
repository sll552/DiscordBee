using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MusicBeePlugin
{
  public partial class Plugin
  {
    private MusicBeeApiInterface _mbApiInterface;
    private readonly PluginInfo _about = new PluginInfo();
    private DiscordRpc.EventHandlers _discordHandlers;
    private DiscordRpc.RichPresence _discordPresence;
    private readonly Timer _discordUpdateTimer = new Timer();

    public const string DiscordRpcDll = "discord-rpc-w32";

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
      _about.VersionMajor = 1;  // your plugin version
      _about.VersionMinor = 0;
      _about.Revision = 0;
      _about.MinInterfaceVersion = MinInterfaceVersion;
      _about.MinApiRevision = MinApiRevision;
      _about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
      _about.ConfigurationPanelHeight = 0;   // height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function
      
      var curdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      if (!File.Exists(curdir + "\\" + DiscordRpcDll + ".dll"))
      {
        Debug.WriteLine("DiscordRPC library could not be found");
        return null;
      }

      _discordHandlers.disconnectedCallback += DisconnectedCallback;
      _discordHandlers.errorCallback += ErrorCallback;
      _discordHandlers.readyCallback += ReadyCallback;
      _discordHandlers.joinCallback += JoinCallback;
      _discordHandlers.requestCallback += RequestCallback;
      _discordHandlers.spectateCallback += SpectateCallback;

      DiscordRpc.Initialize("409394531948298250", ref _discordHandlers, true, null);

      _discordUpdateTimer.AutoReset = false;
      _discordUpdateTimer.Interval = 1000;
      _discordUpdateTimer.Elapsed += DiscordUpdateTimerOnElapsed;

      Debug.WriteLine(_about.Name + " loaded");

      return _about;
    }

    private void DiscordUpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
    {
      DiscordRpc.UpdatePresence(ref _discordPresence);
      _discordUpdateTimer.Stop();
    }

    private void SpectateCallback(string secret)
    {
      // not relevant
    }

    private void RequestCallback(ref DiscordRpc.JoinRequest request)
    {
      // not relevant
    }

    private void JoinCallback(string secret)
    {
      // not relevant
    }

    private void ReadyCallback()
    {
    }

    private void ErrorCallback(int errorCode, string message)
    {
      _discordUpdateTimer.Stop();
      Debug.WriteLine("Errored: " + errorCode + "Msg: " + message );
    }

    private void DisconnectedCallback(int errorCode, string message)
    {
      _discordUpdateTimer.Stop();
      Debug.WriteLine("Disconnected: " + errorCode + "Msg: " + message);
    }

    public bool Configure(IntPtr panelHandle)
    {
      return false;
    }

    // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
    // its up to you to figure out whether anything has changed and needs updating
    public void SaveSettings()
    {

    }

    // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
    public void Close(PluginCloseReason reason)
    {
      _discordUpdateTimer.Stop();
      DiscordRpc.Shutdown();
    }

    // uninstall this plugin - clean up any persisted files
    public void Uninstall()
    {
    }

    // receive event notifications from MusicBee
    // you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
    public void ReceiveNotification(string sourceFileUrl, NotificationType type)
    {
      // perform some action depending on the notification type
      switch (type)
      {
        case NotificationType.PluginStartup:
        case NotificationType.TrackChanged:
        case NotificationType.PlayStateChanged:
          UpdateDiscordPresence(_mbApiInterface.Player_GetPlayState());
          break;
       case NotificationType.TrackChanging:
          break;case NotificationType.AutoDjStarted:
          break;
        case NotificationType.AutoDjStopped:
          break;
        case NotificationType.VolumeMuteChanged:
          break;
        case NotificationType.VolumeLevelChanged:
          break;
        case NotificationType.NowPlayingListChanged:
          break;
        case NotificationType.NowPlayingListEnded:
          break;
        case NotificationType.NowPlayingArtworkReady:
          break;
        case NotificationType.NowPlayingLyricsReady:
          break;
        case NotificationType.TagsChanging:
          break;
        case NotificationType.TagsChanged:
          break;
        case NotificationType.RatingChanging:
          break;
        case NotificationType.RatingChanged:
          break;
        case NotificationType.PlayCountersChanged:
          break;
        case NotificationType.ScreenSaverActivating:
          break;
        case NotificationType.ShutdownStarted:
          break;
        case NotificationType.EmbedInPanel:
          break;
        case NotificationType.PlayerRepeatChanged:
          break;
        case NotificationType.PlayerShuffleChanged:
          break;
        case NotificationType.PlayerEqualiserOnOffChanged:
          break;
        case NotificationType.PlayerScrobbleChanged:
          break;
        case NotificationType.ReplayGainChanged:
          break;
        case NotificationType.FileDeleting:
          break;
        case NotificationType.FileDeleted:
          break;
        case NotificationType.ApplicationWindowChanged:
          break;
        case NotificationType.StopAfterCurrentChanged:
          break;
        case NotificationType.LibrarySwitched:
          break;
        case NotificationType.FileAddedToLibrary:
          break;
        case NotificationType.FileAddedToInbox:
          break;
        case NotificationType.SynchCompleted:
          break;
        case NotificationType.DownloadCompleted:
          break;
        case NotificationType.MusicBeeStarted:
          break;
      }
    }

    private void UpdateDiscordPresence(PlayState playerGetPlayState)
    {
      void SetImage(string name)
      {
        _discordPresence.largeImageKey = "logo";
        _discordPresence.largeImageText = "MusicBee";
        _discordPresence.smallImageKey = name;
        _discordPresence.smallImageText = name;
      }

      _discordPresence.state = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackTitle).Trim();
      var t = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1));
      _discordPresence.startTimestamp = (long) (Math.Round(t.TotalSeconds) - Math.Round(_mbApiInterface.Player_GetPosition() / 1000.0));

      switch (playerGetPlayState)
      {
        case PlayState.Playing:
          //_discordPresence.state += " Playing";
          SetImage("play");
          break;
        case PlayState.Stopped:
          //_discordPresence.state += " Stopped";
          SetImage("stop");
          _discordPresence.startTimestamp = 0;
          break;
        case PlayState.Paused:
          //_discordPresence.state += " Paused";
          SetImage("pause");
          _discordPresence.startTimestamp = 0;
          break;
        case PlayState.Undefined:
        case PlayState.Loading:
          break;
      }

      _discordPresence.details = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist).Trim();
      var album = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Album);
      if (!string.IsNullOrEmpty(album))
      {
         _discordPresence.details += " - " + album.Trim();
      }
     
      _discordPresence.partyId = "aaaaa";

      var trackcnt = 0;
      var trackno = 0;
      try
      {
        trackcnt = int.Parse(_mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackCount));
        trackno = int.Parse(_mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackNo));
      }
      catch (Exception)
      {
        // ignored
      }
      _discordPresence.partyMax = trackcnt;
      _discordPresence.partySize = trackno;

      if (!_discordUpdateTimer.Enabled) _discordUpdateTimer.Start();
    }
  }
}