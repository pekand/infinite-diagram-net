using System;
using System.Windows.Forms;

namespace Diagram
{
    public partial class AboutForm : Form //UID4238351112
    {
        public Main main = null;

        private System.Windows.Forms.Label labelProgramName;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label labelLicence;
        private System.Windows.Forms.Label labelAuthor;
        private System.Windows.Forms.LinkLabel linkLabelMe;
        private System.Windows.Forms.Label labelLicenceType;
		private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelVersionNumber;
        private System.Windows.Forms.LinkLabel labelHomepage;

        public AboutForm(Main main)
        {
            this.main = main;
            this.InitializeComponent();

            this.labelLicenceType.Text = this.main.programOptions.license;
            this.labelVersionNumber.Text = Os.GetThisAssemblyVersion();
            this.linkLabelMe.Text = this.main.programOptions.author; 
            this.labelHomepage.Text = this.main.programOptions.home_page;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.labelProgramName = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.labelLicence = new System.Windows.Forms.Label();
            this.labelLicenceType = new System.Windows.Forms.Label();
            this.labelAuthor = new System.Windows.Forms.Label();
            this.linkLabelMe = new System.Windows.Forms.LinkLabel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelVersionNumber = new System.Windows.Forms.Label();
            this.labelHomepage = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // labelProgramName
            // 
            this.labelProgramName.AutoSize = true;
            this.labelProgramName.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelProgramName.Location = new System.Drawing.Point(19, 18);
            this.labelProgramName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelProgramName.Name = "labelProgramName";
            this.labelProgramName.Size = new System.Drawing.Size(241, 37);
            this.labelProgramName.TabIndex = 0;
            this.labelProgramName.Text = "Infinite Diagram";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(212, 123);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(65, 30);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // labelLicence
            // 
            this.labelLicence.AutoSize = true;
            this.labelLicence.Location = new System.Drawing.Point(33, 81);
            this.labelLicence.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelLicence.Name = "labelLicence";
            this.labelLicence.Size = new System.Drawing.Size(19, 15);
            this.labelLicence.TabIndex = 3;
            this.labelLicence.Text = "lic";
            // 
            // labelLicenceType
            // 
            this.labelLicenceType.AutoSize = true;
            this.labelLicenceType.Location = new System.Drawing.Point(133, 81);
            this.labelLicenceType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelLicenceType.Name = "labelLicenceType";
            this.labelLicenceType.Size = new System.Drawing.Size(0, 15);
            this.labelLicenceType.TabIndex = 6;
            // 
            // labelAuthor
            // 
            this.labelAuthor.AutoSize = true;
            this.labelAuthor.Location = new System.Drawing.Point(33, 61);
            this.labelAuthor.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAuthor.Name = "labelAuthor";
            this.labelAuthor.Size = new System.Drawing.Size(45, 15);
            this.labelAuthor.TabIndex = 4;
            this.labelAuthor.Text = "author:";
            // 
            // linkLabelMe
            // 
            this.linkLabelMe.AutoSize = true;
            this.linkLabelMe.Location = new System.Drawing.Point(133, 61);
            this.linkLabelMe.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabelMe.Name = "linkLabelMe";
            this.linkLabelMe.Size = new System.Drawing.Size(44, 15);
            this.linkLabelMe.TabIndex = 5;
            this.linkLabelMe.TabStop = true;
            this.linkLabelMe.Text = "Author";
            this.linkLabelMe.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelMe_LinkClicked);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(33, 100);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(48, 15);
            this.labelVersion.TabIndex = 3;
            this.labelVersion.Text = "version:";
            // 
            // labelVersionNumber
            // 
            this.labelVersionNumber.AutoSize = true;
            this.labelVersionNumber.Location = new System.Drawing.Point(133, 100);
            this.labelVersionNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelVersionNumber.Name = "labelVersionNumber";
            this.labelVersionNumber.Size = new System.Drawing.Size(22, 15);
            this.labelVersionNumber.TabIndex = 3;
            this.labelVersionNumber.Text = "0.0";
            // 
            // labelHomepage
            // 
            this.labelHomepage.AutoSize = true;
            this.labelHomepage.Location = new System.Drawing.Point(33, 115);
            this.labelHomepage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelHomepage.Name = "labelHomepage";
            this.labelHomepage.Size = new System.Drawing.Size(0, 15);
            this.labelHomepage.TabIndex = 3;
            this.labelHomepage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LabelHomepage_HomepageClicked);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 173);
            this.Controls.Add(this.labelLicenceType);
            this.Controls.Add(this.linkLabelMe);
            this.Controls.Add(this.labelAuthor);
            this.Controls.Add(this.labelLicence);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelVersionNumber);
            this.Controls.Add(this.labelHomepage);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.labelProgramName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About Infinite Diagram";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AboutForm_FormClosed);
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LinkLabelMe_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:pekand@gmail.com");
            }
            catch (Exception ex)
            {

                Program.log.Write("Send email error:"+ex.Message);
            }
            
        }

        private void LabelHomepage_HomepageClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Network.OpenUrl(this.main.programOptions.home_page);
            this.Close();
        }

        private void AboutForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
