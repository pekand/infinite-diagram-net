using System;
using System.Drawing;
using System.Windows.Forms;

namespace Diagram
{
    public delegate void PositionChangeEventHandler(object source, PositionEventArgs e);

    public class PositionEventArgs : EventArgs
    {
        private double ScrollPosition;
        public PositionEventArgs(double position)
        {
            ScrollPosition = position;
        }
        public double GetPosition()
        {
            return ScrollPosition;
        }
    }

    public class ScrollBar //UID2684111687
    {
        public object parent;   // okno v ktorom je scrollbar vykreslovany

        // position
        public long barx = 10;
        public long bary = 100;
        public long barwidth = 1000;
        public long barheight = 30;

        // scroll bar orientation
        public bool vertical = true;
        public bool horizontal = true;

        // margin
        public long barmarginleft = 40;
        public long barmarginright = 40;
        public long barmarginbottom = 20;

        // track
        public double position = 0.5F;
        public long trackwidth = 50;
        public long trackpos = 150;
        public long trackposold = 150;

        // mouse click
        public bool mousedown = false; // mouse click on scrollbar
        public long delta = 0;

        // timer - animation
        readonly Timer timer = new Timer(); // timer pre animaciu
        public long opacity = 0;
        public bool animation = false; // animation is running
        public bool active = false; // scrolbarr is visible
        public bool fadein = true; // running dade in animation
        public bool fadeout = false; // running fade out animation

        // event change position
        public event PositionChangeEventHandler OnChangePosition;

        public Color color = Color.Black;

        public ScrollBar(object parent, long width, long height, bool horizontalOrientation = true, double per = 0.5F)
        {
            this.parent = parent;

            ((Form)this.parent).Paint += new System.Windows.Forms.PaintEventHandler(this.PaintEvent);

            vertical = !horizontalOrientation;
            horizontal = horizontalOrientation;

            SetPosition(per);

            if (horizontal)
            {
                barx = barmarginleft;
                bary = height - barheight - barmarginbottom;
                barwidth = width - barx - barmarginright;

                trackpos = barx + (long)((barwidth - trackwidth) * position);
            }

            if (vertical)
            {
                bary = barmarginleft;
                barx = width - barheight - barmarginbottom;
                barwidth = barheight;
                barheight = height - bary - barmarginright;

                trackpos = bary + (long)((barheight - trackwidth) * position);
            }

            timer.Tick += new EventHandler(Tick);
            timer.Interval = 50;
            timer.Enabled = false;
        }

        public bool Resize(long width, long height)
        {
            if (horizontal)
            {
                bary = height - barheight - barmarginbottom;
                barwidth = width - barx - barmarginright;
            }

            if (vertical)
            {
                barx = width - barwidth - barmarginbottom;
                barheight = height - bary - barmarginright;
            }

            return true;
        }

        // EVENT Paint                                                                                 // [PAINT] [EVENT]
        public void PaintEvent(object sender, PaintEventArgs e)
        {
            this.Paint(e.Graphics);
        }

        public void Paint(Graphics g)
        {
            if (animation || active || fadeout)
            {

                Rectangle bar = new Rectangle();
                Rectangle tracker = new Rectangle();

                if (horizontal)
                {
                    bar.X = (int)barx;
                    bar.Y = (int)bary;
                    bar.Width = (int)barwidth;
                    bar.Height = (int)barheight;

                    tracker.X = (int)trackpos;
                    tracker.Y = (int)bary;
                    tracker.Width = (int)trackwidth;
                    tracker.Height = (int)barheight;
                }

                if (vertical)
                {
                    bar.X = (int)barx;
                    bar.Y = (int)bary;
                    bar.Width = (int)barwidth;
                    bar.Height = (int)barheight;

                    tracker.X = (int)barx;
                    tracker.Y = (int)trackpos;
                    tracker.Width = (int)barwidth;
                    tracker.Height = (int)trackwidth;
                }

                g.FillRectangle(new SolidBrush(Color.FromArgb((int)this.opacity, this.color)), bar);
                g.FillRectangle(new SolidBrush(Color.FromArgb((int)this.opacity * 2, this.color)), tracker);
            }
        }

