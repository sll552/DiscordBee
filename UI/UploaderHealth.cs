namespace MusicBeePlugin.UI
{
  using MusicBeePlugin.DiscordTools.Assets;
  using System.Collections.Generic;
  using System.Windows.Forms;

  public partial class UploaderHealth : Form
  {
    private readonly List<IAssetUploader> _uploader;
    private readonly BindingSource _source = new BindingSource();
    public UploaderHealth(List<IAssetUploader> uploader)
    {
      InitializeComponent();
      _uploader = uploader;

      SetupGridView();

      VisibleChanged += UploaderHealth_VisibleChanged;
      FormClosing += UploaderHealth_FormClosing;
    }

    private void SetupGridView()
    {
      dataGridView1.AutoGenerateColumns = true;
      dataGridView1.DataSource = _source;
    }

    private void UploaderHealth_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason != CloseReason.UserClosing)
      {
        return;
      }

      Hide();
      e.Cancel = true;
    }

    private void UploaderHealth_VisibleChanged(object sender, System.EventArgs e)
    {
      if (Visible)
      {
        RefreshStatus();
      }
    }

    private void buttonRefresh_Click(object sender, System.EventArgs e)
    {
      RefreshStatus();
    }
    private void RefreshStatus()
    {
      _source.Clear();

      foreach (var uploader in _uploader)
      {
        UploaderHealthInfo uploaderHealthInfo = uploader.GetHealth();
        _source.Add(new UploaderHealthState(uploader.GetType().Name, uploaderHealthInfo.IsHealthy, string.Join(";\n", uploaderHealthInfo.AdditionalInfo)));
      }

      var lastcol = dataGridView1.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
      if (lastcol != null)
      {
        lastcol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      }

      foreach (DataGridViewColumn col in dataGridView1.Columns)
      {
        col.SortMode = DataGridViewColumnSortMode.Automatic;
      }
    }
  }

  internal class UploaderHealthState
  {
    public UploaderHealthState(string name, bool isHealthy, string info)
    {
      Name = name;
      Healthy = isHealthy;
      Info = info;
    }

    public string Name { get; }
    public bool Healthy { get; }
    public string Info { get; }
  }
}
