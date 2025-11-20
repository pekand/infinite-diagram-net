#nullable disable

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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateForm));
            labelInfo = new Label();
            buttonYes = new Button();
            buttonNo = new Button();
            imageList1 = new ImageList(components);
            linkLabelVisit = new LinkLabel();
            buttonSkip = new Button();
            labelSkip = new Label();
            SuspendLayout();
            // 
            // labelInfo
            // 
            labelInfo.AutoSize = true;
            labelInfo.Font = new Font("Microsoft Sans Serif", 14F);
            labelInfo.Location = new Point(86, 48);
            labelInfo.Margin = new Padding(5, 0, 5, 0);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(323, 24);
            labelInfo.TabIndex = 0;
            labelInfo.Text = "Do you want install new version now?";
            // 
            // buttonYes
            // 
            buttonYes.Location = new Point(368, 201);
            buttonYes.Margin = new Padding(5, 4, 5, 4);
            buttonYes.Name = "buttonYes";
            buttonYes.Size = new Size(101, 36);
            buttonYes.TabIndex = 1;
            buttonYes.Text = "Yes";
            buttonYes.UseVisualStyleBackColor = true;
            buttonYes.Click += ButtonYes_Click;
            // 
            // buttonNo
            // 
            buttonNo.Location = new Point(475, 201);
            buttonNo.Margin = new Padding(5, 4, 5, 4);
            buttonNo.Name = "buttonNo";
            buttonNo.Size = new Size(117, 36);
            buttonNo.TabIndex = 2;
            buttonNo.Text = "No";
            buttonNo.UseVisualStyleBackColor = true;
            buttonNo.Click += ButtonNo_Click;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth8Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // linkLabelVisit
            // 
            linkLabelVisit.AutoSize = true;
            linkLabelVisit.Location = new Point(278, 104);
            linkLabelVisit.Margin = new Padding(5, 0, 5, 0);
            linkLabelVisit.Name = "linkLabelVisit";
            linkLabelVisit.Size = new Size(263, 20);
            linkLabelVisit.TabIndex = 5;
            linkLabelVisit.TabStop = true;
            linkLabelVisit.Text = "Visit homepage for more information";
            linkLabelVisit.LinkClicked += LinkLabelVisit_LinkClicked;
            // 
            // buttonSkip
            // 
            buttonSkip.Location = new Point(38, 201);
            buttonSkip.Margin = new Padding(3, 4, 3, 4);
            buttonSkip.Name = "buttonSkip";
            buttonSkip.Size = new Size(143, 36);
            buttonSkip.TabIndex = 6;
            buttonSkip.Text = "Skip this version";
            buttonSkip.UseVisualStyleBackColor = true;
            buttonSkip.Click += ButtonSkip_Click;
            // 
            // labelSkip
            // 
            labelSkip.AutoSize = true;
            labelSkip.Location = new Point(38, 177);
            labelSkip.Name = "labelSkip";
            labelSkip.Size = new Size(377, 20);
            labelSkip.TabIndex = 7;
            labelSkip.Text = "(You can download update manually from popup menu)";
            // 
            // UpdateForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(608, 256);
            Controls.Add(labelSkip);
            Controls.Add(buttonSkip);
            Controls.Add(linkLabelVisit);
            Controls.Add(buttonNo);
            Controls.Add(buttonYes);
            Controls.Add(labelInfo);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 4, 5, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "UpdateForm";
            Text = "New version is available";
            Load += UpdateForm_Load;
            Shown += UpdateForm_Shown;
            ResumeLayout(false);
            PerformLayout();

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

        private void LinkLabelVisit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Network.OpenUrl("https://infinite-diagram.pekand.com/");
        }

        private void ButtonSkip_Click(object sender, EventArgs e)
        {

            this.skipVersion = "No";
            this.Close();
        }

        private void UpdateForm_Load(object sender, EventArgs e)
        {

        }
    }
}
