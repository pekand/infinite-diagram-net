using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Diagram;
using System.Drawing;
using System.ComponentModel;

namespace DropPlugin
{
    public class DropPluginStorage : IStorage
    {
        public string saveDataDirectory = null;
    }

    public class DropPlugin : Plugin, IDropPlugin, ISavePlugin, ILoadPlugin, IPopupPlugin
    {
        System.Windows.Forms.ToolStripMenuItem selectDataDirectoryItem = null;
        System.Windows.Forms.ToolStripMenuItem removeDataDirectoryItem = null;

        public string Name
        {
            get
            {
                return "Drop Plugin";
            }
        }

        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        public DropPluginStorage getStorage(Diagram.Diagram diagram)
        {
            IStorage storage = diagram.dataStorage.GetStorage("DropPlugin");

            if (storage == null) {
                storage = new DropPluginStorage();
                diagram.dataStorage.SetStorage("DropPlugin", storage);
            }

            return storage as DropPluginStorage;
        }

        public bool DropAction(DiagramView diagramview, DragEventArgs e)
        {
            if (getStorage(diagramview.diagram).saveDataDirectory == null) {
                return false;
            }

            if (!Os.DirectoryExists(getStorage(diagramview.diagram).saveDataDirectory))
            {
                return false;
            }

            try
            {

                Nodes newNodes = new Nodes();

                string[] formats = e.Data.GetFormats();
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


                string datedir = Os.Combine(getStorage(diagramview.diagram).saveDataDirectory, DateTime.Now.ToString("yyyy-dd-MM"));
                Os.CreateDirectory(datedir);

                int counter = 1;
                while (Os.DirectoryExists(Os.Combine(datedir, counter.ToString().PadLeft(4, '0'))) && counter < 9999)
                {
                    counter++;
                }

                string dropdir = Os.Combine(datedir, counter.ToString().PadLeft(4, '0'));
                List<string> fileList = new List<string>();

                Os.CreateDirectory(dropdir);

                foreach (string file in files)
                {
                    Node newrec = diagramview.CreateNode(diagramview.GetMousePosition());
                    newNodes.Add(newrec);
                    newrec.SetName(Os.GetFileName(file));

                    if (Os.DirectoryExists(file) || Os.Exists(file)) // directory
                    {
                        fileList.Add(file);
                        newrec.link = Os.MakeRelative(dropdir, diagramview.diagram.FileName);
                    }
                }

                diagramview.diagram.Unsave("create", newNodes, null, diagramview.shift, diagramview.scale, diagramview.currentLayer.id);
                

                Job.DoJob(
                   new DoWorkEventHandler(
                       delegate (object o, DoWorkEventArgs args)
                       {

                           long fullSize = 0;
                           long transferedSize = 0;
                           long status = 0;

                           foreach (String file in fileList)
                           {
                               if (Os.IsFile(file)) {
                                   fullSize += Os.FileSize(file);
                               }

                               if (Os.IsDirectory(file))
                               {
                                   fullSize += Os.DirectorySize(file);
                               }
                           }

                           foreach (String file in fileList)
                           {
                              
                               Os.Copy(file, dropdir, (long count) => {
                                   transferedSize += count;
                                   if (fullSize > 0)
                                   {
                                       int currentStatus =(int)((double)transferedSize / (double)fullSize * 100.0);
                                       if (currentStatus != status) {
                                           status = currentStatus;
                                       }
                                   }
                               });
                           }
                       }
                   ),
                   new RunWorkerCompletedEventHandler(
                       delegate (object o, RunWorkerCompletedEventArgs args)
                       {                           
                           diagramview.Invalidate();
                       }
                   )
               );

                return true;
            }
            catch (Exception ex)
            {
                Program.log.Write("drop file goes wrong: error: " + ex.Message);
            }


            return false;
        }

        public bool SaveAction(Diagram.Diagram diagram, XElement root)
        {
            if (getStorage(diagram).saveDataDirectory == null)
            {
                return false;
            }

            //find plugin element if exists

            XElement plugins = null;
            foreach (XElement el in root.Descendants())
            {
                if (el.Name.ToString() == "plugins")
                {
                    plugins = el;
                }
            }

            if (plugins == null) {
                plugins = new XElement("plugins");
                root.Add(plugins);
            }

            
            XElement DropPlugin = new XElement("DropPlugin");
            DropPlugin.Add(new XElement("dataDirectory", getStorage(diagram).saveDataDirectory));
            plugins.Add(DropPlugin);
            
            return true;
        }

        public bool LoadAction(Diagram.Diagram diagram, XElement root)
        {

            foreach (XElement el in root.Descendants())
            {
                try
                {
                    if (el.Name.ToString() == "plugins")
                    {
                        foreach (XElement plugin in el.Descendants())
                        {
                            if (plugin.Name.ToString() == "DropPlugin")
                            {
                                foreach (XElement option in el.Descendants())
                                {
                                    if (option.Name.ToString() == "dataDirectory")
                                    {
                                        getStorage(diagram).saveDataDirectory = option.Value;
                                    }
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Program.log.Write("load xml diagram drop plugin: " + ex.Message);
                }
            }


            return false;
        }

        public void PopupAddItemsAction(DiagramView diagramView, ToolStripMenuItem pluginsItem)
        {
            this.selectDataDirectoryItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectDataDirectoryItem.Name = "selectDataDirectoryItem";
            this.selectDataDirectoryItem.Text = "Select data directory";
            this.selectDataDirectoryItem.Click += new System.EventHandler((sender, e) => this.SelectDataDirectoryItem_Click(sender, e, diagramView));

            pluginsItem.DropDownItems.Add(this.selectDataDirectoryItem);

            this.removeDataDirectoryItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeDataDirectoryItem.Name = "removeDataDirectoryItem";
            this.removeDataDirectoryItem.Text = "Remove data directory";
            this.removeDataDirectoryItem.Click += new System.EventHandler((sender, e) => this.RemoveDataDirectoryItem_Click(sender, e, diagramView));

            pluginsItem.DropDownItems.Add(this.removeDataDirectoryItem);
        }

        public void PopupOpenAction(DiagramView diagramView, ToolStripMenuItem pluginsItem)
        {
            this.selectDataDirectoryItem.Enabled = !diagramView.diagram.IsReadOnly();
            this.removeDataDirectoryItem.Enabled = !diagramView.diagram.IsReadOnly();
        }


        public void SelectDataDirectoryItem_Click(object sender, EventArgs e, Diagram.DiagramView diagramView)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    getStorage(diagramView.diagram).saveDataDirectory = folderDialog.SelectedPath;
                }
            }

        }

        public void RemoveDataDirectoryItem_Click(object sender, EventArgs e, Diagram.DiagramView diagramView)
        {
            getStorage(diagramView.diagram).saveDataDirectory = null;

        }
    }
}
