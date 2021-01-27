using System;
using System.Windows.Forms;
using Diagram;

namespace Plugin
{
    public class CreateDirectoryPlugin : IPopupPlugin
    {
        ToolStripMenuItem createDirectoryItem = null;

        public string Name
        {
            get
            {
                return "Create directory from diagram";
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

        public void OpenFileOnPosition(string file, long pos = 0)
        {
            Os.OpenFileOnPosition(file, pos);
        }

        public void CreateDirectoryItem_Click(object sender, EventArgs e, Diagram.DiagramView diagramview)
        {
            string DiagramPath = diagramview.diagram.FileName.Trim();

            if (DiagramPath == "" && !Os.FileExists(DiagramPath))
            {
                return;
            }

            string NewDirectoryPath = Os.Combine(Os.GetFileDirectory(DiagramPath), "test");

            Os.CreateDirectory(NewDirectoryPath);

            Node newrec = diagramview.CreateNode(diagramview.actualMousePos.Clone());

            newrec.SetName("test");
            newrec.link = NewDirectoryPath;
            diagramview.diagram.Unsave("create", newrec, diagramview.shift, diagramview.currentLayer.id);

        }

        public void PopupAddItemsAction(Diagram.DiagramView diagramview, ToolStripMenuItem pluginsItem)
        {
            this.createDirectoryItem = new ToolStripMenuItem();
            this.createDirectoryItem.Name = "editItem";
            this.createDirectoryItem.Text = "CreateDirectory";
            this.createDirectoryItem.Click += new System.EventHandler((sender, e) => this.CreateDirectoryItem_Click(sender, e, diagramview));

            pluginsItem.DropDownItems.Add(this.createDirectoryItem);
        }

        public void PopupOpenAction(Diagram.DiagramView diagramview,ToolStripMenuItem pluginsItem)
        {
            this.createDirectoryItem.Enabled = !diagramview.diagram.IsReadOnly();
        }
    }
}
