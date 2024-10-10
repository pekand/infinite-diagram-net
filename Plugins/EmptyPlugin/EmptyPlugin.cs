using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Diagram;

namespace Plugin
{
    // example of plugin UID0290845814
    public class EmptyPlugin : INodeOpenPlugin
    {
        

        public string Name
        {
            get
            {
                return "Empty Plugin";
            }
        }

        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        private string? location = null;

        public void SetLocation(string location)
        {
            this.location = location;
        }

        private Log? log = null;

        public void SetLog(Log log)
        {
            this.log = log;
        }

        public bool ClickOnNodeAction(Diagram.Diagram diagram, DiagramView diagramview, Node node)
        {
            return false;
        }

    }
}
