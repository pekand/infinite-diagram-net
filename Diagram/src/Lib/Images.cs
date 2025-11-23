using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagram.src.Lib
{
    public class Images
    {
        public static bool CanDraw(Image img)
        {
            try
            {
                using var testBmp = new Bitmap(1, 1);
                using var g = Graphics.FromImage(testBmp);
                g.DrawImage(img, new Rectangle(0, 0, 1, 1));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
