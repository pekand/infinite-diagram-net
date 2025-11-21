using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagram
{
    public  class RectangleD
    {
        public decimal x = 0;
        public decimal y = 0;
        public decimal width = 0;
        public decimal height = 0;
        public decimal scale = 0;

        public RectangleD(decimal x, decimal y, decimal width, decimal height, decimal scale)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.scale = scale;
        }
    }
}
