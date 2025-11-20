#nullable disable

namespace Diagram
{
    public class Plugin: IDiagramPlugin
    {

        private string location = null;
        private Log log = null;

        public string Name
        {
            get
            {
                return "Plugin";
            }
        }

        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        public void SetLocation(string location)
        {
            this.location = location;
        }

        public void SetLog(Log log)
        {
            this.log = log;
        }

    }
}
