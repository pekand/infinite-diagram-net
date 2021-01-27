using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagram
{
    public class DataStorage
    {
        private readonly IDictionary<string, IStorage> dataStorage = new Dictionary<string, IStorage>(); // data storage

        public IStorage GetStorage(string name)
        {
            if (dataStorage.ContainsKey(name)) {
                return dataStorage[name];
            }

            return null;
        }

        public void SetStorage(string name, IStorage storage)
        {
            dataStorage[name] = storage;
        }
    }
}
