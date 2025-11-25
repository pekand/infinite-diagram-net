#nullable disable

namespace Diagram
{
    public class ImageTransformerData
    {
        public Bitmap image;

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
        public bool imageOutside = false;

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

    public class ImageTransformer(DiagramView diagramView)
    {
        public ImageTransformerData data = new();
        public DiagramView diagramView = diagramView;
        public Node node = null;
        public Node prevNode = null;
        public bool disabled = true;
        public bool canRotate = true;
        public bool canResize = true;
        public bool canMove = true;
        public bool invalidImage = false; // prevend redraw images with exceptions

        public void Form_Init(Node node)
        {
            this.Reset();

            this.invalidImage = false;
            this.node = node;
            this.prevNode = node.Clone();
            this.node.visible = false;

            data.image = null;

            if (node.isImage)
            {
                data.image = ImageManager.CloneBitmap(node.image.Image);
                canRotate = true;
                canResize = true;
                canMove = true;
            }
            else {
                data.image = this.diagramView.DrawTextWithBackground(node);
                canRotate = true;
                canResize = false;
                canMove = false;                
            }

            decimal s = Calc.GetScale(diagramView.scale);
            Rectangle imageRec = this.diagramView.RecatanglePositionFromDiagramToView(node);

            this.data.imageX = imageRec.X;
            this.data.imageY = imageRec.Y;
            this.data.imageWidth = imageRec.Width;
            this.data.imageHeight = imageRec.Height;
            this.data.imageScaleX = 1.0f;
            this.data.imageScaleY = 1.0f;

            if (this.node.isImageTransformed)
            {
                this.data.imageRotateX = this.node.transformationRotateX;
                this.data.imageRotateY = this.node.transformationRotateY;
                this.data.flipX = this.node.transformationFlipX;
                this.data.flipY = this.node.transformationFlipY;
            }

            this.disabled = false;
        }

        public void Reset()
        {
            this.data = new ImageTransformerData();
        }

        public void Form_Paint(object sender, PaintEventArgs e)
        {
            if (this.disabled) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int x = data.imageX + data.vectorX + data.vectorLTX + data.vectorLBX;
            int y = data.imageY + data.vectorY + data.vectorLTY + data.vectorRTY;
            int w = data.imageWidth - data.vectorLTX + data.vectorRTX - data.vectorLBX + data.vectorRBX;
            int h = data.imageHeight - data.vectorLTY - data.vectorRTY + data.vectorLBY + data.vectorRBY;


            Rectangle flippedRect = new(x, y, w, h);

            if (data.flipX)
            {
                flippedRect.X += flippedRect.Width;
                flippedRect.Width = -flippedRect.Width;
            }

            if (data.flipY)
            {
                flippedRect.Y += flippedRect.Height;
                flippedRect.Height = -flippedRect.Height;
            }

            g.TranslateTransform(x + (w / 2), y + (h / 2));
            g.RotateTransform(Calc.GetAngleInDegrees(new Point(0, 0), new Point(data.imageRotateX, data.imageRotateY)));
            g.TranslateTransform(-(x + (w / 2)), -(y + (h / 2)));

            try
            {
                if (!invalidImage)
                {
                    g.DrawImage(data.image, flippedRect);
                }
            }
            catch (Exception ex)
            {
                invalidImage = true;
                Program.log.Write("ImageTransformerData paint error: " + ex.ToString());
            }

            
            g.ResetTransform();

            if (data.imageOver || data.imageSelected)
            {
                Rectangle r = new(x, y, w, h);
                if (r.Width < 0)
                {
                    r.X += r.Width;
                    r.Width = -r.Width;
                }


                if (r.Height < 0)
                {
                    r.Y += r.Height;
                    r.Height = -r.Height;
                }

                Pen pen = new(Color.Black, 1);
                float[] dashes = [4.0f, 2.0f];
                pen.DashPattern = dashes;
                g.DrawRectangle(pen, r);
            }


            if (data.imageScaleLeftTop || data.imageOverLeftTop)
            {
                using Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue));

                Rectangle rect = new(
                    data.imageX + data.vectorX + data.vectorLTX + data.vectorLBX - data.handleSize,
                    data.imageY + data.vectorY + data.vectorLTY + data.vectorRTY - data.handleSize,
                    2 * data.handleSize,
                    2 * data.handleSize
                );

