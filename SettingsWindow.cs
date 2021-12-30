namespace MusicBeePlugin
{
  using System;
  using System.Windows.Forms;

  public partial class SettingsWindow : Form
  {
    private readonly Plugin _parent;
    private PlaceholderTableWindow _placeholderTableWindow;
    private readonly Settings _settings;
    private bool _defaultsRestored;

    public SettingsWindow(Plugin parent, Settings settings)
    {
      _parent = parent;
      _settings = settings;
      InitializeComponent();
      UpdateValues(_settings);
      Text += " (v" + parent.GetVersionString() + ")";

      FormClosing += OnFormClosing;
      Shown += OnShown;
      VisibleChanged += OnVisibleChanged;
    }

    private void OnVisibleChanged(object sender, EventArgs eventArgs)
    {
      if (Visible)
      {
        UpdateValues(_settings);
      }
    }

    private void OnShown(object sender, EventArgs eventArgs)
    {
      UpdateValues(_settings);
    }

    private void OnFormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason != CloseReason.UserClosing) return;
      Hide();
      e.Cancel = true;
    }

    private void UpdateValues(Settings settings)
    {
      textBoxTrackNo.Text = settings.PresenceTrackNo;
      textBoxTrackCnt.Text = settings.PresenceTrackCnt;
      textBoxDetails.Text = settings.PresenceDetails;
      textBoxState.Text = settings.PresenceState;
      textBoxLargeImage.Text = settings.LargeImageText;
      textBoxSmallImage.Text = settings.SmallImageText;
      textBoxSeperator.Text = settings.Seperator;
      textBoxDiscordAppId.Text = settings.DiscordAppId.Equals(Settings.defaults["DiscordAppId"]) ? "" : settings.DiscordAppId;
      checkBoxPresenceUpdate.Checked = settings.UpdatePresenceWhenStopped;
      checkBoxShowTime.Checked = settings.ShowTime;
      checkBoxShowRemainingTime.Checked = settings.ShowRemainingTime;
      checkBoxTextOnly.Checked = settings.TextOnly;
      checkBoxShowPlayState.Checked = settings.ShowPlayState;
      checkBoxShowOnlyNonPlayingState.Checked = settings.ShowOnlyNonPlayingState;
      checkBoxArtworkUpload.Checked = settings.UploadArtwork;

      ValidateInputs();
    }

    private void buttonPlaceholders_Click(object sender, EventArgs e)
    {
      _placeholderTableWindow = new PlaceholderTableWindow();
      _placeholderTableWindow.UpdateTable(_parent.GenerateMetaDataDictionary());
      _placeholderTableWindow.Show(this);
    }

    private void buttonRestoreDefaults_Click(object sender, EventArgs e)
    {
      _settings.Clear();
      UpdateValues(_settings);
      _defaultsRestored = true;
    }

    private void buttonSaveClose_Click(object sender, EventArgs e)
    {
      if (!ValidateInputs())
      {
        return;
      }

      _settings.PresenceTrackNo = textBoxTrackNo.Text;
      _settings.PresenceTrackCnt = textBoxTrackCnt.Text;
      _settings.PresenceDetails = textBoxDetails.Text;
      _settings.PresenceState = textBoxState.Text;
      _settings.LargeImageText = textBoxLargeImage.Text;
      _settings.SmallImageText = textBoxSmallImage.Text;
      _settings.Seperator = textBoxSeperator.Text;
      _settings.DiscordAppId = string.IsNullOrWhiteSpace(textBoxDiscordAppId.Text) ? null : textBoxDiscordAppId.Text;
      _settings.UpdatePresenceWhenStopped = checkBoxPresenceUpdate.Checked;
      _settings.ShowTime = checkBoxShowTime.Checked;
      _settings.ShowRemainingTime = checkBoxShowRemainingTime.Checked;
      _settings.TextOnly = checkBoxTextOnly.Checked;
      _settings.ShowPlayState = checkBoxShowPlayState.Checked;
      _settings.ShowOnlyNonPlayingState = checkBoxShowOnlyNonPlayingState.Checked;
      _settings.UploadArtwork = checkBoxArtworkUpload.Checked;

      if (_defaultsRestored && !_settings.IsDirty)
      {
        _settings.Delete();
        _defaultsRestored = false;
      }

      _settings.Save();
      Hide();
    }

    private bool ValidateInputs()
    {
      bool ContainsDigitsOnly(string s)
      {
        foreach (char c in s)
        {
          if (c < '0' || c > '9')
            return false;
        }
        return true;
      }

      bool validateDiscordId()
      {
        if (textBoxDiscordAppId.Text.Length != Settings.defaults["DiscordAppId"].Length
          || textBoxDiscordAppId.Text.Equals(Settings.defaults["DiscordAppId"])
          || !ContainsDigitsOnly(textBoxDiscordAppId.Text))
        {
          textBoxDiscordAppId.BackColor = System.Drawing.Color.PaleVioletRed;
          return false;
        }
        textBoxDiscordAppId.BackColor = System.Drawing.Color.White;
        return true;
      }

      if (checkBoxArtworkUpload.Checked && !validateDiscordId())
      {
        return false;
      }

      if (textBoxDiscordAppId.Text.Length > 0 && !validateDiscordId())
      {
        return false;
      }

      ResetErrorIndications();

      return true;
    }

    private void ResetErrorIndications()
    {
      textBoxDiscordAppId.BackColor = System.Drawing.Color.White;
    }

    private void textBoxDiscordAppId_TextChanged(object sender, EventArgs e)
    {
      ValidateInputs();
    }

    private void checkBoxArtworkUpload_CheckedChanged(object sender, EventArgs e)
    {
      ValidateInputs();
    }
  }
}
