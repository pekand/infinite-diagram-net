#nullable disable

using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Diagram
{
    public partial class ColorPickerForm : Form 
    {
        public delegate void ColorPickerFormChangeColor(ColorType color);
        public event ColorPickerFormChangeColor ChangeColor;

        public ColorType color = new();

        private int actualBitmap = 0;
        private int scrollState = 0;
        private readonly List<Bitmap> bitmaps = [];

        public bool allowClose = false;
        public bool allowMoveEvent = true;

        bool selecting = false;
        bool keyshift = false;

        private PictureBox pictureBox1;

        public DiagramView diagramView;

        public Point relativeOffset = new Point(0, 0);

        public ColorPickerForm(DiagramView diagramView)
        {

            this.diagramView = diagramView;
            Owner = diagramView;

            InitializeComponent();

            // draw image into box
            Render();
            pictureBox1.Image = this.bitmaps[actualBitmap];

            // create scrollbar
            pictureBox1.Width = 256;
            pictureBox1.Height = 256;
            this.Width = 256 + 16;
            this.Height = 256 + 39;

            this.Left = Screen.FromControl(this).Bounds.Width / 2 - this.Width / 2;
            this.Top = Screen.FromControl(this).Bounds.Height - this.Height - 100;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPickerForm));
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Margin = new Padding(5, 4, 5, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(521, 382);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.MouseDown += ColorPickerForm_MouseDown;
            pictureBox1.MouseMove += ColorPickerForm_MouseMove;
            pictureBox1.MouseUp += ColorPickerForm_MouseUp;
            pictureBox1.MouseWheel += PictureBox1_MouseWhell;
            // 
            // ColorPickerForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(542, 381);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 4, 5, 4);
            MaximizeBox = false;
            Name = "ColorPickerForm";
            ShowInTaskbar = false;
            Text = "Color";
            FormClosing += ColorPickerForm_FormClosing;
            FormClosed += ColorPickerForm_FormClosed;
            Load += ColorPickerForm_Load;
            KeyDown += ColorPickerForm_KeyDown;
            KeyUp += ColorPickerForm_KeyUp;
            Move += ColorPickerForm_Move;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);

        }

        private void ColorPickerForm_Load(object sender, EventArgs e)
        {

        }

        private void ColorPickerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowClose)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void ColorPickerForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void ColorPickerForm_MouseUp(object sender, MouseEventArgs e)
        {
            selecting = false;

            if (0 <= e.X && e.X <= 256 && 0 <= e.Y && e.Y < 256)
            {
                try
                {
                    this.color.Set(this.bitmaps[actualBitmap].GetPixel(e.X, e.Y));
                }
                catch (Exception ex)
                {
                    Program.log.Write("Colorpicker error: " + ex.Message);
                }
            }

            this.ChangeColor?.Invoke(this.color);
        }

        private void ColorPickerForm_MouseDown(object sender, MouseEventArgs e)
        {
            selecting = true;
        }

        private void ColorPickerForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (selecting)
            {
                if (0 <= e.X && e.X < 256 && 0 <= e.Y && e.Y < 256)
                {
                    try
                    {
                        this.color.Set(this.bitmaps[actualBitmap].GetPixel(e.X, e.Y));
                    }
                    catch (Exception ex)
                    {
                        Program.log.Write("Colorpicker error: " + ex.Message);
                    }
                }

                this.ChangeColor?.Invoke(this.color);
            }
        }

        private void PictureBox1_MouseWhell(object sender, MouseEventArgs e)
        {

            int speed = 4;

            if (this.keyshift)
            {
                speed = 1;
            }

            if (e.Delta > 0) // MWHELL
            {
                scrollState += speed;
            }
            else
            {
                scrollState -= speed;
            }


            if (scrollState > 127)
            {
                scrollState = 0;
            }


            if (scrollState < 0)
            {
                scrollState = 127;
            }


            if (0 <= scrollState && scrollState < 64)
            {
                actualBitmap = scrollState;

            }
            if (64 <= scrollState && scrollState < 128)
            {
                actualBitmap = 127 - scrollState;
            }

            pictureBox1.Image = this.bitmaps[actualBitmap];
            pictureBox1.Invalidate();
            this.Invalidate();
        }

        private void ColorPickerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift)
            {
                this.keyshift = true;
            }
        }

        private void ColorPickerForm_KeyUp(object sender, KeyEventArgs e)
        {
            this.keyshift = false;
        }

        private void ColorPickerForm_Move(object sender, EventArgs e)
        {
            if (allowMoveEvent)
            {
                relativeOffset = new Point(diagramView.Left - this.Left, diagramView.Top - this.Top);
            }
            
        }

        private SolidBrush Br(int r, int g, int b)
        {
            return new SolidBrush(Color.FromArgb(r, g, b));
        }

        private void Rc(Graphics gr, int r, int g, int b, int x, int y)
        {
            gr.FillRectangle(Br(r, g, b), x, y, 5, 5);
        }

        public void Render()
        {
            int cr;
            int cg;
            int cb;

            int px = 0;

            for (int i = 0; i < 64; i++)
            {
                Bitmap bmp = new(256, 256);
                Graphics g = Graphics.FromImage(bmp);

                cb = i * 4;
                for (int j = 0; j < 64; j++)
                {
                    cg = j * 4;
                    for (int k = 0; k < 64; k++)
                    {
                        cr = k * 4;
                        Rc(g,
                            cr, cg, cb,
                            k * 4,
                            j * 4
                        );
                    }
                }
                px += 256;
                g.Flush();
                this.bitmaps.Add(bmp);
            }
        }
    }
}
