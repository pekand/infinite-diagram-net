using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Diagram
{
    public class Serialization
    {
        public static IList<String> Deserialize(string data)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<String>));

                TextReader reader = new StringReader(data);

                object obj = deserializer.Deserialize(reader);

                reader.Close();

                return (List<String>)obj;
            }
            catch (Exception ex)
            {
                Program.log.Write("Deserialize: " + ex.Message);
            }

            return null;
        }

        public static string Serialize(IList<String> items)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<String>));

                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                serializer.Serialize(sw, items);
                return sw.GetStringBuilder().ToString();
            }
            catch (Exception ex)
            {
                Program.log.Write("Deserialize: " + ex.Message);
            }

            return null;
        }

        public static XElement ListToXElement(String rootName, List<String> items)
        {
            XElement root = new XElement(rootName);

            foreach (String item in items) {
                root.Add(new XElement("item", item));
            }

            return root;
        }

        public static List<String> XElementToList(XElement element)
        {
            List<String> items = new List<String>();
            
            foreach (XElement child in element.Elements())
            {
                items.Add(child.Value);
            }

            return items;
        }
    }
}
