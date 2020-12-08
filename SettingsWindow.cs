using System;
using System.Windows.Forms;

namespace MusicBeePlugin
{
  public partial class SettingsWindow : Form
  {
    private readonly Plugin _parent;
    private PlaceholderTableWindow _placeholderTableWindow;
    private readonly Settings _settings;
    private readonly Settings _defaultSettings;
    private bool _defaultsRestored;

    public SettingsWindow(Plugin parent, Settings settings)
    {
      _parent = parent;
      _settings = settings;
      _defaultSettings = new Settings();
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

    private void checkBoxAdvanced_CheckedChanged(object sender, EventArgs e)
    {
            if (checkBoxAdvanced.Checked)
            {
                this.MaximumSize = new System.Drawing.Size(100000000, 290);
                this.Height = 290;
            }
            else
            {
                this.MaximumSize = new System.Drawing.Size(100000000, 250);
                this.Height = 250;
            }
    }

    private void UpdateValues(Settings settings)
    {
      textBoxTrackNo.Text = settings.PresenceTrackNo;
      textBoxTrackCnt.Text = settings.PresenceTrackCnt;
      textBoxDetails.Text = settings.PresenceDetails;
      textBoxState.Text = settings.PresenceState;
      textBoxImage.Text = settings.ImageText;
      textBoxSeperator.Text = settings.Seperator;
      textBoxRpcId.Text = settings.RpcId;
      checkBoxPresenceUpdate.Checked = settings.UpdatePresenceWhenStopped;
    }

    private void buttonPlaceholders_Click(object sender, EventArgs e)
    {
      _placeholderTableWindow = new PlaceholderTableWindow();
      _placeholderTableWindow.UpdateTable(_parent.GenerateMetaDataDictionary());
      _placeholderTableWindow.Show(this);
    }

    private void buttonRestoreDefaults_Click(object sender, EventArgs e)
    {
      UpdateValues(_defaultSettings);
      _defaultsRestored = true;
    }

    private void buttonSaveClose_Click(object sender, EventArgs e)
    {
      if (textBoxTrackNo.Text != _defaultSettings.PresenceTrackNo)
      {
        _settings.PresenceTrackNo = textBoxTrackNo.Text;
        _defaultsRestored = false;
      }

      if (textBoxTrackCnt.Text != _defaultSettings.PresenceTrackCnt)
      {
        _settings.PresenceTrackCnt = textBoxTrackCnt.Text;
        _defaultsRestored = false;
      }

      if (textBoxDetails.Text != _defaultSettings.PresenceDetails)
      {
        _settings.PresenceDetails = textBoxDetails.Text;
        _defaultsRestored = false;
      }

      if (textBoxState.Text != _defaultSettings.PresenceState)
      {
        _settings.PresenceState = textBoxState.Text;
        _defaultsRestored = false;
      }

      if (textBoxImage.Text != _defaultSettings.ImageText)
      {
        _settings.ImageText = textBoxImage.Text;
        _defaultsRestored = false;
      }

      if (textBoxSeperator.Text != _defaultSettings.Seperator)
      {
        _settings.Seperator = textBoxSeperator.Text;
        _defaultsRestored = false;
      }

      if (textBoxRpcId.Text != _defaultSettings.RpcId)
      {
          _settings.RpcId = textBoxRpcId.Text;
          _defaultsRestored = false;
      }

      _settings.UpdatePresenceWhenStopped = checkBoxPresenceUpdate.Checked;

      if (_defaultsRestored)
      {
        _settings.Delete();
        _defaultsRestored = false;
      }

      _settings.Save();
      Hide();
    }
  }
}