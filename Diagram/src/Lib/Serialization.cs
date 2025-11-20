using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

#nullable disable

namespace Diagram
{
    public class Serialization
    {


        /// <summary>
        /// deserialize string to  IList<String> </summary>
        public static IList<String> Deserialize(string data)
        {
            try
            {
                XmlSerializer deserializer = new(typeof(List<String>));

                StringReader reader = new(data);

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

        /// <summary>
        /// Serialize IList<String> to string</summary>
        public static string Serialize(IList<String> items)
        {
            try
            {
                XmlSerializer serializer = new(typeof(List<String>));

                StringBuilder sb = new();
                StringWriter sw = new(sb);
                serializer.Serialize(sw, items);
                return sw.GetStringBuilder().ToString();
            }
            catch (Exception ex)
            {
                Program.log.Write("Deserialize: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// List<String> to XElement </summary>
        public static XElement ListToXElement(String rootName, List<String> items)
        {
            XElement root = new(rootName);

            foreach (String item in items) {
                root.Add(new XElement("item", item));
            }

            return root;
        }

        /// <summary>
        /// XElement to List<String> </summary>
        public static List<String> XElementToList(XElement element)
        {
            List<String> items = [];
            
            foreach (XElement child in element.Elements())
            {
                items.Add(child.Value);
            }

            return items;
        }
    }
}
