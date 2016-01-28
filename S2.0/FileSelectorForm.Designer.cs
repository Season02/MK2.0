namespace S2_0
{
    partial class FileSelectorForm
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
            this.host_lv = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // host_lv
            // 
            this.host_lv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.host_lv.BackColor = System.Drawing.SystemColors.Window;
            this.host_lv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.host_lv.Location = new System.Drawing.Point(0, 0);
            this.host_lv.Name = "host_lv";
            this.host_lv.Size = new System.Drawing.Size(435, 304);
            this.host_lv.TabIndex = 0;
            this.host_lv.UseCompatibleStateImageBehavior = false;
            this.host_lv.View = System.Windows.Forms.View.Details;
            this.host_lv.Click += new System.EventHandler(this.host_lv_Click);
            // 
            // FileSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 302);
            this.Controls.Add(this.host_lv);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FileSelectorForm";
            this.ShowIcon = false;
            this.Text = "FileSelectForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView host_lv;

    }
}