using System.Collections.Generic;
using System.Drawing;

namespace Diagram
{
    // map node structure for copy paste operation
    public class BreadcrumbItem //UID0790776066
    {
        public long layerId;
        public long left;
        public long top;
        public long width;
        public long height;
        public string name;
    }

    public class Breadcrumbs
    {
        public bool isVisible = true;
         
        public DiagramView diagramView = null;

        private readonly List<BreadcrumbItem> items = new List<BreadcrumbItem>();

        // resources
        private readonly Font font = new Font("Arial", 12);
        private readonly SolidBrush brush = new SolidBrush(Color.Gray);
        private readonly SolidBrush redBrash = new SolidBrush(Color.FromArgb(200, 255, 102, 0));
        private readonly SolidBrush yellowBrash = new SolidBrush(Color.FromArgb(200, 255, 255, 0));
        private readonly SolidBrush barBrash =  new SolidBrush(Color.FromArgb(50, 0, 0, 0));
        private readonly SolidBrush separatorBrash = new SolidBrush(Color.FromArgb(100, 0, 0, 0));

        private long left = 10;
        private long top = 10;
        private long width = 0;
        private long height = 0;
        private readonly long itemSpace = 5;

        public Breadcrumbs(DiagramView diagramView)
        {
            this.diagramView = diagramView;
        }

        public void Update() //UID2139429132
        {
            this.width = 0;
            this.height = 0;
            this.isVisible = false;

            if (this.diagramView.layersHistory != null 
                && this.diagramView.layersHistory.Count > 1)
            {
                this.items.Clear();

                long i = 0;
                foreach (Layer layer in this.diagramView.layersHistory)
                {
                    //skip first top layer because logo is showed insted
                    if (i++ == 0) continue;

                    BreadcrumbItem item = new BreadcrumbItem();

                    if (layer.parentNode != null)
                    {
                        item.name = layer.parentNode.name;
                    }
                    else
                    {
                        item.name = "Home";
                    }

                    if (item.name.Length > 10) {
                        item.name = item.name.Substring(0, 9);
                    }

                    SizeF s = Fonts.MeasureString(item.name, this.font);
                    item.left = this.width;
                    item.top = 0;
                    item.height = (int)s.Height;
                    item.width = (int)s.Width;
                    this.width += item.width + itemSpace;

                    if (this.height < item.height)
                    {
                        this.height = item.height;
                    }

                    item.layerId = layer.id; // for restore layer after click

                    this.items.Add(item);
                }

                // add logo width
                this.width += this.height;

                // add logo width to items
                foreach (BreadcrumbItem item in items)
                {
                    item.left += this.height;
                }

                this.isVisible = true;
            }
        }

        // EVENT Paint UID1441007106                                                                                
        public void Draw(Graphics g)
        {
            if (!this.isVisible)
            {
                return;
            }

            this.left = this.diagramView.ClientSize.Width - this.width - 10;
            this.top = 10;

            // logo
            long logopadding = (this.height > 10) ? this.height / 10 : 1;

            // draw bar
            g.FillRectangle(
                this.barBrash,
                this.left + this.height + 2,
                this.top,
                this.width - this.height - 2,
                this.height
            );

            //logo background
            g.FillRectangle(
                this.barBrash,
                this.left - 2, 
                this.top - 2, 
                this.height + 4, 
                this.height + 4
            );

            //logo top left
            g.FillRectangle(
                this.yellowBrash,
                this.left + logopadding, 
                this.top + logopadding, 
                this.height - this.height/2 - 2 * logopadding, 
                this.height - this.height / 2 - 2 * logopadding
            );

            //logo bottom right
            g.FillRectangle(
                this.yellowBrash,
                this.left + this.height / 2 + logopadding, 
                this.top + this.height / 2 + logopadding, 
                this.height - this.height / 2 - 2 * logopadding, 
                this.height - this.height / 2 - 2 * logopadding
            );

            //logo bottom left
           g.FillRectangle(
                this.redBrash,
                this.left + logopadding, 
                this.top + this.height / 2 + logopadding, 
                this.height - this.height / 2 - 2 * logopadding, 
                this.height - this.height / 2 - 2 * logopadding
            );

            //logo top right
            g.FillRectangle(
                this.redBrash,
                this.left + this.height / 2 + logopadding, 
                this.top + logopadding, 
                this.height - this.height / 2 - 2 * logopadding, 
                this.height - this.height / 2 - 2 * logopadding
            );

            // Draw node names
            int i = items.Count;
            foreach (BreadcrumbItem item in items)
            {
                // layer name
                g.DrawString(
                    item.name,
                    this.font,
                    this.brush,
                    this.left + item.left, 
                    this.top
                );

                // draw separator
                if (i-- > 1)
                {
                    g.FillEllipse(
                        this.separatorBrash,
                        this.left + item.left + item.width ,
                        this.top + item.top + item.height/2,
                        item.height / 5,
                        item.height / 5
                    );
                }
            }
        }
    }
}