        public bool MouseDown(int mx, int my)
        {
            if (barx <= mx && mx <= barx + barwidth && bary <= my && my <= bary + barheight)
            {
                mousedown = true;

                if (horizontal)
                {
                    if (trackpos <= mx && mx <= trackpos + trackwidth) // click on track
                    {
                        delta = trackpos - mx;
                        trackpos = mx + delta;
                    }
                    else
                    {  // click on bar
                        trackpos = mx - trackwidth / 2;
                        delta = -trackwidth / 2;
                    }

                    if (trackpos < barx)
                    {
                        trackpos = barx;
                    }

                    if (barx + barwidth - trackwidth < trackpos)
                    {
                        trackpos = barx + barwidth - trackwidth;
                    }

                    position = (double)(trackpos - barx) / (barwidth - trackwidth);
                }

                if (vertical)
                {
                    if (trackpos <= my && my <= trackpos + trackwidth) // click on track
                    {
                        delta = trackpos - my;
                        trackpos = my + delta;
                    }
                    else
                    {  // click on bar
                        trackpos = my - trackwidth / 2;
                        delta = -trackwidth / 2;
                    }

                    if (trackpos < bary)
                    {
                        trackpos = bary;
                    }

                    if (bary + barheight - trackwidth < trackpos)
                    {
                        trackpos = bary + barheight - trackwidth;
                    }

                    position = (double)(trackpos - bary) / (barheight - trackwidth);
                }

                return true;
            }
            return false;
        }

        public bool MouseMove(int mx, int my)
        {
            if (mousedown) // click to scroll bar
            {

                if (horizontal)
                {
                    trackpos = mx + delta;

                    if (trackpos < barx)
                    {
                        trackpos = barx;
                    }

                    if (barx + barwidth - trackwidth < trackpos)
                    {
                        trackpos = barx + barwidth - trackwidth;
                    }

                    position = (double)(trackpos - barx) / (barwidth - trackwidth);
                }

                if (vertical)
                {
                    trackpos = my + delta;

                    if (trackpos < bary)
                    {
                        trackpos = bary;
                    }

                    if (bary + barheight - trackwidth < trackpos)
                    {
                        trackpos = bary + barheight - trackwidth;
                    }

                    position = (double)(trackpos - bary) / (barheight - trackwidth);
                }

                if (trackposold != trackpos)
                {
                    trackposold = trackpos;

                    OnChangePosition?.Invoke(this, new PositionEventArgs(position));

                    return true;
                }

            }
            else // only move mouse about scroolbar
            {
                if (barx <= mx && mx <= barx + barwidth && bary <= my && my <= bary + barheight)
                {
                    if (!active)
                    {
                        active = true;
                        this.fadein = true;
                        this.fadeout = false;
                        timer.Start();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (active)
                    {
                        active = false;
                        this.fadein = false;
                        this.fadeout = true;
                        timer.Start();
                        return true;
                    }
                    else
                    {
                        active = false;
                        return false;
                    }
                }
            }

            return false;
        }

        public bool MouseUp()
        {
            if (mousedown) {
                if (trackposold != trackpos)
                {
                    trackposold = trackpos;

                    if (horizontal)
                    {
                        position = (double)(trackpos - barx) / (barwidth - trackwidth);
                    }


                    if (vertical)
                    {
                        position = (double)(trackpos - bary) / (barheight - trackwidth);
                    }

                    OnChangePosition?.Invoke(this, new PositionEventArgs(position));
                }

                mousedown = false;
                return true;
            }

            return false;
        }

        public void Tick(object sender, EventArgs e)
        {
            if (this.fadein)
            {
                if (this.opacity <= 50)
                {
                    this.opacity += 10;
                    this.animation = true;
                }
                else
                {
                    this.animation = false;
                    this.timer.Stop();
                }
            }

            if (this.fadeout)
            {
                if (this.opacity > 0)
                {
                    this.opacity -= 10;
                    this.animation = true;
                }
                else
                {
                    this.timer.Enabled = false;
                    this.timer.Stop();
                }
            }

            ((Form)this.parent).Invalidate();
		}

        public void SetPosition(double per)
        {
            position = per;
            if (horizontal)
            {
                trackpos = (int)(position * (barwidth - trackwidth) + barx);
            }

            if (vertical)
            {
                trackpos = (int)(position * (barheight - trackwidth) + bary);
            }
        }

        public void SetColor(Color color) {
            this.color = color;
        }
    }
}
