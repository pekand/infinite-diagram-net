using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using Diagram;

#nullable disable

namespace Plugin
{
    public class CreateDiagramPlugin : IPopupPlugin
    {
        ToolStripMenuItem CreateDiagramItem = null;

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

        public void CreateDiagramItem_Click(object sender, EventArgs e, Diagram.DiagramView diagramview)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Diagram (*.diagram)|*.diagram";
            saveFileDialog.Title = "Save a Diagram File";

            string DiagramPath = diagramview.diagram.FileName.Trim();

            if (DiagramPath == "" && !Os.FileExists(DiagramPath))
            {
                saveFileDialog.InitialDirectory = DiagramPath;
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                using (FileStream fs = File.Create(filePath))
                {
                }

                Node newrec = diagramview.CreateNode(diagramview.actualMousePos.Clone());
                String filename = Path.GetFileNameWithoutExtension(filePath);
                newrec.SetName(filename);
                newrec.link = filePath;
                diagramview.diagram.Unsave("create", newrec, diagramview.shift, diagramview.currentLayer.id);

            }

        }

        public void PopupAddItemsAction(Diagram.DiagramView diagramview, ToolStripMenuItem pluginsItem)
        {
            this.CreateDiagramItem = new ToolStripMenuItem();
            this.CreateDiagramItem.Name = "CreateDiagram";
            this.CreateDiagramItem.Text = "Create Diagram";
            this.CreateDiagramItem.Click += new System.EventHandler((sender, e) => this.CreateDiagramItem_Click(sender, e, diagramview));

            pluginsItem.DropDownItems.Add(this.CreateDiagramItem);
        }

        public void PopupOpenAction(Diagram.DiagramView diagramview,ToolStripMenuItem pluginsItem)
        {
            this.CreateDiagramItem.Enabled = !diagramview.diagram.IsReadOnly();
        }
    }
}
