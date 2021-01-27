using System;
using System.Windows.Forms;

namespace Diagram
{
    public partial class MainForm : Form //UID2589432679
    {
        // parent
        public Main main = null;

        public MainForm(Main main)
        {
            this.main = main;
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(149, 132);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Program.log.Write("Main form: hide");
            this.Hide();
            BeginInvoke(new MethodInvoker(delegate
            {
                Hide();
            }));
        }

        public void OpenDiagram(String Message) //UID0515997503
        {
            main.OpenDiagram(Message);
        }

        /// <summary>
        /// close application if not form is open</summary>
        public void TerminateApplication() //UID4213278976
        {
            main.TerminateApplication();
        }
    }
}
