namespace MusicBeePlugin.DiscordTools
{
  using DiscordRPC;
  using DiscordRPC.Logging;
  using DiscordRPC.Message;
  using MusicBeePlugin.DiscordTools.Assets;
  using System;
  using System.Diagnostics;
  using System.Threading;
  using System.Threading.Tasks;
  using Serilog;

  public class DiscordClient : IDisposable
  {
    private DiscordRpcClient _discordClient;
    private AssetManager _assetManager;
    private RichPresence _discordPresence;
    private string _discordId;
    private volatile string _currentArtworkHash;
    private volatile bool _artworkUploadEnabled;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public string DiscordId
    {
      get => _discordId;
      set
      {
        if (value != _discordId && !string.IsNullOrWhiteSpace(value))
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
        }
      }
    }

    public AssetManager AssetManager
    {
      get => _assetManager;
      set
      {
        _assetManager?.Dispose();
        _assetManager = value;
        if (!_assetManager.initialised)
        {
          Task.Run(() => _assetManager.Init());
        }
      }
    }

    private void Init()
    {
      Debug.WriteLine("Initialising new DiscordClient instance...", "DiscordBee");
      InitDiscordClient();
    }

    private void InitDiscordClient()
    {
      // Make sure we are clean
      IsConnected = false;
      _discordClient = new DiscordRpcClient(DiscordId, logger: new DiscordLogger());
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

    public async void UploadArtwork(AlbumCoverData artworkData)
    {
      if (IsConnected && _assetManager?.initialised == true && _artworkUploadEnabled)
      {
        var upload = await _assetManager.UploadAsset(artworkData);

        if (upload == null)
        { // this most likely means another task is already uploading this asset.
          return;
        }
        // Update Cover if it matches current song
        _semaphore.Wait();
        try
        {
          if (upload.Hash?.Equals(_currentArtworkHash) == true && _discordPresence != null)
          {
            if (upload.Success)
            {
              _discordPresence.Assets.LargeImageKey = upload.Link;
            }
            else
            {
              _discordPresence.Assets.LargeImageKey = AssetManager.ASSET_LOGO;
            }
            UpdatePresence();
          }
        }
        finally
        {
          _semaphore.Release();
        }
      }
    }

    public void SetPresence(RichPresence desired, AlbumCoverData artworkData)
    {
      _semaphore.Wait();
      try
      {
        _discordPresence = desired.Clone();
      }
      finally
      {
        _semaphore.Release();
      }
      _currentArtworkHash = artworkData.Hash;

      if (IsConnected)
      {
        if (_artworkUploadEnabled && artworkData.HasCover && _assetManager?.initialised == true)
        {
          if (!_assetManager.IsAssetCached(artworkData))
          {
            _semaphore.Wait();
            try
            {
              _discordPresence.Assets.LargeImageKey = AssetManager.ASSET_LOGO;
              UpdatePresence();
            }
            finally
            {
              _semaphore.Release();
            }
          }
          UploadArtwork(artworkData);
        }
        else
        {
          UpdatePresence();
        }
      }
      else
      {
        EnsureInit();
      }
    }

    public void ClearPresence()
    {
      if (IsConnected)
      {
        _semaphore.Wait();
        try
        {
          _discordPresence = null;
        }
        finally
        {
          _semaphore.Release();
        }

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
      if (_artworkUploadEnabled && _assetManager?.initialised == false)
      {
        Task.Run(() => _assetManager.Init());
      }
      _semaphore.Wait();
      try
      {
        if (_discordPresence != null)
        {
          UpdatePresence();
        }
      }
      finally
      {
        _semaphore.Release();
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

    public void Dispose()
    {
      _discordClient.Dispose();
      _assetManager?.Dispose();
    }
  }

  public class DiscordLogger : DiscordRPC.Logging.ILogger
  {
    public LogLevel Level { get; set; }

    public void Error(string message, params object[] args)
    {
      Log.Error(message, args);
    }

    public void Info(string message, params object[] args)
    {
      Log.Information(message, args);
    }

    public void Trace(string message, params object[] args)
    {
      Log.Verbose(message, args);
    }

    public void Warning(string message, params object[] args)
    {
      Log.Warning(message, args);
    }
  }
}
