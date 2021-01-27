using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diagram;

namespace Plugin
{
    public class FirstPlugin : INodeOpenPlugin, IKeyPressPlugin, IOpenDiagramPlugin //UID0290845814
    {
        private static long counter = 0;

        public string Name
        {
            get
            {
                return "First Plugin";
            }
        }

        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        private string location = null;

        public void SetLocation(string location)
        {
            this.location = location;
        }

        private Log log = null;

        public void SetLog(Log log)
        {
            this.log = log;
        }

        public bool ClickOnNodeAction(Diagram.Diagram diagram, DiagramView diagramview, Node node)
        {
            log.Write("Do Something in First Plugin:" + (counter++).ToString());

            return false;
        }

        public bool KeyPressAction(Diagram.Diagram diagram, DiagramView diagramview, Keys keyData)
        {
            log.Write("Do Something in First Plugin:" + (counter++).ToString());

            return false;
        }

        public void OpenDiagramAction(Diagram.Diagram diagram)
        {
            log.Write("Open diagram action fired from first plugin");
        }
    }
}
