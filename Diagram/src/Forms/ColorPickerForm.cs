using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Diagram
{
    public partial class ColorPickerForm : Form //UID2354438225
    {
        public delegate void ColorPickerFormChangeColor(ColorType color);
        public event ColorPickerFormChangeColor ChangeColor;

        public ColorType color = new ColorType();

        private int actualBitmap = 0;
        private int scrollState = 0;
        private readonly List<Bitmap> bitmaps = new List<Bitmap>();

        bool selecting = false;

        bool keyshift = false;

        private PictureBox pictureBox1;
  
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPickerForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(456, 493);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColorPickerForm_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ColorPickerForm_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ColorPickerForm_MouseUp);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWhell);
            // 
            // ColorPickerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(982, 286);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "ColorPickerForm";
            this.Text = "Color";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ColorPickerForm_FormClosed);
            this.Load += new System.EventHandler(this.ColorPickerForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ColorPickerForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ColorPickerForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        private void ColorPickerForm_FormClosed(object sender, FormClosedEventArgs e)
        {

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
                Bitmap bmp = new Bitmap(256, 256);                
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

        public ColorPickerForm()
        {
            InitializeComponent();

            // draw image into box
            Render();
            pictureBox1.Image = this.bitmaps[actualBitmap];

            // create scrollbar
            pictureBox1.Width = 256;
            pictureBox1.Height = 256;
            this.Width = 256+16;
            this.Height = 256 + 39;

            this.Left = Screen.FromControl(this).Bounds.Width / 2 - this.Width / 2;
            this.Top = Screen.FromControl(this).Bounds.Height - this.Height - 100;
        }

        private void ColorPickerForm_MouseUp(object sender, MouseEventArgs e)
        {
            selecting = false;

            if (0 <= e.X && e.X <= 256 && 0 <= e.Y && e.Y < 256)
            {
                try
                {
                    this.color.Set(this.bitmaps[actualBitmap].GetPixel(e.X, e.Y));
                } catch(Exception ex) {
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

        private void ColorPickerForm_Load(object sender, EventArgs e)
        {

        }

        private void PictureBox1_MouseWhell(object sender, MouseEventArgs e)
        {

            int speed = 4;

            if (this.keyshift) {
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


            if (0 <= scrollState && scrollState <64) {
                actualBitmap = scrollState;

            } if (64<= scrollState && scrollState < 128) {
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
    }
}
