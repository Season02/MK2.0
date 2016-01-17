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
            this.host_lv.BackColor = System.Drawing.SystemColors.Info;
            this.host_lv.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.host_lv.Location = new System.Drawing.Point(34, 42);
            this.host_lv.Name = "host_lv";
            this.host_lv.Size = new System.Drawing.Size(388, 254);
            this.host_lv.TabIndex = 0;
            this.host_lv.UseCompatibleStateImageBehavior = false;
            this.host_lv.View = System.Windows.Forms.View.List;
            // 
            // FileSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 393);
            this.Controls.Add(this.host_lv);
            this.Name = "FileSelectorForm";
            this.Text = "FileSelectorForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView host_lv;

    }
}