using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

#nullable disable

namespace Diagram
{
    public class DataStorage
    {
        IDictionary<string, DataStorage> storages = new Dictionary<string, DataStorage>();
        IDictionary<string, string> items = new Dictionary<string, string>();

        public DataStorage getStorage(string key) 
        {
            if (!this.storages.ContainsKey(key))
            {
                return this.addStorage(key);
            }

            return this.storages[key];
        }

        public DataStorage addStorage(string key)
        {
            if (this.storages.ContainsKey(key))
            {
                return this.storages[key];
            }

            DataStorage dataStorage = new DataStorage();
            this.storages[key] = dataStorage;
            return dataStorage;
        }

        public DataStorage removeStorage(string key)
        {
            if (!this.storages.ContainsKey(key))
            {
                return this;
            }

            this.storages.Remove(key);

            return this;
        }

        public string getItem(string key, string defaultValue = "")
        {
            if (!this.items.ContainsKey(key)) {
                return defaultValue;
            }

            return this.items[key];
        }

        public DataStorage addItem(string key, string value)
        {
            this.items[key] = value;
            return this;
        }

        public DataStorage removeItem(string key)
        {
            if (!this.items.ContainsKey(key))
            {
                return this;
            }

            this.items.Remove(key);

            return this;
        }

        public void fromXml(XElement root) {

            foreach (XElement element in root.Elements())
            {
                if (element.HasElements)
                {
                    DataStorage dataStorage = new DataStorage();
                    this.storages[element.Name.ToString()] = dataStorage;
                    dataStorage.toXml(element);
                }
                else {
                    this.items[element.Name.ToString()] = element.Value;
                }
            }
        }
        public XElement toXml(XElement root)
        {
            foreach (KeyValuePair<string, DataStorage> kvp in this.storages)
            {
                XElement storageElement = new XElement(kvp.Key);
                root.Add(kvp.Value.toXml(storageElement));
            }

            foreach (KeyValuePair<string, string> kvp in this.items)
            {
                root.Add(new XElement(kvp.Key, kvp.Value));
            }


            return root;
        }
    }
}
