using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Linq;

namespace Diagram
{
    /// <summary>
    /// repository for font and text related functions</summary>
    public class Fonts //UID1751506871
    {
        /*************************************************************************************************************************/
        // MEASURE

        private static Bitmap bitmap = null; //bitmap for measure font size
        private static Graphics g = null; // graphic from bitmap for measure string size

        /// <summary>
        /// meassure s string size written in font</summary>
        public static SizeF MeasureString(string s, Font font)
        {

            if (bitmap == null)
            {
                bitmap = new Bitmap(1, 1);
                g = Graphics.FromImage(bitmap);
            }

            return g.MeasureString(s, font);
        }

       
        /*************************************************************************************************************************/
        // TEXT

        /// <summary>
        /// convert first character of input string to upper case</summary>
        public static string FirstCharToUpper(string input)
        {
            return input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower();
        }

        /*************************************************************************************************************************/
        // XML TO FONT CONVERSION

        /// <summary>
        /// convert xml element to Font object</summary>
        public static Font XmlToFont(XElement element)
        {

            string fontName = "";
            bool bold = false;
            bool italic = false;
            bool underline = false;
            bool strikeout = false;
            double fontSize = 12F;

            foreach (XElement el in element.Descendants())
            {
                try
                {
                    if (el.Name.ToString() == "name")
                    {
                        fontName = el.Value;
                    }

                    if (el.Name.ToString() == "size")
                    {
                        fontSize = float.Parse(el.Value.Replace(',', '.'));
                    }

                    if (el.Name.ToString() == "bold")
                    {
                        bold = bool.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "italic")
                    {
                        italic = bool.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "underline")
                    {
                        underline = bool.Parse(el.Value);
                    }

                    if (el.Name.ToString() == "strikeout")
                    {
                        strikeout = bool.Parse(el.Value);
                    }

                }
                catch (Exception ex)
                {
                    Program.log.Write("load xml font error: " + ex.Message);
                }
            }

            FontStyle fontStyle = FontStyle.Regular;

            if (bold)
            {
                fontStyle |= FontStyle.Bold;
            }

            if (italic)
            {
                fontStyle |= FontStyle.Italic;
            }

            if (underline)
            {
                fontStyle |= FontStyle.Underline;
            }

            if (strikeout)
            {
                fontStyle |= FontStyle.Strikeout;
            }

            Font font = new Font(fontName, (int)fontSize, fontStyle);
            return font;
        }

        /// <summary>
        /// convert Font object to xml </summary>
        public static XElement FontToXml(Font font, string name = "font")
        {
            XElement element = new XElement(name, new XAttribute("type", "font"));

            element.Add(new XElement("name", font.Name));
            element.Add(new XElement("size", font.Size.ToString().Replace(',', '.')));
            element.Add(new XElement("bold", font.Bold));
            element.Add(new XElement("italic", font.Italic));
            element.Add(new XElement("underline", font.Underline));
            element.Add(new XElement("strikeout", font.Strikeout));

            return element;
        }

        /*************************************************************************************************************************/
        // FONT

        /// <summary>
        /// compare fonts by attributes</summary>
        public static bool Compare(Font font1, Font font2)
        {
            if (font1.Name != font2.Name) return false;
            if (font1.Size != font2.Size) return false;
            if (font1.Style != font2.Style) return false;
            return true;
        }

        /// <summary>
        /// convert font to string</summary>
        public static string FontToString(Font font)
        {
            return TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(font);
        }

        /// <summary>
        /// convert font to string</summary>
        public static Font StringToFont(string font)
        {
            return (Font)TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(font);
        }

    }
}
