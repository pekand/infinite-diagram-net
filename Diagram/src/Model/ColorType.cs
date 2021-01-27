using System.Drawing;

namespace Diagram
{

    /// <summary>
    /// color type encapsulation</summary>
    public class ColorType //UID6485317568
    {
        public Color color = System.Drawing.Color.Black;

        /*************************************************************************************************************************/
        // CONSTRUCTORS

        public ColorType()
        {
        }

        public ColorType(string htmlColor)
        {
            this.color = System.Drawing.ColorTranslator.FromHtml(htmlColor);
        }

        public ColorType(Color color)
        {
            this.color = color;
        }

        public ColorType(ColorType colorType)
        {
            this.color = colorType.color;
        }

        /*************************************************************************************************************************/
        // SETERS AND GETERS

        public void Set(ColorType colorType)
        {
            this.color = colorType.color;
        }

        public void Set(string htmlColor)
        {
            this.color = System.Drawing.ColorTranslator.FromHtml(htmlColor);
        }

        public void Set(Color color)
        {
            this.color = color;
        }

        public Color Get()
        {
            return color;
        }

        public Color Invert()
        {
            return Color.FromArgb(this.color.ToArgb() ^ 0xFFFFFF);
        }


        /*************************************************************************************************************************/
        // CONVERSION

        /// <summary>
        /// convert system color to string</summary>
        public override string ToString()
        {
            return System.Drawing.ColorTranslator.ToHtml(color);
        }
    }
}
