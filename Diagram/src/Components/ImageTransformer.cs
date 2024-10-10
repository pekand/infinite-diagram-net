﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diagram
{
    public class ImageTransformerData
    {
        public Image image;

        public int imageX = 0;
        public int imageY = 0;
        public int imageWidth = 0;
        public int imageHeight = 0;
        public int imageRotateX = 0;
        public int imageRotateY = 0;
        public bool flipX = false;
        public bool flipY = false;

        public bool imageSelected = false;
        public bool imageMove = false;
        public bool imageScale = false;
        public bool imageScaleLeftTop = false;
        public bool imageScaleRightTop = false;
        public bool imageScaleLeftBottom = false;
        public bool imageScaleRightBottom = false;
        public bool imageRotate = false;
        public bool imageOverRotate = false;
        public bool imageOver = false;
        public bool imageOverLeftTop = false;
        public bool imageOverRightTop = false;
        public bool imageOverLeftBottom = false;
        public bool imageOverRightBottom = false;


        public int mouseDownX = 0;
        public int mouseDownY = 0;
        public int mouseMoveX = 0;
        public int mouseMoveY = 0;

        public int vectorX = 0;
        public int vectorY = 0;
        public int vectorLTX = 0;
        public int vectorLTY = 0;
        public int vectorRTX = 0;
        public int vectorRTY = 0;
        public int vectorLBX = 0;
        public int vectorLBY = 0;
        public int vectorRBX = 0;
        public int vectorRBY = 0;

        public int handleSize = 8;

        public int imageMinWidth = 30;
        public int imageMinHeight = 30;

        public float imageScaleX = 1;
        public float imageScaleY = 1;

    }

    public class ImageTransformer
    {
        public ImageTransformerData data = new ImageTransformerData();
        public Form parent = null;

        public ImageTransformer(Form parent)
        {
            this.parent = parent;
        }

        public float GetAngleInDegrees(Point point1, Point point2)
        {
            double deltaX = point2.X - point1.X;
            double deltaY = point2.Y - point1.Y;

            if (deltaX == 0 && deltaY == 0)
            {
                return 0;
            }

            double radians = Math.Atan2(deltaY, deltaX);

            double degrees = radians * (180.0 / Math.PI);

            return (float)((degrees + 360.0) % 360.0);
        }

        public void Form_Init(object sender, EventArgs e)
        {
            data.image = Image.FromFile("c:\\Users\\pekar\\Desktop\\Resize image\\WinFormsApp1\\arrow.jpg");

            data.imageX = 50;
            data.imageY = 50;
            data.imageScaleX = 1.0f;
            data.imageScaleY = 1.0f;
            data.imageWidth = (int)(data.image.Width * data.imageScaleX);
            data.imageHeight = (int)(data.image.Height * data.imageScaleY);

        }

        public void Form_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int x = data.imageX + data.vectorX + data.vectorLTX + data.vectorLBX;
            int y = data.imageY + data.vectorY + data.vectorLTY + data.vectorRTY;
            int w = data.imageWidth - data.vectorLTX + data.vectorRTX - data.vectorLBX + data.vectorRBX;
            int h = data.imageHeight - data.vectorLTY - data.vectorRTY + data.vectorLBY + data.vectorRBY;


            Rectangle flippedRect = new Rectangle(x, y, w, h);

            if (data.flipX)
            {
                flippedRect.X = flippedRect.X + flippedRect.Width;
                flippedRect.Width = -flippedRect.Width;
            }

            if (data.flipY)
            {
                flippedRect.Y = flippedRect.Y + flippedRect.Height;
                flippedRect.Height = -flippedRect.Height;
            }

            g.TranslateTransform(x + (w / 2), y + (h / 2));
            g.RotateTransform(this.GetAngleInDegrees(new Point(0, 0), new Point(data.imageRotateX, data.imageRotateY)));
            g.TranslateTransform(-(x + (w / 2)), -(y + (h / 2)));
            g.DrawImage(data.image, flippedRect);
            g.ResetTransform();

            if (data.imageOver || data.imageSelected)
            {
                Rectangle r = new Rectangle(x, y, w, h);
                if (r.Width < 0)
                {
                    r.X = r.X + r.Width;
                    r.Width = -r.Width;
                }


                if (r.Height < 0)
                {
                    r.Y = r.Y + r.Height;
                    r.Height = -r.Height;
                }

                Pen pen = new Pen(Color.Black, 1);
                float[] dashes = { 4.0f, 2.0f };
                pen.DashPattern = dashes;
                g.DrawRectangle(pen, r);
            }


            if (data.imageScaleLeftTop || data.imageOverLeftTop)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue)))
                {
                    Rectangle rect = new Rectangle(
                        data.imageX + data.vectorX + data.vectorLTX + data.vectorLBX - data.handleSize,
                        data.imageY + data.vectorY + data.vectorLTY + data.vectorRTY - data.handleSize,
                        2 * data.handleSize,
                        2 * data.handleSize
                    );

                    e.Graphics.FillEllipse(brush, rect);
                }
            }

            if (data.imageScaleRightTop || data.imageOverRightTop)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue)))
                {
                    Rectangle rect = new Rectangle(
                        data.imageX + data.vectorX + data.vectorLTX + data.vectorLBX - data.handleSize +
                        data.imageWidth - data.vectorLTX + data.vectorRTX - data.vectorLBX + data.vectorRBX,
                        data.imageY + data.vectorY + data.vectorLTY + data.vectorRTY - data.handleSize,
                        2 * data.handleSize,
                        2 * data.handleSize
                    );
                    e.Graphics.FillEllipse(brush, rect);
                }

            }

            if (data.imageScaleLeftBottom || data.imageOverLeftBottom)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue)))
                {
                    Rectangle rect = new Rectangle(
                        data.imageX + data.vectorX + data.vectorLTX + data.vectorLBX - data.handleSize,
                        data.imageY + data.vectorY + data.vectorLTY + data.vectorRTY - data.handleSize +
                        data.imageHeight - data.vectorLTY - data.vectorRTY + data.vectorLBY + data.vectorRBY,
                        2 * data.handleSize,
                        2 * data.handleSize
                    );
                    e.Graphics.FillEllipse(brush, rect);
                }
            }

            if (data.imageScaleRightBottom || data.imageOverRightBottom)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue)))
                {
                    Rectangle rect = new Rectangle(
                        data.imageX + data.vectorX + data.vectorLTX + data.vectorLBX - data.handleSize +
                        data.imageWidth - data.vectorLTX + data.vectorRTX - data.vectorLBX + data.vectorRBX,
                        data.imageY + data.vectorY + data.vectorLTY + data.vectorRTY - data.handleSize +
                        data.imageHeight - data.vectorLTY - data.vectorRTY + data.vectorLBY + data.vectorRBY,
                        2 * data.handleSize,
                        2 * data.handleSize
                    );
                    e.Graphics.FillEllipse(brush, rect);
                }
            }

            if (data.imageRotate || data.imageOverRotate)
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 1),
                    new Point(x + (w / 2), y + (h / 2)),
                    new Point(x + (w / 2) + data.imageRotateX, y + (h / 2) + data.imageRotateY)
                );

                using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue)))
                {
                    Rectangle rect = new Rectangle(
                        x + (w / 2) - data.handleSize,
                        y + (h / 2) - data.handleSize,
                        2 * data.handleSize,
                        2 * data.handleSize
                    );
                    e.Graphics.FillEllipse(brush, rect);
                }

                using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue)))
                {
                    Rectangle rect = new Rectangle(
                        x + (w / 2) + data.imageRotateX - data.handleSize,
                        y + (h / 2) + data.imageRotateY - data.handleSize,
                        2 * data.handleSize,
                        2 * data.handleSize
                    );
                    e.Graphics.FillEllipse(brush, rect);
                }
            }
        }

        public void Form_MouseDown(object sender, MouseEventArgs e)
        {
            bool changed = false;
            int x = e.X;
            int y = e.Y;
            data.mouseDownX = x;
            data.mouseDownY = y;
            int s = data.handleSize;
            int cornerLeftTopX = data.imageX;
            int cornerLeftTopY = data.imageY;
            int cornerRightTopX = data.imageX + data.imageWidth;
            int cornerRightTopY = data.imageY;
            int cornerLeftBottomX = data.imageX;
            int cornerLeftBottomY = data.imageY + data.imageHeight;
            int cornerRightBottomX = data.imageX + data.imageWidth;
            int cornerRightBottomY = data.imageY + data.imageHeight;
            int middleX = data.imageX + (data.imageWidth / 2);
            int middleY = data.imageY + (data.imageHeight / 2);

            data.imageScale = false;
            data.imageScaleLeftTop = false;
            data.imageScaleRightTop = false;
            data.imageScaleLeftBottom = false;
            data.imageScaleRightBottom = false;
            data.imageMove = false;
            data.imageRotate = false;

            if (cornerLeftTopX - s < x && x < cornerLeftTopX + s && cornerLeftTopY - s < y && y < cornerLeftTopY + s)
            {
                data.imageScale = true;
                data.imageScaleLeftTop = true;
                data.imageSelected = true;
                changed = true;
            }
            else if (cornerRightTopX - s < x && x < cornerRightTopX + s && cornerRightTopY - s < y && y < cornerRightTopY + s)
            {
                data.imageScale = true;
                data.imageScaleRightTop = true;
                data.imageSelected = true;
                changed = true;
            }
            else if (cornerLeftBottomX - s < x && x < cornerLeftBottomX + s && cornerLeftBottomY - s < y && y < cornerLeftBottomY + s)
            {
                data.imageScale = true;
                data.imageScaleLeftBottom = true;
                data.imageSelected = true;
                changed = true;
            }
            else if (cornerRightBottomX - s < x && x < cornerRightBottomX + s && cornerRightBottomY - s < y && y < cornerRightBottomY + s)
            {
                data.imageScale = true;
                data.imageScaleRightBottom = true;
                data.imageSelected = true;
                changed = true;
            }
            else if (middleX - s < x && x < middleX + s && middleY - s < y && y < middleY + s)
            {
                data.imageRotate = true;
                data.imageSelected = true;
                changed = true;
            }
            else if (cornerLeftTopX < x && x < cornerRightBottomX && cornerLeftTopY < y && y < cornerRightBottomY)
            {
                data.imageSelected = true;
                data.imageMove = true;
                changed = true;
            }
            else if (data.imageSelected)
            {
                data.imageSelected = false;
                changed = true;
            }

            if (changed)
            {
                parent.Invalidate();
            }
        }

        public void Form_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            data.mouseMoveX = x;
            data.mouseMoveY = y;

            bool changed = false;
            int s = data.handleSize;
            int cornerLeftTopX = data.imageX;
            int cornerLeftTopY = data.imageY;
            int cornerRightTopX = data.imageX + data.imageWidth;
            int cornerRightTopY = data.imageY;
            int cornerLeftBottomX = data.imageX;
            int cornerLeftBottomY = data.imageY + data.imageHeight;
            int cornerRightBottomX = data.imageX + data.imageWidth;
            int cornerRightBottomY = data.imageY + data.imageHeight;
            int middleX = data.imageX + (data.imageWidth / 2);
            int middleY = data.imageY + (data.imageHeight / 2);

            if (data.imageScaleLeftTop)
            {
                data.vectorLTX = data.mouseMoveX - data.mouseDownX;
                data.vectorLTY = data.mouseMoveY - data.mouseDownY;
                changed = true;
            }

            if (data.imageScaleRightTop)
            {
                data.vectorRTX = data.mouseMoveX - data.mouseDownX;
                data.vectorRTY = data.mouseMoveY - data.mouseDownY;
                changed = true;

            }

            if (data.imageScaleLeftBottom)
            {
                data.vectorLBX = data.mouseMoveX - data.mouseDownX;
                data.vectorLBY = data.mouseMoveY - data.mouseDownY;
                changed = true;
            }

            if (data.imageScaleRightBottom)
            {
                data.vectorRBX = data.mouseMoveX - data.mouseDownX;
                data.vectorRBY = data.mouseMoveY - data.mouseDownY;
                changed = true;
            }

            if (data.imageRotate)
            {
                data.imageRotateX = data.mouseMoveX - middleX;
                data.imageRotateY = data.mouseMoveY - middleY;
                changed = true;
            }

            if (data.imageMove)
            {
                data.vectorX = data.mouseMoveX - data.mouseDownX;
                data.vectorY = data.mouseMoveY - data.mouseDownY;
                changed = true;
            }


            if (cornerLeftTopX < x && x < cornerRightTopX && cornerLeftTopY < y && y < cornerRightBottomY)
            {
                if (!data.imageOver)
                {
                    data.imageOver = true;
                    changed = true;
                }
            }
            else if (data.imageOver)
            {
                data.imageOver = false;
                changed = true;
            }

            if (cornerLeftTopX - s < x && x < cornerLeftTopX + s && cornerLeftTopY - s < y && y < cornerLeftTopY + s)
            {
                if (!data.imageOverLeftTop)
                {
                    data.imageOverLeftTop = true;
                    data.imageOverRightTop = false;
                    data.imageOverLeftBottom = false;
                    data.imageOverRightBottom = false;
                    data.imageOverRotate = false;
                    changed = true;
                }
            }
            else if (cornerRightTopX - s < x && x < cornerRightTopX + s && cornerRightTopY - s < y && y < cornerRightTopY + s)
            {
                if (!data.imageOverRightTop)
                {
                    data.imageOverLeftTop = false;
                    data.imageOverRightTop = true;
                    data.imageOverLeftBottom = false;
                    data.imageOverRightBottom = false;
                    data.imageOverRotate = false;
                    changed = true;
                }
            }
            else if (cornerLeftBottomX - s < x && x < cornerLeftBottomX + s && cornerLeftBottomY - s < y && y < cornerLeftBottomY + s)
            {
                if (!data.imageOverLeftBottom)
                {
                    data.imageOverLeftTop = false;
                    data.imageOverRightTop = false;
                    data.imageOverLeftBottom = true;
                    data.imageOverRightBottom = false;
                    data.imageOverRotate = false;
                    changed = true;
                }
            }
            else if (cornerRightBottomX - s < x && x < cornerRightBottomX + s && cornerRightBottomY - s < y && y < cornerRightBottomY + s)
            {
                if (!data.imageOverRightBottom)
                {
                    data.imageOverLeftTop = false;
                    data.imageOverRightTop = false;
                    data.imageOverLeftBottom = false;
                    data.imageOverRightBottom = true;
                    data.imageOverRotate = false;
                    changed = true;
                }
            }
            else if (middleX - s < x && x < middleX + s && middleY - s < y && y < middleY + s)
            {
                data.imageOverLeftTop = false;
                data.imageOverRightTop = false;
                data.imageOverLeftBottom = false;
                data.imageOverRightBottom = false;
                data.imageOverRotate = true;
                changed = true;
            }
            else if (data.imageOverLeftTop || data.imageOverRightTop || data.imageOverLeftBottom || data.imageOverRightBottom || data.imageOverRotate)
            {
                data.imageOverLeftTop = false;
                data.imageOverRightTop = false;
                data.imageOverLeftBottom = false;
                data.imageOverRightBottom = false;
                data.imageOverRotate = false;
                changed = true;
            }

            if (changed)
            {
                parent.Invalidate();
            }
        }

        public void Form_MouseUp(object sender, MouseEventArgs e)
        {
            bool changed = false;
            int x = e.X;
            int y = e.Y;
            int s = data.handleSize;

            if (data.imageScaleLeftTop)
            {
                data.imageX = data.imageX + data.vectorLTX;
                data.imageY = data.imageY + data.vectorLTY;
                data.imageWidth = data.imageWidth - data.vectorLTX;
                data.imageHeight = data.imageHeight - data.vectorLTY;

                if (data.imageWidth < 0)
                {
                    data.flipX = !data.flipX;
                    data.imageWidth = -data.imageWidth;
                    data.imageX = data.imageX - data.imageWidth;
                }

                if (data.imageHeight < 0)
                {
                    data.flipY = !data.flipY;
                    data.imageHeight = -data.imageHeight;
                    data.imageY = data.imageY - data.imageHeight;
                }

                if (data.imageWidth < data.imageMinWidth)
                {
                    data.imageX = data.imageX - (data.imageMinWidth - data.imageWidth);
                    data.imageWidth = data.imageMinWidth;
                }
                if (data.imageHeight < data.imageMinHeight)
                {
                    data.imageY = data.imageY - (data.imageMinHeight - data.imageHeight);
                    data.imageHeight = data.imageMinHeight;
                }

                changed = true;
            }

            if (data.imageScaleRightTop)
            {
                data.imageY = data.imageY + data.vectorRTY;
                data.imageWidth = data.imageWidth + data.vectorRTX;
                data.imageHeight = data.imageHeight - data.vectorRTY;

                if (data.imageWidth < 0)
                {
                    data.flipX = !data.flipX;
                    data.imageWidth = -data.imageWidth;
                    data.imageX = data.imageX - data.imageWidth;
                }

                if (data.imageHeight < 0)
                {
                    data.flipY = !data.flipY;
                    data.imageHeight = -data.imageHeight;
                    data.imageY = data.imageY - data.imageHeight;
                }

                if (data.imageWidth < data.imageMinWidth)
                {
                    data.imageWidth = data.imageMinWidth;
                }
                if (data.imageHeight < data.imageMinHeight)
                {
                    data.imageY = data.imageY - (data.imageMinHeight - data.imageHeight);
                    data.imageHeight = data.imageMinHeight;
                }

                changed = true;
            }

            if (data.imageScaleLeftBottom)
            {
                data.imageX = data.imageX + data.vectorLBX;
                data.imageWidth = data.imageWidth - data.vectorLBX;
                data.imageHeight = data.imageHeight + data.vectorLBY;

                if (data.imageWidth < 0)
                {
                    data.flipX = !data.flipX;
                    data.imageWidth = -data.imageWidth;
                    data.imageX = data.imageX - data.imageWidth;
                }

                if (data.imageHeight < 0)
                {
                    data.flipY = !data.flipY;
                    data.imageHeight = -data.imageHeight;
                    data.imageY = data.imageY - data.imageHeight;
                }

                if (data.imageWidth < data.imageMinWidth)
                {
                    data.imageX = data.imageX - (data.imageMinWidth - data.imageWidth);
                    data.imageWidth = data.imageMinWidth;
                }
                if (data.imageHeight < data.imageMinHeight)
                {
                    data.imageHeight = data.imageMinHeight;
                }

                changed = true;
            }

            if (data.imageScaleRightBottom)
            {
                data.imageWidth = data.imageWidth + data.vectorRBX;
                data.imageHeight = data.imageHeight + data.vectorRBY;

                if (data.imageWidth < 0)
                {
                    data.flipX = !data.flipX;
                    data.imageWidth = -data.imageWidth;
                    data.imageX = data.imageX - data.imageWidth;
                }

                if (data.imageHeight < 0)
                {
                    data.flipY = !data.flipY;
                    data.imageHeight = -data.imageHeight;
                    data.imageY = data.imageY - data.imageHeight;
                }

                if (data.imageWidth < data.imageMinWidth)
                {
                    data.imageWidth = data.imageMinWidth;
                }
                if (data.imageHeight < data.imageMinHeight)
                {
                    data.imageHeight = data.imageMinHeight;
                }
                changed = true;
            }

            if (data.imageMove)
            {
                data.imageX = data.imageX + data.vectorX;
                data.imageY = data.imageY + data.vectorY;
                changed = true;
            }

            if (data.imageRotate)
            {
                int middleX = data.imageX + (data.imageWidth / 2);
                int middleY = data.imageY + (data.imageHeight / 2);
                if (middleX - s < x && x < middleX + s && middleY - s < y && y < middleY + s)
                {
                    data.imageRotateX = 0;
                    data.imageRotateY = 0;
                }

                data.imageRotate = false;
                changed = true;
            }

            data.vectorX = 0;
            data.vectorY = 0;
            data.vectorLTX = 0;
            data.vectorLTY = 0;
            data.vectorRTX = 0;
            data.vectorRTY = 0;
            data.vectorLBX = 0;
            data.vectorLBY = 0;
            data.vectorRBX = 0;
            data.vectorRBY = 0;

            data.imageScale = false;
            data.imageScaleLeftTop = false;
            data.imageScaleRightTop = false;
            data.imageScaleLeftBottom = false;
            data.imageScaleRightBottom = false;
            data.imageMove = false;

            if (changed)
            {
                parent.Invalidate();
            }
        }
    }
}
