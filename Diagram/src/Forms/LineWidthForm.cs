#nullable disable

namespace Diagram
{
    public partial class LineWidthForm : Form //UID4672738884
    {
        public delegate void LineWidthFormTrackbarChangedEventHandler(int value);
        public event LineWidthFormTrackbarChangedEventHandler TrackbarStateChanged;
		private System.Windows.Forms.TrackBar trackBar1;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LineWidthForm));
            trackBar1 = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(16, 18);
            trackBar1.Margin = new Padding(5, 4, 5, 4);
            trackBar1.Minimum = 1;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(576, 45);
            trackBar1.TabIndex = 0;
            trackBar1.Value = 1;
            trackBar1.ValueChanged += TrackBar1_ValueChanged;
            // 
            // LineWidthForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(625, 96);
            Controls.Add(trackBar1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 4, 5, 4);
            MaximizeBox = false;
            Name = "LineWidthForm";
            Text = "Line width";
            Load += LineWidthForm_Load;
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        public LineWidthForm()
        {
            InitializeComponent();
        }

        public void LineWidthForm_Load(object sender, EventArgs e)
        {

        }

        public void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            this.TrackbarStateChanged?.Invoke(this.trackBar1.Value);
        }

        public void SetValue(long value)
        {
            this.trackBar1.Value = (int)value; // TODO: scale long to int
        }
        
    }
}
