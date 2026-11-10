using ClosedXML.Excel;
using System;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace SpendWise
{
    public partial class Export : MetroFramework.Forms.MetroForm
    {
        readonly Connection con = new Connection();
        // constructor
        public Export()
        {
            InitializeComponent();
        }
        // exports a microsoft excel xlsx document  
        private void Btn_transactions_Click(object sender, EventArgs e)
        {
            // display UI that enable user to save file location
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // collect all the known transactions
                        string queryExport = "SELECT ID, Description, Amount, Action, Date FROM transactions";
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            // export them to an excel file Note. DataTable required
                            workbook.Worksheets.Add(con.ExportData(queryExport));
                            workbook.SaveAs(sfd.FileName);
                        }
                        // play chime
                        SoundPlayer ice = new SoundPlayer(@"sfx/ice.wav");
                        ice.Play();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        // exports an sqlite db
        private void Btn_database_Click(object sender, EventArgs e)
        {
            // display UI that enable user to save file location
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "SQLitedb|*.db" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string sourcePath = @"SpendWise.db";
                        if (!File.Exists(sourcePath))
                        {
                            File.Copy(sourcePath, sfd.FileName);
                        }
                        else
                        {
                            File.Delete(sfd.FileName);
                            File.Copy(sourcePath, sfd.FileName);
                        }
                        // play chime
                        SoundPlayer ice = new SoundPlayer(@"sfx/ice.wav");
                        ice.Play();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Feature unavailable!", "Assistant", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        con.ExecuteQuery($"INSERT INTO events (description, location, date) VALUES('Export function error', 'Export button'. 'date('now')')");
                    }
                }
            }
        }
    }
}
