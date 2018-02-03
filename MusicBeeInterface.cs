using System;
using System.Runtime.InteropServices;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        public const short PluginInfoVersion = 1;
        public const short MinInterfaceVersion = 36;
        public const short MinApiRevision = 48;

        [StructLayout(LayoutKind.Sequential)]
        public struct MusicBeeApiInterface
        {
            public void Initialise(IntPtr apiInterfacePtr)
            {
                CopyMemory(ref this, apiInterfacePtr, 4);
                if (MusicBeeVersion == MusicBeeVersion.v2_0)
                    // MusicBee version 2.0 - Api methods > revision 25 are not available
                    CopyMemory(ref this, apiInterfacePtr, 456);
                else if (MusicBeeVersion == MusicBeeVersion.v2_1)
                    CopyMemory(ref this, apiInterfacePtr, 516);
                else if (MusicBeeVersion == MusicBeeVersion.v2_2)
                    CopyMemory(ref this, apiInterfacePtr, 584);
                else if (MusicBeeVersion == MusicBeeVersion.v2_3)
                    CopyMemory(ref this, apiInterfacePtr, 596);
                else if (MusicBeeVersion == MusicBeeVersion.v2_4)
                    CopyMemory(ref this, apiInterfacePtr, 604);
                else if (MusicBeeVersion == MusicBeeVersion.v2_5)
                    CopyMemory(ref this, apiInterfacePtr, 648);
                else
                    CopyMemory(ref this, apiInterfacePtr, Marshal.SizeOf(this));
            }
            public MusicBeeVersion MusicBeeVersion
            {
                get {
                    if (ApiRevision <= 25)
                        return MusicBeeVersion.v2_0;
                    else if (ApiRevision <= 31)
                        return MusicBeeVersion.v2_1;
                    else if (ApiRevision <= 33)
                        return MusicBeeVersion.v2_2;
                    else if (ApiRevision <= 38)
                        return MusicBeeVersion.v2_3;
                    else if (ApiRevision <= 43)
                        return MusicBeeVersion.v2_4;
                    else if (ApiRevision <= 47)
                        return MusicBeeVersion.v2_5;
                    else
                        return MusicBeeVersion.v3_0;
                }
            }
            public short InterfaceVersion;
            public short ApiRevision;
            public MB_ReleaseStringDelegate MB_ReleaseString;
            public MB_TraceDelegate MB_Trace;
            public Setting_GetPersistentStoragePathDelegate Setting_GetPersistentStoragePath;
            public Setting_GetSkinDelegate Setting_GetSkin;
            public Setting_GetSkinElementColourDelegate Setting_GetSkinElementColour;
            public Setting_IsWindowBordersSkinnedDelegate Setting_IsWindowBordersSkinned;
            public Library_GetFilePropertyDelegate Library_GetFileProperty;
            public Library_GetFileTagDelegate Library_GetFileTag;
            public Library_SetFileTagDelegate Library_SetFileTag;
            public Library_CommitTagsToFileDelegate Library_CommitTagsToFile;
            public Library_GetLyricsDelegate Library_GetLyrics;
            [Obsolete("Use Library_GetArtworkEx")]
            public Library_GetArtworkDelegate Library_GetArtwork;
            public Library_QueryFilesDelegate Library_QueryFiles;
            public Library_QueryGetNextFileDelegate Library_QueryGetNextFile;
            public Player_GetPositionDelegate Player_GetPosition;
            public Player_SetPositionDelegate Player_SetPosition;
            public Player_GetPlayStateDelegate Player_GetPlayState;
            public Player_ActionDelegate Player_PlayPause;
            public Player_ActionDelegate Player_Stop;
            public Player_ActionDelegate Player_StopAfterCurrent;
            public Player_ActionDelegate Player_PlayPreviousTrack;
            public Player_ActionDelegate Player_PlayNextTrack;
            public Player_ActionDelegate Player_StartAutoDj;
            public Player_ActionDelegate Player_EndAutoDj;
            public Player_GetVolumeDelegate Player_GetVolume;
            public Player_SetVolumeDelegate Player_SetVolume;
            public Player_GetMuteDelegate Player_GetMute;
            public Player_SetMuteDelegate Player_SetMute;
            public Player_GetShuffleDelegate Player_GetShuffle;
            public Player_SetShuffleDelegate Player_SetShuffle;
            public Player_GetRepeatDelegate Player_GetRepeat;
            public Player_SetRepeatDelegate Player_SetRepeat;
            public Player_GetEqualiserEnabledDelegate Player_GetEqualiserEnabled;
            public Player_SetEqualiserEnabledDelegate Player_SetEqualiserEnabled;
            public Player_GetDspEnabledDelegate Player_GetDspEnabled;
            public Player_SetDspEnabledDelegate Player_SetDspEnabled;
            public Player_GetScrobbleEnabledDelegate Player_GetScrobbleEnabled;
            public Player_SetScrobbleEnabledDelegate Player_SetScrobbleEnabled;
            public NowPlaying_GetFileUrlDelegate NowPlaying_GetFileUrl;
            public NowPlaying_GetDurationDelegate NowPlaying_GetDuration;
            public NowPlaying_GetFilePropertyDelegate NowPlaying_GetFileProperty;
            public NowPlaying_GetFileTagDelegate NowPlaying_GetFileTag;
            public NowPlaying_GetLyricsDelegate NowPlaying_GetLyrics;
            public NowPlaying_GetArtworkDelegate NowPlaying_GetArtwork;
            public NowPlayingList_ActionDelegate NowPlayingList_Clear;
            public Library_QueryFilesDelegate NowPlayingList_QueryFiles;
            public Library_QueryGetNextFileDelegate NowPlayingList_QueryGetNextFile;
            public NowPlayingList_FileActionDelegate NowPlayingList_PlayNow;
            public NowPlayingList_FileActionDelegate NowPlayingList_QueueNext;
            public NowPlayingList_FileActionDelegate NowPlayingList_QueueLast;
            public NowPlayingList_ActionDelegate NowPlayingList_PlayLibraryShuffled;
            public Playlist_QueryPlaylistsDelegate Playlist_QueryPlaylists;
            public Playlist_QueryGetNextPlaylistDelegate Playlist_QueryGetNextPlaylist;
            public Playlist_GetTypeDelegate Playlist_GetType;
            public Playlist_QueryFilesDelegate Playlist_QueryFiles;
            public Library_QueryGetNextFileDelegate Playlist_QueryGetNextFile;
            public MB_WindowHandleDelegate MB_GetWindowHandle;
            public MB_RefreshPanelsDelegate MB_RefreshPanels;
            public MB_SendNotificationDelegate MB_SendNotification;
            public MB_AddMenuItemDelegate MB_AddMenuItem;
            public Setting_GetFieldNameDelegate Setting_GetFieldName;
            [Obsolete("Use Library_QueryFilesEx", true)]
            public Library_QueryGetAllFilesDelegate Library_QueryGetAllFiles;
            [Obsolete("Use NowPlayingList_QueryFilesEx", true)]
            public Library_QueryGetAllFilesDelegate NowPlayingList_QueryGetAllFiles;
            [Obsolete("Use Playlist_QueryFilesEx", true)]
            public Library_QueryGetAllFilesDelegate Playlist_QueryGetAllFiles;
            public MB_CreateBackgroundTaskDelegate MB_CreateBackgroundTask;
            public MB_SetBackgroundTaskMessageDelegate MB_SetBackgroundTaskMessage;
            public MB_RegisterCommandDelegate MB_RegisterCommand;
            public Setting_GetDefaultFontDelegate Setting_GetDefaultFont;
            public Player_GetShowTimeRemainingDelegate Player_GetShowTimeRemaining;
            public NowPlayingList_GetCurrentIndexDelegate NowPlayingList_GetCurrentIndex;
            public NowPlayingList_GetFileUrlDelegate NowPlayingList_GetListFileUrl;
            public NowPlayingList_GetFilePropertyDelegate NowPlayingList_GetFileProperty;
            public NowPlayingList_GetFileTagDelegate NowPlayingList_GetFileTag;
            public NowPlaying_GetSpectrumDataDelegate NowPlaying_GetSpectrumData;
            public NowPlaying_GetSoundGraphDelegate NowPlaying_GetSoundGraph;
            public MB_GetPanelBoundsDelegate MB_GetPanelBounds;
            public MB_AddPanelDelegate MB_AddPanel;
            public MB_RemovePanelDelegate MB_RemovePanel;
            public MB_GetLocalisationDelegate MB_GetLocalisation;
            public NowPlayingList_IsAnyPriorTracksDelegate NowPlayingList_IsAnyPriorTracks;
            public NowPlayingList_IsAnyFollowingTracksDelegate NowPlayingList_IsAnyFollowingTracks;
            public Player_ShowEqualiserDelegate Player_ShowEqualiser;
            public Player_GetAutoDjEnabledDelegate Player_GetAutoDjEnabled;
            public Player_GetStopAfterCurrentEnabledDelegate Player_GetStopAfterCurrentEnabled;
            public Player_GetCrossfadeDelegate Player_GetCrossfade;
            public Player_SetCrossfadeDelegate Player_SetCrossfade;
            public Player_GetReplayGainModeDelegate Player_GetReplayGainMode;
            public Player_SetReplayGainModeDelegate Player_SetReplayGainMode;
            public Player_QueueRandomTracksDelegate Player_QueueRandomTracks;
            public Setting_GetDataTypeDelegate Setting_GetDataType;
            public NowPlayingList_GetNextIndexDelegate NowPlayingList_GetNextIndex;
            public NowPlaying_GetArtistPictureDelegate NowPlaying_GetArtistPicture;
            public NowPlaying_GetArtworkDelegate NowPlaying_GetDownloadedArtwork;
            // api version 16
            public MB_ShowNowPlayingAssistantDelegate MB_ShowNowPlayingAssistant;
            // api version 17
            public NowPlaying_GetLyricsDelegate NowPlaying_GetDownloadedLyrics;
            // api version 18
            public Player_GetShowRatingTrackDelegate Player_GetShowRatingTrack;
            public Player_GetShowRatingLoveDelegate Player_GetShowRatingLove;
            // api version 19
            public MB_CreateParameterisedBackgroundTaskDelegate MB_CreateParameterisedBackgroundTask;
            public Setting_GetLastFmUserIdDelegate Setting_GetLastFmUserId;
            public Playlist_GetNameDelegate Playlist_GetName;
            public Playlist_CreatePlaylistDelegate Playlist_CreatePlaylist;
            public Playlist_SetFilesDelegate Playlist_SetFiles;
            public Library_QuerySimilarArtistsDelegate Library_QuerySimilarArtists;
            public Library_QueryLookupTableDelegate Library_QueryLookupTable;
            public Library_QueryGetLookupTableValueDelegate Library_QueryGetLookupTableValue;
            public NowPlayingList_FilesActionDelegate NowPlayingList_QueueFilesNext;
            public NowPlayingList_FilesActionDelegate NowPlayingList_QueueFilesLast;
            // api version 20
            public Setting_GetWebProxyDelegate Setting_GetWebProxy;
            // api version 21
            public NowPlayingList_RemoveAtDelegate NowPlayingList_RemoveAt;
            // api version 22
            public Playlist_RemoveAtDelegate Playlist_RemoveAt;
            // api version 23
            public MB_SetPanelScrollableAreaDelegate MB_SetPanelScrollableArea;
            // api version 24
            public MB_InvokeCommandDelegate MB_InvokeCommand;
            public MB_OpenFilterInTabDelegate MB_OpenFilterInTab;
            // api version 25
            public MB_SetWindowSizeDelegate MB_SetWindowSize;
            public Library_GetArtistPictureDelegate Library_GetArtistPicture;
            public Pending_GetFileUrlDelegate Pending_GetFileUrl;
            public Pending_GetFilePropertyDelegate Pending_GetFileProperty;
            public Pending_GetFileTagDelegate Pending_GetFileTag;
            // api version 26
            public Player_GetButtonEnabledDelegate Player_GetButtonEnabled;
            // api version 27
            public NowPlayingList_MoveFilesDelegate NowPlayingList_MoveFiles;
            // api version 28
            public Library_GetArtworkDelegate Library_GetArtworkUrl;
            public Library_GetArtistPictureThumbDelegate Library_GetArtistPictureThumb;
            public NowPlaying_GetArtworkDelegate NowPlaying_GetArtworkUrl;
            public NowPlaying_GetArtworkDelegate NowPlaying_GetDownloadedArtworkUrl;
            public NowPlaying_GetArtistPictureThumbDelegate NowPlaying_GetArtistPictureThumb;
            // api version 29
            public Playlist_IsInListDelegate Playlist_IsInList;
            // api version 30
            public Library_GetArtistPictureUrlsDelegate Library_GetArtistPictureUrls;
            public NowPlaying_GetArtistPictureUrlsDelegate NowPlaying_GetArtistPictureUrls;
            // api version 31
            public Playlist_AddFilesDelegate Playlist_AppendFiles;
            // api version 32
            public Sync_FileStartDelegate Sync_FileStart;
            public Sync_FileEndDelegate Sync_FileEnd;
            // api version 33
            public Library_QueryFilesExDelegate Library_QueryFilesEx;
            public Library_QueryFilesExDelegate NowPlayingList_QueryFilesEx;
            public Playlist_QueryFilesExDelegate Playlist_QueryFilesEx;
            public Playlist_MoveFilesDelegate Playlist_MoveFiles;
            public Playlist_PlayNowDelegate Playlist_PlayNow;
            public NowPlaying_IsSoundtrackDelegate NowPlaying_IsSoundtrack;
            public NowPlaying_GetArtistPictureUrlsDelegate NowPlaying_GetSoundtrackPictureUrls;
            public Library_GetDevicePersistentIdDelegate Library_GetDevicePersistentId;
            public Library_SetDevicePersistentIdDelegate Library_SetDevicePersistentId;
            public Library_FindDevicePersistentIdDelegate Library_FindDevicePersistentId;
            public Setting_GetValueDelegate Setting_GetValue;
            public Library_AddFileToLibraryDelegate Library_AddFileToLibrary;
            public Playlist_DeletePlaylistDelegate Playlist_DeletePlaylist;
            public Library_GetSyncDeltaDelegate Library_GetSyncDelta;
            // api version 35
            public Library_GetFileTagsDelegate Library_GetFileTags;
            public NowPlaying_GetFileTagsDelegate NowPlaying_GetFileTags;
            public NowPlayingList_GetFileTagsDelegate NowPlayingList_GetFileTags;
            // api version 43
            public MB_AddTreeNodeDelegate MB_AddTreeNode;
            public MB_DownloadFileDelegate MB_DownloadFile;
            // api version 47
            public Setting_GetFileConvertCommandLineDelegate Setting_GetFileConvertCommandLine;
            public Player_OpenStreamHandleDelegate Player_OpenStreamHandle;
            public Player_UpdatePlayStatisticsDelegate Player_UpdatePlayStatistics;
            public Library_GetArtworkExDelegate Library_GetArtworkEx;
            public Library_SetArtworkExDelegate Library_SetArtworkEx;
            public MB_GetVisualiserInformationDelegate MB_GetVisualiserInformation;
            public MB_ShowVisualiserDelegate MB_ShowVisualiser;
            public MB_GetPluginViewInformationDelegate MB_GetPluginViewInformation;
            public MB_ShowPluginViewDelegate MB_ShowPluginView;
            public Player_GetOutputDevicesDelegate Player_GetOutputDevices;
            public Player_SetOutputDeviceDelegate Player_SetOutputDevice;
            // api version 48
            public MB_UninistallPluginDelegate MB_UninstallPlugin;
        }

        public enum MusicBeeVersion
        {
            v2_0 = 0,
            v2_1 = 1,
            v2_2 = 2,
            v2_3 = 3,
            v2_4 = 4,
            v2_5 = 5,
            v3_0 = 6
        }

        public enum PluginType
        {
            Unknown = 0,
            General = 1,
            LyricsRetrieval = 2,
            ArtworkRetrieval = 3,
            PanelView = 4,
            DataStream = 5,
            InstantMessenger = 6,
            Storage = 7,
            VideoPlayer = 8,
            DSP = 9,
            TagRetrieval = 10,
            TagOrArtworkRetrieval = 11,
            Upnp = 12
        }

        [StructLayout(LayoutKind.Sequential)]
        public class PluginInfo
        {
            public short PluginInfoVersion;
            public PluginType Type;
            public string Name;
            public string Description;
            public string Author;
            public string TargetApplication;
            public short VersionMajor;
            public short VersionMinor;
            public short Revision;
            public short MinInterfaceVersion;
            public short MinApiRevision;
            public ReceiveNotificationFlags ReceiveNotifications;
            public int ConfigurationPanelHeight;
        }

        [Flags()]
        public enum ReceiveNotificationFlags
        {
            StartupOnly = 0x0,
            PlayerEvents = 0x1,
            DataStreamEvents = 0x2,
            TagEvents = 0x04,
            DownloadEvents = 0x08
        }

        public enum NotificationType
        {
            PluginStartup = 0,          // notification sent after successful initialisation for an enabled plugin
            TrackChanging = 16,
            TrackChanged = 1,
            PlayStateChanged = 2,
            AutoDjStarted = 3,
            AutoDjStopped = 4,
            VolumeMuteChanged = 5,
            VolumeLevelChanged = 6,
            NowPlayingListChanged = 7,
            NowPlayingListEnded = 18,
            NowPlayingArtworkReady = 8,
            NowPlayingLyricsReady = 9,
            TagsChanging = 10,
            TagsChanged = 11,
            RatingChanging = 15,
            RatingChanged = 12,
            PlayCountersChanged = 13,
            ScreenSaverActivating = 14,
            ShutdownStarted = 17,
            EmbedInPanel = 19,
            PlayerRepeatChanged = 20,
            PlayerShuffleChanged = 21,
            PlayerEqualiserOnOffChanged = 22,
            PlayerScrobbleChanged = 23,
            ReplayGainChanged = 24,
            FileDeleting = 25,
            FileDeleted = 26,
            ApplicationWindowChanged = 27,
            StopAfterCurrentChanged = 28,
            LibrarySwitched = 29,
            FileAddedToLibrary = 30,
            FileAddedToInbox = 31,
            SynchCompleted = 32,
            DownloadCompleted = 33,
            MusicBeeStarted = 34
        }

        public enum PluginCloseReason
        {
            MusicBeeClosing = 1,
            UserDisabled = 2,
            StopNoUnload = 3
        }

        public enum CallbackType
        {
            SettingsUpdated = 1,
            StorageReady = 2,
            StorageFailed = 3,
            FilesRetrievedChanged = 4,
            FilesRetrievedNoChange = 5,
            FilesRetrievedFail = 6,
            LyricsDownloaded = 7,
            StorageEject = 8,
            SuspendPlayCounters = 9,
            ResumePlayCounters = 10,
            EnablePlugin = 11,
            DisablePlugin = 12,
            RenderingDevicesChanged = 13,
            FullscreenOn = 14,
            FullscreenOff = 15
        }

        public enum FilePropertyType
        {
            Url = 2,
            Kind = 4,
            Format = 5,
            Size = 7,
            Channels = 8,
            SampleRate = 9,
            Bitrate = 10,
            DateModified = 11,
            DateAdded = 12,
            LastPlayed = 13,
            PlayCount = 14,
            SkipCount = 15,
            Duration = 16,
            Status = 21,
            NowPlayingListIndex = 78,  // only has meaning when called from NowPlayingList_* commands
            ReplayGainTrack = 94,
            ReplayGainAlbum = 95
        }

        public enum MetaDataType
        {
            TrackTitle = 65,
            Album = 30,
            AlbumArtist = 31,        // displayed album artist
            AlbumArtistRaw = 34,     // stored album artist
            Artist = 32,             // displayed artist
            MultiArtist = 33,        // individual artists, separated by a null char
			PrimaryArtist = 19,      // first artist from multi-artist tagged file, otherwise displayed artist
            Artists = 144,
            ArtistsWithArtistRole = 145,
            ArtistsWithPerformerRole = 146,
            ArtistsWithGuestRole = 147,
            ArtistsWithRemixerRole = 148,
            Artwork = 40,
            BeatsPerMin = 41,
            Composer = 43,           // displayed composer
            MultiComposer = 89,      // individual composers, separated by a null char
            Comment = 44,
            Conductor = 45,
            Custom1 = 46,
            Custom2 = 47,
            Custom3 = 48,
            Custom4 = 49,
            Custom5 = 50,
            Custom6 = 96,
            Custom7 = 97,
            Custom8 = 98,
            Custom9 = 99,
            Custom10 = 128,
            Custom11 = 129,
            Custom12 = 130,
            Custom13 = 131,
            Custom14 = 132,
            Custom15 = 133,
            Custom16 = 134,
            DiscNo = 52,
            DiscCount = 54,
            Encoder = 55,
            Genre = 59,
            Genres = 143,
            GenreCategory = 60,
            Grouping = 61,
            Keywords = 84,
            HasLyrics = 63,
            Lyricist = 62,
            Lyrics = 114,
            Mood = 64,
            Occasion = 66,
            Origin = 67,
            Publisher = 73,
            Quality = 74,
            Rating = 75,
            RatingLove = 76,
            RatingAlbum = 104,
            Tempo = 85,
            TrackNo = 86,
            TrackCount = 87,
            Virtual1 = 109,
            Virtual2 = 110,
            Virtual3 = 111,
            Virtual4 = 112,
            Virtual5 = 113,
            Virtual6 = 122,
            Virtual7 = 123,
            Virtual8 = 124,
            Virtual9 = 125,
            Virtual10 = 135,
            Virtual11 = 136,
            Virtual12 = 137,
            Virtual13 = 138,
            Virtual14 = 139,
            Virtual15 = 140,
            Virtual16 = 141,
            Year = 88
        }
        
        public enum FileCodec
        {
            Unknown = -1,
            Mp3 = 1,
            Aac = 2,
            Flac = 3,
            Ogg = 4,
            WavPack = 5,
            Wma = 6,
            Tak = 7,
            Mpc = 8,
            Wave = 9,
            Asx = 10,
            Alac = 11,
            Aiff = 12,
            Pcm = 13,
            Opus = 15,
            Spx = 16,
            Dsd = 17,
            AacNoContainer = 18
        }

        public enum EncodeQuality
        {
            SmallSize = 1,
            Portable = 2,
            HighQuality = 3,
            Archiving = 4
        }

        [Flags()]
        public enum LibraryCategory
        {
            Music = 0,
            Audiobook = 1,
            Video = 2,
            Inbox = 4
        }

        public enum DeviceIdType
        {
            GooglePlay = 1,
            AppleDevice = 2,
            GooglePlay2 = 3,
            AppleDevice2 = 4
        }

        public enum DataType
        {
            String = 0,
            Number = 1,
            DateTime = 2,
            Rating = 3
        }

        public enum SettingId
        {
            CompactPlayerFlickrEnabled = 1,
            FileTaggingPreserveModificationTime = 2,
            LastDownloadFolder = 3,
            ArtistGenresOnly = 4,
            IgnoreNamePrefixes = 5,
            IgnoreNameChars = 6,
            PlayCountTriggerPercent = 7,
            PlayCountTriggerSeconds = 8,
            SkipCountTriggerPercent = 9,
            SkipCountTriggerSeconds = 10,
            CustomWebLinkName1 = 11,
            CustomWebLinkName2 = 12,
            CustomWebLinkName3 = 13,
            CustomWebLinkName4 = 14,
            CustomWebLinkName5 = 15,
            CustomWebLinkName6 = 16,
            CustomWebLink1 = 17,
            CustomWebLink2 = 18,
            CustomWebLink3 = 19,
            CustomWebLink4 = 20,
            CustomWebLink5 = 21,
            CustomWebLink6 = 22,
            CustomWebLinkNowPlaying1 = 23,
            CustomWebLinkNowPlaying2 = 24,
            CustomWebLinkNowPlaying3 = 25,
            CustomWebLinkNowPlaying4 = 26,
            CustomWebLinkNowPlaying5 = 27,
            CustomWebLinkNowPlaying6 = 28
        }

        public enum ComparisonType
        {
            Is = 0,
            IsSimilar = 20
        }

        public enum LyricsType
        {
            NotSpecified = 0,
            Synchronised = 1,
            UnSynchronised = 2
        }

        public enum PlayState
        {
            Undefined = 0,
            Loading = 1,
            Playing = 3,
            Paused = 6,
            Stopped = 7
        }

        public enum RepeatMode
        {
            None = 0,
            All = 1,
            One = 2
        }

        public enum PlayButtonType
        {
            PreviousTrack = 0,
            PlayPause = 1,
            NextTrack = 2,
            Stop = 3
        }

        public enum PlaylistFormat
        {
            Unknown = 0,
            M3u = 1,
            Xspf = 2,
            Asx = 3,
            Wpl = 4,
            Pls = 5,
            Auto = 7,
            M3uAscii = 8,
            AsxFile = 9,
            Radio = 10,
            M3uExtended = 11,
            Mbp = 12
        }

        public enum SkinElement
        {
            SkinInputControl = 7,
            SkinInputPanel = 10,
            SkinInputPanelLabel = 14,
            SkinTrackAndArtistPanel = -1
        }

        public enum ElementState
        {
            ElementStateDefault = 0,
            ElementStateModified = 6
        }

        public enum ElementComponent
        {
            ComponentBorder = 0,
            ComponentBackground = 1,
            ComponentForeground = 3
        }

        public enum PluginPanelDock
        {
            ApplicationWindow = 0,
            TrackAndArtistPanel = 1,
            TextBox = 3,
            ComboBox = 4,
            MainPanel = 5
        }

        
        public enum ReplayGainMode
        {
            Off = 0,
            Track = 1,
            Album = 2,
            Smart = 3
        }
        
        public enum PlayStatisticType
        {
            NoChange = 0,
            IncreasePlayCount = 1,
            IncreaseSkipCount = 2
        }

        public enum Command
        {
            NavigateTo = 1
        }
        
        public enum DownloadTarget
        {
            Inbox = 0,
            MusicLibrary = 1,
            SpecificFolder = 3
        }

        [Flags()]
        public enum PictureLocations: byte
        {
            None = 0,
            EmbedInFile = 1,
            LinkToOrganisedCopy = 2,
            LinkToSource = 4,
            FolderThumb = 8
        }

        public enum WindowState
        {
            Off = -1,
            Normal = 0,
            Fullscreen = 1,
            Desktop = 2
        }

        public delegate void MB_ReleaseStringDelegate(string p1);
        public delegate void MB_TraceDelegate(string p1);
        public delegate IntPtr MB_WindowHandleDelegate();
        public delegate void MB_RefreshPanelsDelegate();
        public delegate void MB_SendNotificationDelegate(CallbackType type);
        public delegate System.Windows.Forms.ToolStripItem MB_AddMenuItemDelegate(string menuPath, string hotkeyDescription, EventHandler handler);
        public delegate bool MB_AddTreeNodeDelegate(string treePath, string name, System.Drawing.Bitmap icon, EventHandler openHandler, EventHandler closeHandler);
        public delegate void MB_RegisterCommandDelegate(string command, EventHandler handler);
        public delegate void MB_CreateBackgroundTaskDelegate(System.Threading.ThreadStart taskCallback, System.Windows.Forms.Form owner);
        public delegate void MB_CreateParameterisedBackgroundTaskDelegate(System.Threading.ParameterizedThreadStart taskCallback, object parameters, System.Windows.Forms.Form owner);
        public delegate void MB_SetBackgroundTaskMessageDelegate(string message);
        public delegate System.Drawing.Rectangle MB_GetPanelBoundsDelegate(PluginPanelDock dock);
        public delegate bool MB_SetPanelScrollableAreaDelegate(System.Windows.Forms.Control panel, System.Drawing.Size scrollArea, bool alwaysShowScrollBar);
        public delegate System.Windows.Forms.Control MB_AddPanelDelegate(System.Windows.Forms.Control panel, PluginPanelDock dock);
        public delegate void MB_RemovePanelDelegate(System.Windows.Forms.Control panel);
        public delegate string MB_GetLocalisationDelegate(string id, string defaultText);
        public delegate bool MB_ShowNowPlayingAssistantDelegate();
        public delegate bool MB_InvokeCommandDelegate(Command command, object parameter);
        public delegate bool MB_OpenFilterInTabDelegate(MetaDataType field1, ComparisonType comparison1, string value1, MetaDataType field2, ComparisonType comparison2, string value2);
        public delegate bool MB_SetWindowSizeDelegate(int width, int height);
        public delegate bool MB_DownloadFileDelegate(string url, DownloadTarget target, string targetFolder, bool cancelDownload);
        public delegate bool MB_GetVisualiserInformationDelegate(out string[] visualiserNames, out string defaultVisualiserName, out WindowState defaultState, out WindowState currentState);
        public delegate bool MB_ShowVisualiserDelegate(string visualiserName, WindowState state);
        public delegate bool MB_GetPluginViewInformationDelegate(string pluginFilename, out string[] viewNames, out string defaultViewName, out WindowState defaultState, out WindowState currentState);
        public delegate bool MB_ShowPluginViewDelegate(string pluginFilename, string viewName, WindowState state);
        public delegate bool MB_UninistallPluginDelegate(string pluginFilename, string password);
        public delegate string Setting_GetFieldNameDelegate(MetaDataType field);
        public delegate string Setting_GetPersistentStoragePathDelegate();
        public delegate string Setting_GetSkinDelegate();
        public delegate int Setting_GetSkinElementColourDelegate(SkinElement element, ElementState state, ElementComponent component);
        public delegate bool Setting_IsWindowBordersSkinnedDelegate();
        public delegate System.Drawing.Font Setting_GetDefaultFontDelegate();
        public delegate DataType Setting_GetDataTypeDelegate(MetaDataType field);
        public delegate string Setting_GetLastFmUserIdDelegate();
        public delegate string Setting_GetWebProxyDelegate();
        public delegate bool Setting_GetValueDelegate(SettingId settingId, ref object value);
        public delegate string Setting_GetFileConvertCommandLineDelegate(FileCodec codec, EncodeQuality encodeQuality);
        public delegate string Library_GetFilePropertyDelegate(string sourceFileUrl, FilePropertyType type);
        public delegate string Library_GetFileTagDelegate(string sourceFileUrl, MetaDataType field);
        public delegate bool Library_GetFileTagsDelegate(string sourceFileUrl, MetaDataType[] fields, ref string[] results);
        public delegate bool Library_SetFileTagDelegate(string sourceFileUrl, MetaDataType field, string value);
        public delegate string Library_GetDevicePersistentIdDelegate(string sourceFileUrl, DeviceIdType idType);
        public delegate bool Library_SetDevicePersistentIdDelegate(string sourceFileUrl, DeviceIdType idType, string value);
        public delegate bool Library_FindDevicePersistentIdDelegate(DeviceIdType idType, string[] ids, ref string[] values);
        public delegate bool Library_CommitTagsToFileDelegate(string sourceFileUrl);
        public delegate string Library_AddFileToLibraryDelegate(string sourceFileUrl, LibraryCategory category);
        public delegate bool Library_GetSyncDeltaDelegate(string[] cachedFiles, DateTime updatedSince, LibraryCategory categories, ref string[] newFiles, ref string[] updatedFiles, ref string[] deletedFiles);
        public delegate string Library_GetLyricsDelegate(string sourceFileUrl, LyricsType type);
        public delegate string Library_GetArtworkDelegate(string sourceFileUrl, int index);
        public delegate bool Library_GetArtworkExDelegate(string sourceFileUrl, int index, bool retrievePictureData, ref PictureLocations pictureLocations, ref string pictureUrl, ref byte[] imageData);
        public delegate bool Library_SetArtworkExDelegate(string sourceFileUrl, int index, byte[] imageData);
        public delegate string Library_GetArtistPictureDelegate(string artistName, int fadingPercent, int fadingColor);
        public delegate bool Library_GetArtistPictureUrlsDelegate(string artistName, bool localOnly, ref string[] urls);
        public delegate string Library_GetArtistPictureThumbDelegate(string artistName);
        public delegate bool Library_QueryFilesDelegate(string query);
        public delegate string Library_QueryGetNextFileDelegate();
        public delegate string Library_QueryGetAllFilesDelegate();
        public delegate bool Library_QueryFilesExDelegate(string query, ref string[] files);
        public delegate string Library_QuerySimilarArtistsDelegate(string artistName, double minimumArtistSimilarityRating);
        public delegate bool Library_QueryLookupTableDelegate(string keyTags, string valueTags, string query);
        public delegate string Library_QueryGetLookupTableValueDelegate(string key);
        public delegate int Player_GetPositionDelegate();
        public delegate bool Player_SetPositionDelegate(int position);
        public delegate PlayState Player_GetPlayStateDelegate();
        public delegate bool Player_GetButtonEnabledDelegate(PlayButtonType button);
        public delegate bool Player_ActionDelegate();
        public delegate int Player_QueueRandomTracksDelegate(int count);
        public delegate float Player_GetVolumeDelegate();
        public delegate bool Player_SetVolumeDelegate(float volume);
        public delegate bool Player_GetMuteDelegate();
        public delegate bool Player_SetMuteDelegate(bool mute);
        public delegate bool Player_GetShuffleDelegate();
        public delegate bool Player_SetShuffleDelegate(bool shuffle);
        public delegate RepeatMode Player_GetRepeatDelegate();
        public delegate bool Player_SetRepeatDelegate(RepeatMode repeat);
        public delegate bool Player_GetEqualiserEnabledDelegate();
        public delegate bool Player_SetEqualiserEnabledDelegate(bool enabled);
        public delegate bool Player_GetDspEnabledDelegate();
        public delegate bool Player_SetDspEnabledDelegate(bool enabled);
        public delegate bool Player_GetScrobbleEnabledDelegate();
        public delegate bool Player_SetScrobbleEnabledDelegate(bool enabled);
        public delegate bool Player_GetShowTimeRemainingDelegate();
        public delegate bool Player_GetShowRatingTrackDelegate();
        public delegate bool Player_GetShowRatingLoveDelegate();
        public delegate bool Player_ShowEqualiserDelegate();
        public delegate bool Player_GetAutoDjEnabledDelegate();
        public delegate bool Player_GetStopAfterCurrentEnabledDelegate();
        public delegate bool Player_GetCrossfadeDelegate();
        public delegate bool Player_SetCrossfadeDelegate(bool crossfade);
        public delegate ReplayGainMode Player_GetReplayGainModeDelegate();
        public delegate bool Player_SetReplayGainModeDelegate(ReplayGainMode mode);
        public delegate int Player_OpenStreamHandleDelegate(string url, bool useMusicBeeSettings, bool enableDsp, ReplayGainMode gainType);
        public delegate bool Player_UpdatePlayStatisticsDelegate(string url, PlayStatisticType countType, bool disableScrobble);
        public delegate bool Player_GetOutputDevicesDelegate(out string[] deviceNames, out string activeDeviceName);
        public delegate bool Player_SetOutputDeviceDelegate(string deviceName);
        public delegate string NowPlaying_GetFileUrlDelegate();
        public delegate int NowPlaying_GetDurationDelegate();
        public delegate string NowPlaying_GetFilePropertyDelegate(FilePropertyType type);
        public delegate string NowPlaying_GetFileTagDelegate(MetaDataType field);
        public delegate bool NowPlaying_GetFileTagsDelegate(MetaDataType[] fields, ref string[] results);
        public delegate string NowPlaying_GetLyricsDelegate();
        public delegate string NowPlaying_GetArtworkDelegate();
        public delegate string NowPlaying_GetArtistPictureDelegate(int fadingPercent);
        public delegate bool NowPlaying_GetArtistPictureUrlsDelegate(bool localOnly, ref string[] urls);
        public delegate string NowPlaying_GetArtistPictureThumbDelegate();
        public delegate bool NowPlaying_IsSoundtrackDelegate();
        public delegate int NowPlaying_GetSpectrumDataDelegate(float[] fftData);
        public delegate bool NowPlaying_GetSoundGraphDelegate(float[] graphData);
        public delegate int NowPlayingList_GetCurrentIndexDelegate();
        public delegate int NowPlayingList_GetNextIndexDelegate(int offset);
        public delegate bool NowPlayingList_IsAnyPriorTracksDelegate();
        public delegate bool NowPlayingList_IsAnyFollowingTracksDelegate();
        public delegate string NowPlayingList_GetFileUrlDelegate(int index);
        public delegate string NowPlayingList_GetFilePropertyDelegate(int index, FilePropertyType type);
        public delegate string NowPlayingList_GetFileTagDelegate(int index, MetaDataType field);
        public delegate bool NowPlayingList_GetFileTagsDelegate(int index, MetaDataType[] fields, ref string[] results);
        public delegate bool NowPlayingList_ActionDelegate();
        public delegate bool NowPlayingList_FileActionDelegate(string sourceFileUrl);
        public delegate bool NowPlayingList_FilesActionDelegate(string[] sourceFileUrl);
        public delegate bool NowPlayingList_RemoveAtDelegate(int index);
        public delegate bool NowPlayingList_MoveFilesDelegate(int[] fromIndices, int toIndex);
        public delegate string Playlist_GetNameDelegate(string playlistUrl);
        public delegate PlaylistFormat Playlist_GetTypeDelegate(string playlistUrl);
        public delegate bool Playlist_QueryPlaylistsDelegate();
        public delegate string Playlist_QueryGetNextPlaylistDelegate();
        public delegate bool Playlist_IsInListDelegate(string playlistUrl, string filename);
        public delegate bool Playlist_QueryFilesDelegate(string playlistUrl);
        public delegate bool Playlist_QueryFilesExDelegate(string playlistUrl, ref string[] filenames);
        public delegate string Playlist_CreatePlaylistDelegate(string folderName, string playlistName, string[] filenames);
        public delegate bool Playlist_DeletePlaylistDelegate(string playlistUrl);
        public delegate bool Playlist_SetFilesDelegate(string playlistUrl, string[] filenames);
        public delegate bool Playlist_AddFilesDelegate(string playlistUrl, string[] filenames);
        public delegate bool Playlist_RemoveAtDelegate(string playlistUrl, int index);
        public delegate bool Playlist_MoveFilesDelegate(string playlistUrl, int[] fromIndices, int toIndex);
        public delegate bool Playlist_PlayNowDelegate(string playlistUrl);
        public delegate string Pending_GetFileUrlDelegate();
        public delegate string Pending_GetFilePropertyDelegate(FilePropertyType field);
        public delegate string Pending_GetFileTagDelegate(MetaDataType field);
        public delegate string Sync_FileStartDelegate(string filename);
        public delegate void Sync_FileEndDelegate(string filename, bool success, string errorMessage);

        [System.Security.SuppressUnmanagedCodeSecurity()]
        [DllImport("kernel32.dll")]
        private static extern void CopyMemory(ref MusicBeeApiInterface mbApiInterface, IntPtr src, int length);
    }
}