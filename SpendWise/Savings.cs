using System;
using System.Data;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace SpendWise
{
    public partial class Savings : Form
    {
        readonly Connection con = new Connection();
        readonly Money money = new Money();
        public Savings()
        {
            InitializeComponent();
            string percent = con.ReadString("SELECT save FROM wallet");
            lbl_percent.Text = percent + "%";
            scrollbar_savings_percentage.Value = int.Parse(percent);
            // display current money save
            lbl_saved.Text = "Money saved: " + money.CheckSavings();
            // check active theme
            PreviousTheme();
        }
        // update the database with new savings amount
        private void Btn_apply_Click(object sender, EventArgs e)
        {
            try
            {
                int percentage = scrollbar_savings_percentage.Value;
                con.ExecuteQuery($"UPDATE wallet SET save = '{percentage}'");
                lbl_title.Text = "Change saved!";
                lbl_title.ForeColor = Color.MediumSeaGreen;
            }
            catch (Exception)
            {
                lbl_title.Text = "Error!";
                lbl_title.ForeColor = Color.Crimson;
            }
            
        }
        // update the lable when scroll bar is scrubbed
        private void Scrollbar_savings_percentage_Scroll(object sender, ScrollEventArgs e)
        {
            lbl_percent.Text = scrollbar_savings_percentage.Value.ToString() + "%";
        }
        // reset percentage to zero
        private void Btn_reset_Click(object sender, EventArgs e)
        {
            try
            {
                // play chime
                SoundPlayer chime = new SoundPlayer(@"sfx/glass.wav");
                chime.Play();
                DialogResult dialogResult = MessageBox.Show("Resetting the savings will clear all your savings history, are you sure?", "Reset Savings?", MessageBoxButtons.YesNo);
            
                if (dialogResult == DialogResult.Yes)
                {
                    // play chime
                    SoundPlayer chime2 = new SoundPlayer(@"sfx/pulse.wav");
                    chime2.Play();
                    // update wallet
                    con.ExecuteQuery("UPDATE wallet SET savings = 0");
                    MessageBox.Show("Savings emptied!", "Assistant");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong, bug recorded!!", "Assistant");
            }
        }
        // check Previous theme
        private void PreviousTheme()
        {
            try
            {
                string UserTheme = con.ReadString("SELECT Theme FROM wallet");
                if (UserTheme == "Light")
                {
                    this.BackColor = Color.White;
                    lbl_title.ForeColor = Color.Black;
                    lbl_percent.ForeColor = Color.Black;
                    scrollbar_savings_percentage.Theme = MetroFramework.MetroThemeStyle.Light;
                    btn_apply.BackColor = Color.FromArgb(72, 174, 120);
                    btn_reset.BackColor = Color.SteelBlue;
                }
                else if (UserTheme == "Dark")
                {
                    this.BackColor = Color.FromArgb(17, 17, 17);
                    lbl_title.ForeColor = Color.White;
                    lbl_percent.ForeColor = Color.White;
                    scrollbar_savings_percentage.Theme = MetroFramework.MetroThemeStyle.Dark;
                    btn_apply.BackColor = Color.FromArgb(20, 30, 30);
                    btn_reset.BackColor = Color.FromArgb(20, 30, 40);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong, bug recorded!", "Assistant");
            }
        }
    }
}
