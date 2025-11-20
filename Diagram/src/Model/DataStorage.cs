using System.Xml.Linq;

#nullable disable

namespace Diagram
{
    public class DataStorage
    {
        private readonly  Dictionary<string, DataStorage> storages = [];
        private readonly  Dictionary<string, string> items = [];

        public DataStorage GetStorage(string key) 
        {
            if (storages.TryGetValue(key, out DataStorage value))
            {
                return value;
            }

            return AddStorage(key);
        }

        public DataStorage AddStorage(string key)
        {
            if (storages.TryGetValue(key, out DataStorage value))
            {
                return value;
            }

            DataStorage dataStorage = new();
            storages[key] = dataStorage;
            return dataStorage;
        }

        public DataStorage RemoveStorage(string key)
        {
            storages.Remove(key);

            return this;
        }

        public string GetItem(string key, string defaultValue = "")
        {
            if (items.TryGetValue(key, out string value)) {
                return value;
            }

            return defaultValue;            
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
