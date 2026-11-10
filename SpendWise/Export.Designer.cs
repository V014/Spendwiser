namespace SpendWise
{
    partial class Export
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Export));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btn_database = new System.Windows.Forms.Button();
            this.btn_transactions = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(20, 60);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btn_database);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btn_transactions);
            this.splitContainer1.Size = new System.Drawing.Size(348, 127);
            this.splitContainer1.SplitterDistance = 170;
            this.splitContainer1.TabIndex = 0;
            // 
            // btn_database
            // 
            this.btn_database.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btn_database.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_database.FlatAppearance.BorderSize = 0;
            this.btn_database.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_database.Font = new System.Drawing.Font("Arial", 14F);
            this.btn_database.ForeColor = System.Drawing.Color.White;
            this.btn_database.Image = ((System.Drawing.Image)(resources.GetObject("btn_database.Image")));
            this.btn_database.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_database.Location = new System.Drawing.Point(0, 0);
            this.btn_database.Name = "btn_database";
            this.btn_database.Size = new System.Drawing.Size(170, 127);
            this.btn_database.TabIndex = 0;
            this.btn_database.Text = "Database";
            this.btn_database.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_database.UseVisualStyleBackColor = false;
            this.btn_database.Click += new System.EventHandler(this.Btn_database_Click);
            // 
            // btn_transactions
            // 
            this.btn_transactions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.btn_transactions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_transactions.FlatAppearance.BorderSize = 0;
            this.btn_transactions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_transactions.Font = new System.Drawing.Font("Arial", 14F);
            this.btn_transactions.ForeColor = System.Drawing.Color.White;
            this.btn_transactions.Image = ((System.Drawing.Image)(resources.GetObject("btn_transactions.Image")));
            this.btn_transactions.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_transactions.Location = new System.Drawing.Point(0, 0);
            this.btn_transactions.Name = "btn_transactions";
            this.btn_transactions.Size = new System.Drawing.Size(174, 127);
            this.btn_transactions.TabIndex = 0;
            this.btn_transactions.Text = "Transactions";
            this.btn_transactions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_transactions.UseVisualStyleBackColor = false;
            this.btn_transactions.Click += new System.EventHandler(this.Btn_transactions_Click);
            // 
            // Export
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 207);
            this.Controls.Add(this.splitContainer1);
            this.MaximizeBox = false;
            this.Name = "Export";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Green;
            this.Text = "Export";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btn_database;
        private System.Windows.Forms.Button btn_transactions;
    }
}