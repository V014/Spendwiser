using System.Drawing;
using System.Windows.Forms;

namespace SpendWise
{
    class StyleDataGrid
    {
        // these values dont change make a function and call them at will
        public void gridSettings(DataGridView dataGrid)
        {
            dataGrid.BorderStyle = BorderStyle.None;
            dataGrid.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGrid.EnableHeadersVisualStyles = false;
            dataGrid.ColumnHeadersHeight = 30;
            dataGrid.RowHeadersVisible = false;
        }
        // styling of the datagrid
        public void DarkStyle(DataGridView dataGrid)
        {
            // call similar properties
            gridSettings(dataGrid);

            dataGrid.BackgroundColor = Color.FromArgb(17, 17, 17);
            
            dataGrid.AlternatingRowsDefaultCellStyle.Font = new Font("Roboto", 8);
            dataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24,24,24);
            dataGrid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Gray;

            dataGrid.RowsDefaultCellStyle.BackColor = Color.FromArgb(17, 17, 17);
            dataGrid.RowsDefaultCellStyle.ForeColor = Color.Gray;

            dataGrid.DefaultCellStyle.Font = new Font("Roboto", 8);
            dataGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 177, 89);

            dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 17, 17);
            dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Roboto", 11);
            dataGrid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            dataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(17, 17, 17);
        }

        // styling of the datagrid
        public void LightStyle(DataGridView dataGrid)
        {
            // call similar properties
            gridSettings(dataGrid);

            dataGrid.BackgroundColor = Color.White;

            dataGrid.AlternatingRowsDefaultCellStyle.Font = new Font("Roboto", 8);
            dataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
            dataGrid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            dataGrid.RowsDefaultCellStyle.BackColor = Color.White;
            dataGrid.RowsDefaultCellStyle.ForeColor = Color.Black;

            dataGrid.DefaultCellStyle.Font = new Font("Roboto", 8);
            dataGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 177, 89);

            dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Roboto", 11);
            dataGrid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;
            dataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
        }
    }
}
