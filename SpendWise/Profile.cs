using System;
using System.Drawing;
using System.Media;
using System.Security.Principal;
using System.Windows.Forms;

namespace SpendWise
{
    public partial class Profile : Form
    {
        readonly Connection con = new Connection();
        public Profile()
        {
            InitializeComponent();
            txt_name.Text = WindowsIdentity.GetCurrent().Name;
            // load theme
            PreviousTheme();
        }

        private void Btn_submit_Click(object sender, EventArgs e)
        {
            string name = txt_name.Text;
            try
            { 
                con.ExecuteQuery($"UPDATE wallet SET owner = '{name}'");
                MessageBox.Show("Profile updated!", "Assistant");
            }
            catch
            {
                // play chime
                SoundPlayer save = new SoundPlayer(@"sfx/error.wav");
                save.Play();
                // log the error
                con.ExecuteQuery("INSERT INTO events (date, description, location) VALUES(strftime('%Y-%m-%d %H:%M', 'now', 'localtime'), 'SQL error', 'Profile')");
                MessageBox.Show("Feature unavailable", "Assistant");
            }
        }
        private void PreviousTheme()
        {
            try
            {
                string UserTheme = con.ReadString("SELECT Theme FROM wallet");
                if (UserTheme == "Light")
                {
                    this.BackColor = Color.White;
                    lbl_title.ForeColor = Color.Black;
                    txt_name.BackColor = Color.White;
                    btn_save.BackColor = Color.FromArgb(72, 174, 120);
                }
                else if (UserTheme == "Dark")
                {
                    this.BackColor = Color.FromArgb(17, 17, 17);
                    lbl_title.ForeColor = Color.White;
                    txt_name.BackColor = Color.FromArgb(17, 17, 17);
                    btn_save.BackColor = Color.FromArgb(43, 140, 94);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
