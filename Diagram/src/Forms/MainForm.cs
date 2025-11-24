#nullable disable

namespace Diagram
{
    public partial class MainForm : Form 
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
            SuspendLayout();
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(170, 167);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 4, 5, 4);
            Name = "MainForm";
            ShowInTaskbar = false;
            Text = "MainForm";
            Load += MainForm_Load;
            ResumeLayout(false);

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

        public void OpenDiagram(String Message) 
        {
            main.OpenDiagram(Message);
        }

        /// <summary>
        /// close application if not form is open</summary>
        public void TerminateApplication() 
        {
            main.TerminateApplication();
        }
    }
}
