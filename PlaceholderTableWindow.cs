namespace MusicBeePlugin
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows.Forms;

  public partial class PlaceholderTableWindow : Form
  {
    public PlaceholderTableWindow()
    {
      InitializeComponent();
    }

    public void UpdateTable(Dictionary<string, string> newData)
    {
      dataGridView1.DataSource = null;
      dataGridView1.Columns.Clear();
      dataGridView1.Rows.Clear();
      dataGridView1.Refresh();

      dataGridView1.AutoGenerateColumns = true;
      dataGridView1.DataSource = newData.ToList().ToSortableBindingList();
      dataGridView1.Refresh();
      var lastcol =
        dataGridView1.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
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
