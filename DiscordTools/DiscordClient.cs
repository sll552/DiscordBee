namespace MusicBeePlugin.DiscordTools
{
  using DiscordRPC;
  using DiscordRPC.Logging;
  using DiscordRPC.Message;
  using System;
  using System.Diagnostics;
  using System.Text;
  using System.Threading.Tasks;

  public class DiscordClient
  {
    private DiscordRpcClient _discordClient;
    private AssetManager _assetManager;
    private readonly LevelDbReader _levelDbReader = new LevelDbReader();
    private RichPresence _discordPresence;
    private string _discordId;
    private string _currentArtworkHash;
    private bool _artworkUploadEnabled;

    public string DiscordId
    {
      get => _discordId;
      set
      {
        if (value != _discordId && !String.IsNullOrWhiteSpace(value))
        {
          _discordId = value;
          Init();
        }
      }
    }

    private bool _isConnected;
    public bool IsConnected
    {
      get => _isConnected;
      private set
      {
        if (value != _isConnected)
        {
          _isConnected = value;
          if (!value && _discordClient?.IsDisposed == false)
          {
            // _isConnected set from true to false and _discordClient is not null and not disposed
            Debug.WriteLine("Closing client connection...", "DiscordBee");
            try
            {
              _discordClient.ClearPresence();
            }
            catch (ObjectDisposedException)
            {
              // connection was null, just continue
            }
            finally
            {
              _discordClient.Dispose();
              _assetManager = null;
            }
          }
        }
      }
    }

    public bool ArtworkUploadEnabled
    {
      get => _artworkUploadEnabled;
      set
      {
        if (value != _artworkUploadEnabled)
        {
          _artworkUploadEnabled = value;
          if (value && IsConnected)
          {
            Init();
          }
          else if (!value)
          {
            _assetManager = null;
          }
        }
      }
    }

    private void Init()
    {
      Debug.WriteLine("Initialising new DiscordClient instance...", "DiscordBee");
      InitDiscordClient();
    }

    private async Task AssetManagerInit()
    {
      _levelDbReader.Init();
      _assetManager = new AssetManager(_levelDbReader.Token, _discordId);
      await _assetManager.Init();
    }

    private void InitDiscordClient()
    {
      // Make sure we are clean
      IsConnected = false;
      _discordClient = new DiscordRpcClient(DiscordId, logger: new DebugLogger(LogLevel.Trace));
      _discordClient.OnError += ErrorCallback;
      _discordClient.OnClose += DisconnectedCallback;
      _discordClient.OnReady += ReadyCallback;
      _discordClient.OnConnectionFailed += ConnectionFailedCallback;
      _discordClient.ShutdownOnly = true;
      _discordClient.SkipIdenticalPresence = true;
      _discordClient.Initialize();
    }

    public void Close()
    {
      IsConnected = false;
    }

    public void UploadArtwork(string artworkData)
    {
      if (IsConnected && _assetManager?.initialised == true)
      {
        _ = _assetManager.UploadAsset(artworkData).ContinueWith(
          t =>
          {
            var id = t.Result;
            if (id == null)
            { // this most likely means another task is already uploading this asset.
              return;
            }
            // Update Cover if it matches current song
            if (id.Equals(_currentArtworkHash))
            {
              _discordPresence.Assets.LargeImageKey = id;
              UpdatePresence();
            }
          }, TaskContinuationOptions.OnlyOnRanToCompletion
        );
      }
    }

    public void SetPresence(RichPresence desired, string artworkData)
    {
      _discordPresence = desired.Clone();
      _currentArtworkHash = AssetManager.GetHash(artworkData ?? "");

      // do preprocessing here
      if (IsConnected)
      {
        if (_assetManager?.initialised == true)
        {
          var assetId = _assetManager.GetCachedAsset(artworkData);
          if (assetId == null)
          {
            assetId = AssetManager.ASSET_LOGO;
            UploadArtwork(artworkData);
          }
          _discordPresence.Assets.LargeImageKey = assetId;
        }
        UpdatePresence();
      }
    }

    public void ClearPresence()
    {
      if (IsConnected)
      {
        _discordPresence = null;
        _discordClient.ClearPresence();
      }
    }

    private void EnsureInit()
    {
      if (!IsConnected && DiscordId != null && (_discordClient?.IsDisposed ?? true))
      {
        // _discordClient is either null or disposed
        Init();
      }
    }

    private void UpdatePresence()
    {
      EnsureInit();
      Debug.WriteLine("Sending Presence update ...", "DiscordBee");
      _discordClient.SetPresence(_discordPresence);
    }

    private void ConnectionFailedCallback(object sender, ConnectionFailedMessage args)
    {
      if (IsConnected)
      {
        IsConnected = false;
      }
    }

    private void ReadyCallback(object sender, ReadyMessage args)
    {
      Debug.WriteLine($"Ready. Connected to Discord Client with User: {args.User.Username}", "DiscordRpc");
      IsConnected = true;
      if (_artworkUploadEnabled)
      {
        _ = AssetManagerInit();
      }
      if (_discordPresence != null)
      {
        UpdatePresence();
      }
    }

    private void ErrorCallback(object sender, ErrorMessage e)
    {
      Debug.Fail($"DiscordRpc: ERROR ({e.Code})", e.Message);
      if (e.Code == ErrorCode.PipeException || e.Code == ErrorCode.UnkownError)
      {
        IsConnected = false;
      }
    }

    private void DisconnectedCallback(object sender, CloseMessage c)
    {
      Debug.WriteLine("DiscordRpc: Disconnected ({0}) - {1}", c.Code, c.Reason);
      IsConnected = false;
    }
  }

  public class DebugLogger : ILogger
  {
    public LogLevel Level { get; set; }

    public DebugLogger(LogLevel level)
    {
      Level = level;
    }

    public void Error(string message, params object[] args)
    {
      if (Level > LogLevel.Error)
      {
        return;
      }

      Log(message, args);
    }

    public void Info(string message, params object[] args)
    {
      if (Level > LogLevel.Info)
      {
        return;
      }

      Log(message, args);
    }

    public void Trace(string message, params object[] args)
    {
      if (Level > LogLevel.Trace)
      {
        return;
      }

      Log(message, args);
    }

    public void Warning(string message, params object[] args)
    {
      if (Level > LogLevel.Warning)
      {
        return;
      }

      Log(message, args);
    }

    private void Log(string msg, params object[] args)
    {
      Debug.WriteLine("" + Level.ToString() + ": " + msg, args);
    }
  }
}
