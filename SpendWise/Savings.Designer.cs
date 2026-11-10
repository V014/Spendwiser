namespace SpendWise
{
    partial class Savings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl_title = new System.Windows.Forms.Label();
            this.lbl_percent = new System.Windows.Forms.Label();
            this.scrollbar_savings_percentage = new MetroFramework.Controls.MetroScrollBar();
            this.panel_controls = new System.Windows.Forms.SplitContainer();
            this.btn_apply = new System.Windows.Forms.Button();
            this.btn_reset = new System.Windows.Forms.Button();
            this.lbl_saved = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.panel_controls)).BeginInit();
            this.panel_controls.Panel1.SuspendLayout();
            this.panel_controls.Panel2.SuspendLayout();
            this.panel_controls.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.AutoSize = true;
            this.lbl_title.Font = new System.Drawing.Font("Roboto", 16F);
            this.lbl_title.ForeColor = System.Drawing.Color.White;
            this.lbl_title.Location = new System.Drawing.Point(11, 37);
            this.lbl_title.Name = "lbl_title";
            this.lbl_title.Size = new System.Drawing.Size(124, 27);
            this.lbl_title.TabIndex = 3;
            this.lbl_title.Text = "Percentage";
            // 
            // lbl_percent
            // 
            this.lbl_percent.AutoSize = true;
            this.lbl_percent.Font = new System.Drawing.Font("Roboto", 66F);
            this.lbl_percent.ForeColor = System.Drawing.Color.White;
            this.lbl_percent.Location = new System.Drawing.Point(12, 64);
            this.lbl_percent.Name = "lbl_percent";
            this.lbl_percent.Size = new System.Drawing.Size(158, 106);
            this.lbl_percent.TabIndex = 3;
            this.lbl_percent.Text = "0%";
            // 
            // scrollbar_savings_percentage
            // 
            this.scrollbar_savings_percentage.LargeChange = 10;
            this.scrollbar_savings_percentage.Location = new System.Drawing.Point(12, 173);
            this.scrollbar_savings_percentage.Maximum = 100;
            this.scrollbar_savings_percentage.Minimum = 0;
            this.scrollbar_savings_percentage.MouseWheelBarPartitions = 10;
            this.scrollbar_savings_percentage.Name = "scrollbar_savings_percentage";
            this.scrollbar_savings_percentage.Orientation = MetroFramework.Controls.MetroScrollOrientation.Horizontal;
            this.scrollbar_savings_percentage.ScrollbarSize = 10;
            this.scrollbar_savings_percentage.Size = new System.Drawing.Size(227, 10);
            this.scrollbar_savings_percentage.Style = MetroFramework.MetroColorStyle.Green;
            this.scrollbar_savings_percentage.TabIndex = 6;
            this.scrollbar_savings_percentage.Theme = MetroFramework.MetroThemeStyle.Light;
            this.scrollbar_savings_percentage.UseSelectable = true;
            this.scrollbar_savings_percentage.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Scrollbar_savings_percentage_Scroll);
            // 
            // panel_controls
            // 
            this.panel_controls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_controls.Location = new System.Drawing.Point(0, 196);
            this.panel_controls.Name = "panel_controls";
            // 
            // panel_controls.Panel1
            // 
            this.panel_controls.Panel1.Controls.Add(this.btn_apply);
            // 
            // panel_controls.Panel2
            // 
            this.panel_controls.Panel2.Controls.Add(this.btn_reset);
            this.panel_controls.Size = new System.Drawing.Size(262, 40);
            this.panel_controls.SplitterDistance = 132;
            this.panel_controls.TabIndex = 7;
            // 
            // btn_apply
            // 
            this.btn_apply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btn_apply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_apply.FlatAppearance.BorderSize = 0;
            this.btn_apply.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MediumSeaGreen;
            this.btn_apply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_apply.Font = new System.Drawing.Font("Roboto", 11F);
            this.btn_apply.ForeColor = System.Drawing.Color.White;
            this.btn_apply.Location = new System.Drawing.Point(0, 0);
            this.btn_apply.Name = "btn_apply";
            this.btn_apply.Size = new System.Drawing.Size(132, 40);
            this.btn_apply.TabIndex = 5;
            this.btn_apply.Text = "Apply";
            this.btn_apply.UseVisualStyleBackColor = false;
            this.btn_apply.Click += new System.EventHandler(this.Btn_apply_Click);
            // 
            // btn_reset
            // 
            this.btn_reset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.btn_reset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_reset.FlatAppearance.BorderSize = 0;
            this.btn_reset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btn_reset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_reset.Font = new System.Drawing.Font("Roboto", 11F);
            this.btn_reset.ForeColor = System.Drawing.Color.White;
            this.btn_reset.Location = new System.Drawing.Point(0, 0);
            this.btn_reset.Name = "btn_reset";
            this.btn_reset.Size = new System.Drawing.Size(126, 40);
            this.btn_reset.TabIndex = 5;
            this.btn_reset.Text = "Reset";
            this.btn_reset.UseVisualStyleBackColor = false;
            this.btn_reset.Click += new System.EventHandler(this.Btn_reset_Click);
            // 
            // lbl_saved
            // 
            this.lbl_saved.AutoSize = true;
            this.lbl_saved.Font = new System.Drawing.Font("Roboto Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_saved.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(174)))), ((int)(((byte)(120)))));
            this.lbl_saved.Location = new System.Drawing.Point(12, 9);
            this.lbl_saved.Name = "lbl_saved";
            this.lbl_saved.Size = new System.Drawing.Size(59, 19);
            this.lbl_saved.TabIndex = 3;
            this.lbl_saved.Text = "Saved :";
            // 
            // Savings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.ClientSize = new System.Drawing.Size(262, 236);
            this.Controls.Add(this.panel_controls);
            this.Controls.Add(this.scrollbar_savings_percentage);
            this.Controls.Add(this.lbl_percent);
            this.Controls.Add(this.lbl_saved);
            this.Controls.Add(this.lbl_title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Savings";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Savings";
            this.panel_controls.Panel1.ResumeLayout(false);
            this.panel_controls.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panel_controls)).EndInit();
            this.panel_controls.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbl_title;
        private System.Windows.Forms.Label lbl_percent;
        private MetroFramework.Controls.MetroScrollBar scrollbar_savings_percentage;
        private System.Windows.Forms.SplitContainer panel_controls;
        private System.Windows.Forms.Button btn_apply;
        private System.Windows.Forms.Button btn_reset;
        private System.Windows.Forms.Label lbl_saved;
    }
}