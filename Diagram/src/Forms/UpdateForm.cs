using System;
using System.Windows.Forms;

namespace Diagram
{
    public partial class UpdateForm : Form
    {

        private System.ComponentModel.IContainer components = null;
        
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button buttonYes;
        private System.Windows.Forms.Button buttonNo;
        private System.Windows.Forms.ImageList imageList1;

        private string update = "No";
        private string skipVersion = "No";

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateForm));
            this.labelInfo = new System.Windows.Forms.Label();
            this.buttonYes = new System.Windows.Forms.Button();
            this.buttonNo = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.linkLabelVisit = new System.Windows.Forms.LinkLabel();
            this.buttonSkip = new System.Windows.Forms.Button();
            this.labelSkip = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelInfo.Location = new System.Drawing.Point(75, 36);
            this.labelInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(323, 24);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "Do you want install new version now?";
            // 
            // buttonYes
            // 
            this.buttonYes.Location = new System.Drawing.Point(322, 151);
            this.buttonYes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonYes.Name = "buttonYes";
            this.buttonYes.Size = new System.Drawing.Size(88, 27);
            this.buttonYes.TabIndex = 1;
            this.buttonYes.Text = "Yes";
            this.buttonYes.UseVisualStyleBackColor = true;
            this.buttonYes.Click += new System.EventHandler(this.ButtonYes_Click);
            // 
            // buttonNo
            // 
            this.buttonNo.Location = new System.Drawing.Point(416, 151);
            this.buttonNo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonNo.Name = "buttonNo";
            this.buttonNo.Size = new System.Drawing.Size(102, 27);
            this.buttonNo.TabIndex = 2;
            this.buttonNo.Text = "No";
            this.buttonNo.UseVisualStyleBackColor = true;
            this.buttonNo.Click += new System.EventHandler(this.ButtonNo_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // linkLabelVisit
            // 
            this.linkLabelVisit.AutoSize = true;
            this.linkLabelVisit.Location = new System.Drawing.Point(243, 78);
            this.linkLabelVisit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabelVisit.Name = "linkLabelVisit";
            this.linkLabelVisit.Size = new System.Drawing.Size(209, 15);
            this.linkLabelVisit.TabIndex = 5;
            this.linkLabelVisit.TabStop = true;
            this.linkLabelVisit.Text = "Visit homepage for more informations";
            this.linkLabelVisit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelVisit_LinkClicked);
            // 
            // buttonSkip
            // 
            this.buttonSkip.Location = new System.Drawing.Point(33, 151);
            this.buttonSkip.Name = "buttonSkip";
            this.buttonSkip.Size = new System.Drawing.Size(125, 27);
            this.buttonSkip.TabIndex = 6;
            this.buttonSkip.Text = "Skip this version";
            this.buttonSkip.UseVisualStyleBackColor = true;
            this.buttonSkip.Click += new System.EventHandler(this.buttonSkip_Click);
            // 
            // labelSkip
            // 
            this.labelSkip.AutoSize = true;
            this.labelSkip.Location = new System.Drawing.Point(33, 133);
            this.labelSkip.Name = "labelSkip";
            this.labelSkip.Size = new System.Drawing.Size(303, 15);
            this.labelSkip.TabIndex = 7;
            this.labelSkip.Text = "(You can download update manualy from popup menu)";
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 192);
            this.Controls.Add(this.labelSkip);
            this.Controls.Add(this.buttonSkip);
            this.Controls.Add(this.linkLabelVisit);
            this.Controls.Add(this.buttonNo);
            this.Controls.Add(this.buttonYes);
            this.Controls.Add(this.labelInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateForm";
            this.Text = "New version is available";
            this.Shown += new System.EventHandler(this.UpdateForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public UpdateForm()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ButtonYes_Click(object sender, EventArgs e)
        {
            this.update = "Yes";
            this.Close();
        }

        private void ButtonNo_Click(object sender, EventArgs e)
        {
            this.update = "No";
            this.Close();
        }

        public bool CanUpdate()
        {
            return this.update == "Yes";
        }

        public bool SkipVersion()
        {
            return this.skipVersion == "Yes";
        }

        private void UpdateForm_Shown(object sender, EventArgs e)
        {
            this.update = "No";
            this.skipVersion = "No";

            this.CenterToScreen();
        }

        private void linkLabelVisit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Network.OpenUrl("https://infinite-diagram.pekand.com/");
        }

        private void buttonSkip_Click(object sender, EventArgs e)
        {

            this.skipVersion = "No";
            this.Close();
        }
    }
}
