using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpendWise
{
    public partial class Expenditure : Form
    {
        readonly Connection con = new Connection();
        readonly StyleDataGrid styleGrid = new StyleDataGrid();
        readonly Currency currency = new Currency();

        public Expenditure()
        {
            InitializeComponent();
            LoadInvestments();
            PreviousTheme();
        }
        // load things that cost the user
        private void LoadInvestments()
        {
            try
            {
                con.LoadData($"SELECT Description, COUNT(description) AS Occured, SUM(Amount) AS Amount FROM transactions WHERE action = '-' GROUP BY Description ORDER BY Amount DESC", Data_expenditure);
                styleGrid.DarkStyle(Data_expenditure);
                lbl_expenditure.Text = "Total expenditure : " + currency.LoadCurrency() + LoadExpenditure();
            }
            catch (Exception)
            {
                MessageBox.Show("Suggestion list unavailable!", "Assistant");
            }
        }
        // load the overall expenditure
        private string LoadExpenditure()
        {
            try
            {
                string Expenditure = con.ReadString("SELECT sum(amount) FROM transactions WHERE action = '-'");
                return Expenditure;
            }
            catch (Exception)
            {
                return 0.ToString();
            }

        }
        // load previous theme selected by user
        private void PreviousTheme()
        {
            try
            {
                string UserTheme = con.ReadString("SELECT Theme FROM wallet");
                if (UserTheme == "Light")
                {
                    this.BackColor = Color.White;
                    styleGrid.LightStyle(Data_expenditure);
                }
                else if (UserTheme == "Dark")
                {
                    this.BackColor = Color.FromArgb(17, 17, 17);
                    styleGrid.DarkStyle(Data_expenditure);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
