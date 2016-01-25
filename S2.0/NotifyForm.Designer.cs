namespace S2_0
{
    partial class NotifyForm
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
            this.title = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.content = new System.Windows.Forms.TextBox();
            this.Close = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(12, 9);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(38, 20);
            this.title.TabIndex = 0;
            this.title.Text = "Title";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(16, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(111, 107);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // content
            // 
            this.content.BackColor = System.Drawing.Color.Teal;
            this.content.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.content.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.content.ForeColor = System.Drawing.Color.AliceBlue;
            this.content.Location = new System.Drawing.Point(151, 39);
            this.content.Multiline = true;
            this.content.Name = "content";
            this.content.Size = new System.Drawing.Size(310, 107);
            this.content.TabIndex = 2;
            this.content.Text = "EAWJ;LGJR;AJG;AJG;FSJGK;LJDFS;LGJDFSK;LJG;LKDFSJG;LKDFSJLKG;JJJJJJJJJJJJJLKDFSJLG" +
    "JDFLJG";
            this.content.TextChanged += new System.EventHandler(this.content_TextChanged);
            // 
            // Close
            // 
            this.Close.AutoSize = true;
            this.Close.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Close.Location = new System.Drawing.Point(442, 7);
            this.Close.Name = "Close";
            this.Close.Size = new System.Drawing.Size(20, 22);
            this.Close.TabIndex = 3;
            this.Close.Text = "X";
            this.Close.Click += new System.EventHandler(this.Close_Click);
            this.Close.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Close_MouseClick);
            this.Close.MouseHover += new System.EventHandler(this.Close_MouseHover);
            // 
            // NotifyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.ClientSize = new System.Drawing.Size(473, 158);
            this.Controls.Add(this.Close);
            this.Controls.Add(this.content);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.title);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NotifyForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "NotifyForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.NotifyForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label title;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox content;
        private System.Windows.Forms.Label Close;
    }
}