                e.Graphics.FillEllipse(brush, rect);
            }

            if (data.imageScaleRightTop || data.imageOverRightTop)
            {
                using Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue));

                Rectangle rect = new(
                    data.imageX + data.vectorX + data.vectorLTX + data.vectorLBX - data.handleSize +
                    data.imageWidth - data.vectorLTX + data.vectorRTX - data.vectorLBX + data.vectorRBX,
                    data.imageY + data.vectorY + data.vectorLTY + data.vectorRTY - data.handleSize,
                    2 * data.handleSize,
                    2 * data.handleSize
                );
                e.Graphics.FillEllipse(brush, rect);

            }

            if (data.imageScaleLeftBottom || data.imageOverLeftBottom)
            {
                using Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue));

                Rectangle rect = new(
                    data.imageX + data.vectorX + data.vectorLTX + data.vectorLBX - data.handleSize,
                    data.imageY + data.vectorY + data.vectorLTY + data.vectorRTY - data.handleSize +
                    data.imageHeight - data.vectorLTY - data.vectorRTY + data.vectorLBY + data.vectorRBY,
                    2 * data.handleSize,
                    2 * data.handleSize
                );
                e.Graphics.FillEllipse(brush, rect);
            }

            if (data.imageScaleRightBottom || data.imageOverRightBottom)
            {
                using Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue));

                Rectangle rect = new(
                    data.imageX + data.vectorX + data.vectorLTX + data.vectorLBX - data.handleSize +
                    data.imageWidth - data.vectorLTX + data.vectorRTX - data.vectorLBX + data.vectorRBX,
                    data.imageY + data.vectorY + data.vectorLTY + data.vectorRTY - data.handleSize +
                    data.imageHeight - data.vectorLTY - data.vectorRTY + data.vectorLBY + data.vectorRBY,
                    2 * data.handleSize,
                    2 * data.handleSize
                );
                e.Graphics.FillEllipse(brush, rect);
            }


            e.Graphics.DrawLine(new(Color.Black, 1),
                new Point(x + (w / 2), y + (h / 2)),
                new Point(x + (w / 2) + data.imageRotateX, y + (h / 2) + data.imageRotateY)
            );

            using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue)))
            {
                Rectangle rect = new(
                    x + (w / 2) - data.handleSize,
                    y + (h / 2) - data.handleSize,
                    2 * data.handleSize,
                    2 * data.handleSize
                );
                e.Graphics.FillEllipse(brush, rect);
            }

            using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue)))
            {
                Rectangle rect = new(
                    x + (w / 2) + data.imageRotateX - data.handleSize,
                    y + (h / 2) + data.imageRotateY - data.handleSize,
                    2 * data.handleSize,
                    2 * data.handleSize
                );
                e.Graphics.FillEllipse(brush, rect);
            }

        }

        public void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.disabled) return;

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
            int imageRotateX = data.imageRotateX + middleX;
            int imageRotateY = data.imageRotateY + middleY;

            data.imageScale = false;
            data.imageScaleLeftTop = false;
            data.imageScaleRightTop = false;
            data.imageScaleLeftBottom = false;
            data.imageScaleRightBottom = false;
            data.imageMove = false;
            data.imageRotate = false;

            if (canResize && cornerLeftTopX - s < x && x < cornerLeftTopX + s && cornerLeftTopY - s < y && y < cornerLeftTopY + s)
            {
                data.imageScale = true;
                data.imageScaleLeftTop = true;
                data.imageSelected = true;
                changed = true;
            }
            else if (canResize && cornerRightTopX - s < x && x < cornerRightTopX + s && cornerRightTopY - s < y && y < cornerRightTopY + s)
            {
                data.imageScale = true;
                data.imageScaleRightTop = true;
                data.imageSelected = true;
                changed = true;
            }
            else if (canResize && cornerLeftBottomX - s < x && x < cornerLeftBottomX + s && cornerLeftBottomY - s < y && y < cornerLeftBottomY + s)
            {
                data.imageScale = true;
                data.imageScaleLeftBottom = true;
                data.imageSelected = true;
                changed = true;
            }
            else if (canResize && cornerRightBottomX - s < x && x < cornerRightBottomX + s && cornerRightBottomY - s < y && y < cornerRightBottomY + s)
            {
                data.imageScale = true;
                data.imageScaleRightBottom = true;
                data.imageSelected = true;
                changed = true;
            }
            else if (canRotate && middleX - s < x && x < middleX + s && middleY - s < y && y < middleY + s)
            {
                data.imageRotate = true;
                data.imageSelected = true;
                changed = true;
            }
            else if (canRotate && imageRotateX - s < x && x < imageRotateX + s && imageRotateY - s < y && y < imageRotateY + s)
            {
                data.imageRotate = true;
                data.imageSelected = true;
                changed = true;
            }
            else if(canMove &&  cornerLeftTopX < x && x < cornerRightBottomX && cornerLeftTopY < y && y < cornerRightBottomY)
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
                diagramView.Invalidate();
            }
        }

        public void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.disabled) return;

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

            if (canResize && data.imageScaleLeftTop)
            {
                data.vectorLTX = data.mouseMoveX - data.mouseDownX;
                data.vectorLTY = data.mouseMoveY - data.mouseDownY;
                changed = true;
            }

            if (canResize && data.imageScaleRightTop)
            {
                data.vectorRTX = data.mouseMoveX - data.mouseDownX;
                data.vectorRTY = data.mouseMoveY - data.mouseDownY;
                changed = true;

            }

            if (canResize && data.imageScaleLeftBottom)
            {
                data.vectorLBX = data.mouseMoveX - data.mouseDownX;
                data.vectorLBY = data.mouseMoveY - data.mouseDownY;
                changed = true;
            }

            if (canResize && data.imageScaleRightBottom)
            {
                data.vectorRBX = data.mouseMoveX - data.mouseDownX;
                data.vectorRBY = data.mouseMoveY - data.mouseDownY;
                changed = true;
            }

            if (canRotate && data.imageRotate)
            {
                data.imageRotateX = data.mouseMoveX - middleX;
                data.imageRotateY = data.mouseMoveY - middleY;
                changed = true;
            }

            if (canMove &&  data.imageMove)
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

            if (canResize && cornerLeftTopX - s < x && x < cornerLeftTopX + s && cornerLeftTopY - s < y && y < cornerLeftTopY + s)
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
            else if (canResize && cornerRightTopX - s < x && x < cornerRightTopX + s && cornerRightTopY - s < y && y < cornerRightTopY + s)
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
            else if (canResize && cornerLeftBottomX - s < x && x < cornerLeftBottomX + s && cornerLeftBottomY - s < y && y < cornerLeftBottomY + s)
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
            else if (canResize && cornerRightBottomX - s < x && x < cornerRightBottomX + s && cornerRightBottomY - s < y && y < cornerRightBottomY + s)
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
            else if (canRotate && middleX - s < x && x < middleX + s && middleY - s < y && y < middleY + s)
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
                diagramView.Invalidate();
            }
        }

        public void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.disabled) return;

            bool changed = false;
            int x = e.X;
            int y = e.Y;
            int s = data.handleSize;

            if (!data.imageSelected) {
                this.TransformationFinish();                
            }

            if (canResize &&  data.imageScaleLeftTop)
            {
                data.imageX += data.vectorLTX;
                data.imageY += data.vectorLTY;
                data.imageWidth -= data.vectorLTX;
                data.imageHeight -= data.vectorLTY;

                if (data.imageWidth < 0)
                {
                    data.flipX = !data.flipX;
                    data.imageWidth = -data.imageWidth;
                    data.imageX -= data.imageWidth;
                }

                if (data.imageHeight < 0)
                {
                    data.flipY = !data.flipY;
                    data.imageHeight = -data.imageHeight;
                    data.imageY -= data.imageHeight;
                }

                if (data.imageWidth < data.imageMinWidth)
                {
                    data.imageX -= (data.imageMinWidth - data.imageWidth);
                    data.imageWidth = data.imageMinWidth;
                }
                if (data.imageHeight < data.imageMinHeight)
                {
                    data.imageY -= (data.imageMinHeight - data.imageHeight);
                    data.imageHeight = data.imageMinHeight;
                }

                changed = true;
            }

            if (canResize && data.imageScaleRightTop)
            {
                data.imageY += data.vectorRTY;
                data.imageWidth += data.vectorRTX;
                data.imageHeight -= data.vectorRTY;

                if (data.imageWidth < 0)
                {
                    data.flipX = !data.flipX;
                    data.imageWidth = -data.imageWidth;
                    data.imageX -= data.imageWidth;
                }

                if (data.imageHeight < 0)
                {
                    data.flipY = !data.flipY;
                    data.imageHeight = -data.imageHeight;
                    data.imageY -= data.imageHeight;
                }

                if (data.imageWidth < data.imageMinWidth)
                {
                    data.imageWidth = data.imageMinWidth;
                }
                if (data.imageHeight < data.imageMinHeight)
                {
                    data.imageY -= (data.imageMinHeight - data.imageHeight);
                    data.imageHeight = data.imageMinHeight;
                }

                changed = true;
            }

            if (canResize && data.imageScaleLeftBottom)
            {
                data.imageX += data.vectorLBX;
                data.imageWidth -= data.vectorLBX;
                data.imageHeight += data.vectorLBY;

                if (data.imageWidth < 0)
                {
                    data.flipX = !data.flipX;
                    data.imageWidth = -data.imageWidth;
                    data.imageX -= data.imageWidth;
                }

                if (data.imageHeight < 0)
                {
                    data.flipY = !data.flipY;
                    data.imageHeight = -data.imageHeight;
                    data.imageY -= data.imageHeight;
                }

                if (data.imageWidth < data.imageMinWidth)
                {
                    data.imageX -= (data.imageMinWidth - data.imageWidth);
                    data.imageWidth = data.imageMinWidth;
                }
                if (data.imageHeight < data.imageMinHeight)
                {
                    data.imageHeight = data.imageMinHeight;
                }

                changed = true;
            }

            if (canResize && data.imageScaleRightBottom)
            {
                data.imageWidth += data.vectorRBX;
                data.imageHeight += data.vectorRBY;

                if (data.imageWidth < 0)
                {
                    data.flipX = !data.flipX;
                    data.imageWidth = -data.imageWidth;
                    data.imageX -= data.imageWidth;
                }

                if (data.imageHeight < 0)
                {
                    data.flipY = !data.flipY;
                    data.imageHeight = -data.imageHeight;
                    data.imageY -= data.imageHeight;
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

            if (canMove && data.imageMove)
            {
                data.imageX += data.vectorX;
                data.imageY += data.vectorY;
                changed = true;
            }

            if (canRotate && data.imageRotate)
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
                diagramView.Invalidate();
            }
        }

        public bool ProcessCmdKey(Message msg, Keys keyData)
        {
            if (this.disabled) return false;

            if (keyData == Keys.Escape)
            {
                this.TransformationFinish();
                return true;
            }

            return false;
        }

        public void TransformationFinish() 
        {
            RectangleD rectangle =  this.diagramView.RecatanglePositionFromViewToDiagram(
                this.data.imageX, 
                this.data.imageY, 
                (decimal)(this.data.imageWidth * this.data.imageScaleX),
                (decimal)(this.data.imageHeight * this.data.imageScaleY),
                this.node.scale
            );

            bool isModified = false;
            if (
                this.node.position.x != rectangle.x ||
                this.node.position.y != rectangle.y ||
                this.node.width != rectangle.width ||
                this.node.height != rectangle.height ||
                this.node.isImageTransformed != true ||
                this.node.transformationRotateX != this.data.imageRotateX ||
                this.node.transformationRotateY != this.data.imageRotateY ||
                this.node.transformationFlipX != this.data.flipX ||
                this.node.transformationFlipY != this.data.flipY
            )
            {
                isModified = true;
            }

            this.node.position.x = rectangle.x;
            this.node.position.y = rectangle.y;
            this.node.width = rectangle.width;
            this.node.height = rectangle.height;

            this.node.isImageTransformed = true;
            this.node.transformationRotateX = this.data.imageRotateX;
            this.node.transformationRotateY = this.data.imageRotateY;
            this.node.transformationFlipX = this.data.flipX;
            this.node.transformationFlipY = this.data.flipY;

            this.disabled = true;
            this.node.visible = true;

            data.image.Dispose();
            data.image = null;

            this.Diable();

            this.diagramView.TransformImageFinish(isModified, this.prevNode);          
        }

        public void Diable()
        {
            this.disabled = true;
        }

        public void Enable()
        {
            this.disabled = false;
        }

    }
}
