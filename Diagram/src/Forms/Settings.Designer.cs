namespace Diagram.src.Forms
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            panel = new Panel();
            buttonOk = new Button();
            SuspendLayout();
            // 
            // panel
            // 
            panel.AutoScroll = true;
            panel.Dock = DockStyle.Top;
            panel.Location = new Point(0, 0);
            panel.Name = "panel";
            panel.Size = new Size(694, 711);
            panel.TabIndex = 1;
            // 
            // buttonOk
            // 
            buttonOk.Location = new Point(594, 725);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(88, 35);
            buttonOk.TabIndex = 2;
            buttonOk.Text = "Ok";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += buttonOk_Click;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(694, 772);
            Controls.Add(buttonOk);
            Controls.Add(panel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Settings";
            Text = "Settings";
            Load += Settings_Load;
            Resize += Settings_Resize;
            ResumeLayout(false);
        }

        #endregion

        private Panel panel;
        private Button buttonOk;
        private Label label1;
        private TextBox textBox1;
    }
}