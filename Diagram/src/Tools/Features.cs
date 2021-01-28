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
            return global::Diagram.Properties.Resources.FeaturesList;         
        }
    }
}
