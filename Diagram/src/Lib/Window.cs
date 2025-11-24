using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagram
{
    public class Window
    {
        public static void CenterForm(Form parent, Form child)
        {
            var x = parent.Left + (parent.Width - child.Width) / 2;
            var y = parent.Top + (parent.Height - child.Height) / 2;

            child.StartPosition = FormStartPosition.Manual;
            child.Location = new Point(x, y);
        }
    }
}
