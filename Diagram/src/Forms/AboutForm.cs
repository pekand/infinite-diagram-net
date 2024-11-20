using System;
using System.Windows.Forms;

#nullable disable

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
            labelProgramName = new Label();
            buttonOk = new Button();
            labelLicence = new Label();
            labelLicenceType = new Label();
            labelAuthor = new Label();
            linkLabelMe = new LinkLabel();
            labelVersion = new Label();
            labelVersionNumber = new Label();
            labelHomepage = new LinkLabel();
            SuspendLayout();
            // 
            // labelProgramName
            // 
            labelProgramName.AutoSize = true;
            labelProgramName.Font = new Font("Microsoft Sans Serif", 24F);
            labelProgramName.Location = new Point(22, 23);
            labelProgramName.Margin = new Padding(2, 0, 2, 0);
            labelProgramName.Name = "labelProgramName";
            labelProgramName.Size = new Size(241, 37);
            labelProgramName.TabIndex = 0;
            labelProgramName.Text = "Infinite Diagram";
            labelProgramName.Click += labelProgramName_Click;
            // 
            // buttonOk
            // 
            buttonOk.Location = new Point(242, 156);
            buttonOk.Margin = new Padding(2, 3, 2, 3);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(74, 38);
            buttonOk.TabIndex = 1;
            buttonOk.Text = "OK";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += ButtonOk_Click;
            // 
            // labelLicence
            // 
            labelLicence.AutoSize = true;
            labelLicence.Location = new Point(38, 103);
            labelLicence.Margin = new Padding(2, 0, 2, 0);
            labelLicence.Name = "labelLicence";
            labelLicence.Size = new Size(21, 19);
            labelLicence.TabIndex = 3;
            labelLicence.Text = "lic";
            // 
            // labelLicenceType
            // 
            labelLicenceType.AutoSize = true;
            labelLicenceType.Location = new Point(152, 103);
            labelLicenceType.Margin = new Padding(2, 0, 2, 0);
            labelLicenceType.Name = "labelLicenceType";
            labelLicenceType.Size = new Size(0, 19);
            labelLicenceType.TabIndex = 6;
            // 
            // labelAuthor
            // 
            labelAuthor.AutoSize = true;
            labelAuthor.Location = new Point(38, 77);
            labelAuthor.Margin = new Padding(2, 0, 2, 0);
            labelAuthor.Name = "labelAuthor";
            labelAuthor.Size = new Size(53, 19);
            labelAuthor.TabIndex = 4;
            labelAuthor.Text = "author:";
            // 
            // linkLabelMe
            // 
            linkLabelMe.AutoSize = true;
            linkLabelMe.Location = new Point(152, 77);
            linkLabelMe.Margin = new Padding(2, 0, 2, 0);
            linkLabelMe.Name = "linkLabelMe";
            linkLabelMe.Size = new Size(52, 19);
            linkLabelMe.TabIndex = 5;
            linkLabelMe.TabStop = true;
            linkLabelMe.Text = "Author";
            linkLabelMe.LinkClicked += LinkLabelMe_LinkClicked;
            // 
            // labelVersion
            // 
            labelVersion.AutoSize = true;
            labelVersion.Location = new Point(38, 127);
            labelVersion.Margin = new Padding(2, 0, 2, 0);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(56, 19);
            labelVersion.TabIndex = 3;
            labelVersion.Text = "version:";
            // 
            // labelVersionNumber
            // 
            labelVersionNumber.AutoSize = true;
            labelVersionNumber.Location = new Point(152, 127);
            labelVersionNumber.Margin = new Padding(2, 0, 2, 0);
            labelVersionNumber.Name = "labelVersionNumber";
            labelVersionNumber.Size = new Size(28, 19);
            labelVersionNumber.TabIndex = 3;
            labelVersionNumber.Text = "0.0";
            // 
            // labelHomepage
            // 
            labelHomepage.AutoSize = true;
            labelHomepage.Location = new Point(38, 146);
            labelHomepage.Margin = new Padding(2, 0, 2, 0);
            labelHomepage.Name = "labelHomepage";
            labelHomepage.Size = new Size(0, 19);
            labelHomepage.TabIndex = 3;
            labelHomepage.LinkClicked += LabelHomepage_HomepageClicked;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(346, 219);
            Controls.Add(labelLicenceType);
            Controls.Add(linkLabelMe);
            Controls.Add(labelAuthor);
            Controls.Add(labelLicence);
            Controls.Add(labelVersion);
            Controls.Add(labelVersionNumber);
            Controls.Add(labelHomepage);
            Controls.Add(buttonOk);
            Controls.Add(labelProgramName);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2, 3, 2, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "About Infinite Diagram";
            FormClosed += AboutForm_FormClosed;
            Load += AboutForm_Load;
            ResumeLayout(false);
            PerformLayout();
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

                Program.log.Write("Send email error:" + ex.Message);
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

        private void labelProgramName_Click(object sender, EventArgs e)
        {

        }
    }
}
