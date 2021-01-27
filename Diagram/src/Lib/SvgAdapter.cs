using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Diagram
{
    class SvgAdapter
    {

        public static Bitmap getSvgImageFromResources(byte[] image, int width = 100, int height = 100) {

            Stream stream = new MemoryStream(image);

            StreamReader reader = new StreamReader(stream);
            string svgImage = reader.ReadToEnd();

            XmlDocument svgImageXml = new XmlDocument();
            svgImageXml.LoadXml(svgImage);

            SvgDocument svgDocument = Svg.SvgDocument.Open(svgImageXml);  
            svgDocument.ShapeRendering = SvgShapeRendering.Auto;

            Bitmap bmpImage = svgDocument.Draw(width, height);

            return bmpImage;
        }

        public static Bitmap getLockImage(int width = 100, int height = 100)
        {

            return SvgAdapter.getSvgImageFromResources(global::Diagram.Properties.Resources.lock_svg_image, width, height);
        }

    }
}
