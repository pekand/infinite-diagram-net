using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Diagram;

namespace Plugin
{
    // example of plugin UID0290845814
    public class FirstPlugin : IDropPlugin, IKeyPressPlugin, ILoadPlugin, INodeOpenPlugin, IOpenDiagramPlugin, IPopupPlugin, ISavePlugin
    {
        private int counter = 0;

        System.Windows.Forms.ToolStripMenuItem firstPluginItem = null;

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
            log.Write("FirstPlugin: Do Something in First Plugin:" + (counter++).ToString());

            return false;
        }

        public bool KeyPressAction(Diagram.Diagram diagram, DiagramView diagramview, Keys keyData)
        {
            log.Write("FirstPlugin: key press: "+ keyData.ToString());

            return false;
        }

        public void OpenDiagramAction(Diagram.Diagram diagram)
        {
            log.Write("FirstPlugin: Open diagram action");
            this.counter++;
        }

        public bool DropAction(DiagramView diagramview, DragEventArgs e)
        {
            log.Write("FirstPlugin: drop to diagram");
            string[] formats = e.Data.GetFormats();
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string format in formats)
            {
                log.Write("FirstPlugin: drop format " + format);
            }

            foreach (string file in files)
            {
                log.Write("FirstPlugin: drop file "+ file);
            }

            return true;
        }

        public bool LoadAction(Diagram.Diagram diagram, XElement root)
        {
            this.counter = Int32.Parse(diagram.dataStorage.getStorage("FirstPlugin").getItem("counter", "0"));
            
            log.Write("FirstPlugin: load diagram xml");
            foreach (XElement el in root.Descendants())
            {
                try
                {
                    if (el.Name.ToString() == "plugins")
                    {
                        foreach (XElement plugin in el.Descendants())
                        {
                            if (plugin.Name.ToString() == "FirstPlugin")
                            {
                                foreach (XElement firstPluginElement in el.Descendants())
                                {
                                    if (firstPluginElement.Name.ToString() == "counter")
                                    {
                                        this.counter = Int32.Parse(firstPluginElement.Value);
                                    }
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Program.log.Write("FirstPlugin: load xml: " + ex.Message);
                }
            }


            return false;
        }

        public bool SaveAction(Diagram.Diagram diagram, XElement root)
        {
            log.Write("FirstPlugin: save diagram xml");

            // second way to save data (if plugin is not installed this data are preserverd)
            diagram.dataStorage.addStorage("FirstPlugin").addItem("counter", this.counter.ToString());

            // second way to save data (if plugin is not installed this data are removed)
            XElement plugins = null;
            foreach (XElement el in root.Descendants())
            {
                if (el.Name.ToString() == "plugins")
                {
                    plugins = el;
                }
            }

            if (plugins == null)
            {
                plugins = new XElement("plugins");
                root.Add(plugins);
            }


            XElement FirstPlugin = new XElement("FirstPlugin");
            FirstPlugin.Add(new XElement("counter", this.counter.ToString()));
            plugins.Add(FirstPlugin);

            return true;
        }

        public void PopupAddItemsAction(DiagramView diagramView, ToolStripMenuItem pluginsItem)
        {
            log.Write("FirstPlugin: popup init");
            this.firstPluginItem = new System.Windows.Forms.ToolStripMenuItem();
            this.firstPluginItem.Name = "firstPluginItem";
            this.firstPluginItem.Text = "First plugin item";
            this.firstPluginItem.Click += new System.EventHandler((sender, e) => this.firstPluginItem_Click(sender, e, diagramView));
            pluginsItem.DropDownItems.Add(this.firstPluginItem);
        }

        public void PopupOpenAction(DiagramView diagramView, ToolStripMenuItem pluginsItem)
        {
            log.Write("FirstPlugin: popup shown");
            this.firstPluginItem.Enabled = !diagramView.diagram.IsReadOnly();
        }

        public void firstPluginItem_Click(object sender, EventArgs e, Diagram.DiagramView diagramView)
        {
            log.Write("FirstPlugin: popup item click");

        }

    }
}
