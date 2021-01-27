using System;
using System.Windows.Forms;

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
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(14, 14);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(504, 45);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.Value = 1;
            this.trackBar1.ValueChanged += new System.EventHandler(this.TrackBar1_ValueChanged);
            // 
            // LineWidthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 76);
            this.Controls.Add(this.trackBar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "LineWidthForm";
            this.Text = "Line width";
            this.Load += new System.EventHandler(this.LineWidthForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
