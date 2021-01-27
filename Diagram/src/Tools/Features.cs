using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Diagram
{
    public class Features
    {
        public static string GetFeatures() 
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Diagram.FeaturesList.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName)) {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        
    }
}
