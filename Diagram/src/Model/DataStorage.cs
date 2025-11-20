using System.Xml.Linq;

#nullable disable

namespace Diagram
{
    public class DataStorage
    {
        IDictionary<string, DataStorage> storages = new Dictionary<string, DataStorage>();
        IDictionary<string, string> items = new Dictionary<string, string>();

        public DataStorage GetStorage(string key) 
        {
            if (!storages.ContainsKey(key))
            {
                return AddStorage(key);
            }

            return storages[key];
        }

        public DataStorage AddStorage(string key)
        {
            if (storages.ContainsKey(key))
            {
                return storages[key];
            }

            DataStorage dataStorage = new();
            storages[key] = dataStorage;
            return dataStorage;
        }

        public DataStorage RemoveStorage(string key)
        {
            if (!storages.ContainsKey(key))
            {
                return this;
            }

            storages.Remove(key);

            return this;
        }

        public string GetItem(string key, string defaultValue = "")
        {
            if (!items.ContainsKey(key)) {
                return defaultValue;
            }

            return items[key];
        }

        public DataStorage AddItem(string key, string value)
        {
            items[key] = value;
            return this;
        }

        public DataStorage RemoveItem(string key)
        {
            if (!items.ContainsKey(key))
            {
                return this;
            }

            items.Remove(key);

            return this;
        }

        public void FromXml(XElement root) {

            foreach (XElement element in root.Elements())
            {
                if (element.HasElements)
                {
                    DataStorage dataStorage = new();
                    storages[element.Name.ToString()] = dataStorage;
                    dataStorage.ToXml(element);
                }
                else {
                    items[element.Name.ToString()] = element.Value;
                }
            }
        }
        public XElement ToXml(XElement root)
        {
            foreach (KeyValuePair<string, DataStorage> kvp in storages)
            {
                XElement storageElement = new(kvp.Key);
                root.Add(kvp.Value.ToXml(storageElement));
            }

            foreach (KeyValuePair<string, string> kvp in items)
            {
                root.Add(new XElement(kvp.Key, kvp.Value));
            }


            return root;
        }
    }
}
