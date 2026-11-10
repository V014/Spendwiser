using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace SpendWise
{
    public partial class Growth : Form
    {
        private readonly Dashboard dashboard = new Dashboard();
        private readonly Connection con = new Connection();
        public Growth()
        {
            InitializeComponent();
            try
            {
                // pull established year from dashboard
                int established = dashboard.LoadEstablished();
                // pull current year from dashborad
                int yearNow = dashboard.LoadYear();
                // calculates years in service
                int years = yearNow - established;
                // display beginning value
                lbl_bv.Text = dashboard.LoadCapital().ToString();
                // display ending value
                lbl_ev.Text = dashboard.LoadRevenue().ToString();
                // display years
                lbl_years.Text = years.ToString();
                // display compound annual growth rate
                lbl_CAGR.Text = dashboard.LoadGrowth() + "%";
            }
            catch (Exception)
            {
                // play chime
                SoundPlayer chime = new SoundPlayer(@"sfx/warning.wav");
                chime.Play();

                MessageBox.Show("Data unavilable!", "Assistant");
                // log the error
                con.ExecuteQuery("INSERT INTO events (date, Description, Location) VALUES(strftime('%Y-%m-%d %H:%M', 'now', 'localtime'), 'SQL error', 'Growth action')");
            }
            PreviousTheme();
            
        }
        private void PreviousTheme()
        {
            try
            {
                string UserTheme = con.ReadString("SELECT Theme FROM wallet");
                if (UserTheme == "Light")
                {
                    this.BackColor = Color.White;
                    lbl_title_CAGR.ForeColor = Color.Black;
                    lbl_CAGR.ForeColor = Color.Black;
                    lbl_title_bv.ForeColor = Color.Black;
                    lbl_title_ev.ForeColor = Color.Black;
                    lbl_title_years.ForeColor = Color.Black;
                    lbl_bv.ForeColor = Color.Black;
                    lbl_ev.ForeColor = Color.Black;
                    lbl_years.ForeColor = Color.Black;
                }
                else if (UserTheme == "Dark")
                {
                    this.BackColor = Color.FromArgb(17, 17, 17);
                    lbl_title_CAGR.ForeColor = Color.White;
                    lbl_CAGR.ForeColor = Color.White;
                    lbl_title_bv.ForeColor = Color.White;
                    lbl_title_ev.ForeColor = Color.White;
                    lbl_title_years.ForeColor = Color.White;
                    lbl_bv.ForeColor = Color.White;
                    lbl_ev.ForeColor = Color.White;
                    lbl_years.ForeColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
