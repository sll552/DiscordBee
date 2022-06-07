namespace MusicBeePlugin.UI
{
  using MusicBeePlugin.DiscordTools.Assets;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows.Forms;

  public partial class UploaderHealth : Form
  {
    private readonly List<IAssetUploader> _uploader;
    public UploaderHealth(List<IAssetUploader> uploader)
    {
      InitializeComponent();
      _uploader = uploader;
      VisibleChanged += UploaderHealth_VisibleChanged;
      FormClosing += UploaderHealth_FormClosing;
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
      var newData = new Dictionary<string, string>();

      foreach (var uploader in _uploader)
      {
        newData.Add(uploader.GetType().Name, $"Healthy: {uploader.IsHealthy()}");
      }

      dataGridView1.DataSource = null;
      dataGridView1.Columns.Clear();
      dataGridView1.Rows.Clear();
      dataGridView1.Refresh();

      dataGridView1.AutoGenerateColumns = true;
      dataGridView1.DataSource = newData.ToList().ToSortableBindingList();
      dataGridView1.Refresh();
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
}
