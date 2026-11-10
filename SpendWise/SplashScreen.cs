using System;
using System.Windows.Forms;

namespace SpendWise
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void SplashScreen_Shown(object sender, EventArgs e)
        {
            time = new Timer();
            time.Interval = 1000;
            time.Start();
            time.Tick += new EventHandler(time_Tick);
        }
        void time_Tick(object sender, EventArgs e)
        {
            time.Stop();
            Dashboard home = new Dashboard();
            home.Show();
            this.Hide();
        }
    }
}
