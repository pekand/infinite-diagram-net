using Diagram.src.Forms;
using System.ComponentModel;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

#nullable disable

namespace Diagram
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class Popup : ContextMenuStrip 
    {
        public DiagramView diagramView = null;

        private readonly Dictionary<string, ToolStripMenuItem> items = [];
        private readonly Dictionary<string, ToolStripSeparator> separators = [];

        public ToolStripItem[] recentItems = null;
        private System.Windows.Forms.ToolStripMenuItem pluginItems = null;

        public Popup(System.ComponentModel.IContainer container, DiagramView diagramView) : base(container) 
        {
            this.diagramView = diagramView;

            InitializeComponent();

#if DEBUG
            items["consoleItem"].Visible = true;
            items["coordinatesItem"].Visible = true;
#endif
            items["restoreWindowItem"].Checked = this.diagramView.diagram.options.restoreWindow;
            items["gridItem"].Checked = this.diagramView.diagram.options.grid;
            items["bordersItem"].Checked = this.diagramView.diagram.options.borders;
            items["coordinatesItem"].Checked = this.diagramView.diagram.options.coordinates;
            items["readonlyItem"].Checked = this.diagramView.diagram.options.readOnly;
        }


        public ToolStripMenuItem GetPluginsItem()
        {
            return this.pluginItems;
        }



        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() 
        {
            this.SuspendLayout();

            this.CreateMenuItem("editItem", "Edit", null, this.EditItem_Click);
            this.CreateMenuItem("colorItem", "Color", null, this.ColorItem_Click);
            this.CreateMenuItem("linkItem", "Link", null, null);
            this.CreateMenuItem("alignItem", "Align", null, null);
            this.CreateMenuSeparator("quickActionSeparator", null);
            this.CreateMenuItem("copyItem", "Copy", null, this.CopyItem_Click);
            this.CreateMenuItem("cutItem", "Cut", null, this.CutItem_Click);
            this.CreateMenuItem("pasteItem", "Paste", null, this.PasteItem_Click);
            this.CreateMenuSeparator("editSeparator", null);
            this.CreateMenuItem("fileItem", "File", null, null);
            this.CreateMenuItem("editMenuItem", "Edit", null, null);
            this.CreateMenuItem("nodeItem", "Node", null, null);
            this.CreateMenuItem("lineItem", "Line", null, null);
            this.CreateMenuItem("imageItem", "Image", null, null);
            this.CreateMenuItem("attachmentItem", "Attachment", null, null);
            this.CreateMenuItem("viewItem", "View", null, null);
            this.CreateMenuItem("layerItem", "Layer", null, null);
            this.CreateMenuSeparator("helpSeparator", null);
            this.pluginItems = this.CreateMenuItem("pluginsItem", "Plugins", null, null);
            this.CreateMenuItem("toolsItem", "Tools", null, null);
            this.CreateMenuItem("optionItem", "Option", null, null);
            this.CreateMenuItem("helpItem", "Help", null, null);

            /// linkItem
            this.CreateMenuItem("openLinkItem", "Open", "linkItem", this.OpenLinkItem_Click);
            this.CreateMenuItem("openLinkDirectoryItem", "Open directory", "linkItem", this.OpenLinkDirectoryItem_Click);

            // alignItem             
            this.CreateMenuItem("leftItem", "Left", "alignItem", this.LeftItem_Click);
            this.CreateMenuItem("rightItem", "Right", "alignItem", this.RightItem_Click);
            this.CreateMenuItem("toLineItem", "To line", "alignItem", this.ToLineItem_Click);
            this.CreateMenuItem("inColumnItem", "In column", "alignItem", this.InColumnItem_Click);
            this.CreateMenuItem("groupVerticalItem", "Group vertical", "alignItem", this.GroupVerticalItem_Click);
            this.CreateMenuItem("groupHorizontalItem", "Group horizontal", "alignItem", this.GroupHorizontalItem_Click);
            this.CreateMenuItem("sortItem", "Sort", "alignItem", this.SortItem_Click);

            // fileItem
            this.CreateMenuItem("newItem", "New", "fileItem", this.NewItem_Click);
            this.CreateMenuItem("saveItem", "Save", "fileItem", this.SaveItem_Click);
            this.CreateMenuItem("saveAsItem", "Save As", "fileItem", this.SaveAsItem_Click);
            this.CreateMenuItem("exportItem", "Export", "fileItem", null);
            this.CreateMenuItem("openItem", "Open", "fileItem", this.OpenItem_Click);
            this.CreateMenuItem("recentItem", "Recent", "fileItem", null);
            this.CreateMenuItem("exitItem", "Exit", "fileItem", this.ExitItem_Click);

            // exportItem
            this.CreateMenuItem("textItem", "Text", "exportItem", this.TextItem_Click);
            this.CreateMenuItem("exportToPngItem", "Export to png", "exportItem", this.ExportToPngItem_Click);


            // editMenuItem
            this.CreateMenuItem("undoItem", "Undo", "editMenuItem", this.UndoItem_Click);
            this.CreateMenuItem("redoItem", "Redo", "editMenuItem", this.RedoItem_Click);
            this.CreateMenuItem("copyLinkItem", "Copy link", "editMenuItem", this.CopyLinkItem_Click);
            this.CreateMenuItem("copyNoteItem", "Copy note", "editMenuItem", this.CopyNoteItem_Click);
            this.CreateMenuItem("pasteToLinkItem", "Paste to link", "editMenuItem", this.PasteToLinkItem_Click);
            this.CreateMenuItem("pasteToNoteItem", "Paste to note", "editMenuItem", this.PasteToNoteItem_Click);

            // nodeItem
            this.CreateMenuItem("transparentItem", "Transparent", "nodeItem", this.TransparentItem_Click);
            this.CreateMenuItem("fontItem", "Font", "nodeItem", this.FontItem_Click);
            this.CreateMenuItem("fontColorItem", "Font color", "nodeItem", this.FontColorItem_Click);
            this.CreateMenuItem("editLinkItem", "Edit link", "nodeItem", this.EditLinkItem_Click);
            this.CreateMenuItem("bringTopItem", "Bring to top", "nodeItem", this.BringTopItem_Click);
            this.CreateMenuItem("bringBottomItem", "Bring to bottom", "nodeItem", this.BringBottomItem_Click);
            this.CreateMenuItem("removeShortcutItem", "Remove shortcut", "nodeItem", this.RemoveShortcutItem_Click);
            this.CreateMenuItem("protectItem", "Protect", "nodeItem", this.ProtectItem_Click);
            this.CreateMenuItem("transformTextNodeItem", "Transform", "nodeItem", this.TransformTextNodeItem_Click);
            this.CreateMenuItem("resetTransformItem2", "Reset transformation", "nodeItem", this.ResetTransformImageItem_Click);

            // lineItem
            this.CreateMenuItem("lineColorItem", "Color", "lineItem", this.LineColorItem_Click);
            this.CreateMenuItem("lineWidthItem", "Width", "lineItem", this.LineWidthItem_Click);

            // imageItem
            this.CreateMenuItem("imageAddItem", "Add image", "imageItem", this.ImageAddItem_Click);
            this.CreateMenuItem("imageRemoveItem", "Remove image", "imageItem", this.ImageRemoveItem_Click);
            this.CreateMenuItem("imageEmbeddedItem", "Embed image", "imageItem", this.ImageEmbeddedItem_Click);
            this.CreateMenuItem("imageTransformItem", "Transform image", "imageItem", this.TransformImageItem_Click);
            this.CreateMenuItem("resetTransformItem", "Reset transformation", "imageItem", this.ResetTransformImageItem_Click);

            // attachmentItem
            this.CreateMenuItem("deployAttachmentItem", "Deploy attachment", "attachmentItem", this.DeployAttachmentItem_Click);
            this.CreateMenuItem("includeFileItem", "Add file", "attachmentItem", this.IncludeFileItem_Click);
            this.CreateMenuItem("includeDirectoryItem", "Add directory", "attachmentItem", this.IncludeDirectoryItem_Click);
            this.CreateMenuItem("removeAttachmentItem", "Remove", "attachmentItem", this.RemoveFileItem_Click);

            // viewItem
            this.CreateMenuItem("newViewItem", "New View", "viewItem", this.NewViewItem_Click);
            this.CreateMenuItem("centerItem", "Center", "viewItem", this.CenterItem_Click);
            this.CreateMenuItem("setStartPositionItem", "Set start position", "viewItem", this.SetStartPositionItem_Click);
            this.CreateMenuItem("refreshItem", "Refresh", "viewItem", this.RefreshItem_Click);

            // layerItem
            this.CreateMenuItem("inItem", "In", "layerItem", this.InItem_Click);
            this.CreateMenuItem("outItem", "Out", "layerItem", this.OutItem_Click);

            // toolsItem
            this.CreateMenuItem("openDiagramDirectoryItem", "Open Directory", "toolsItem", this.OpenDiagramDirectoryItem_Click);
            this.CreateMenuItem("splitNodeItem", "Split node", "toolsItem", this.SplitNodeItem_Click);

            // optionItem
            this.CreateMenuItem("diagramOptionsItem", "Diagram", "optionItem", null);
            this.CreateMenuItem("optionTheme", "Theme and Colors", "optionItem", null);
            this.CreateMenuItem("optionImages", "Images", "optionItem", null);
            this.CreateMenuItem("optionSecurity", "Security", "optionItem", null);
            this.CreateMenuItem("settingsItem", "Settings", "optionItem", this.SettingsItem_Click);

            // diagramOptionsItem
            this.CreateMenuItem("readonlyItem", "Read only", "diagramOptionsItem", this.ReadonlyItem_Click);
            this.CreateMenuItem("alwaysOnTopItem", "Always on top", "diagramOptionsItem", this.AlwaysOnTopItem_Click);
            this.CreateMenuItem("pinWindowItem", "Pin window", "diagramOptionsItem", this.PinWindowItem_Click);
            this.CreateMenuItem("restoreWindowItem", "Remember window position", "diagramOptionsItem", this.RestoreWindowItem_Click);
            this.CreateMenuItem("openLayerInNewViewItem", "Open layer in new view", "diagramOptionsItem", this.OpenLayerInNewViewItem_Click);
            this.CreateMenuItem("openConfigDirItem", "Open config dir", "diagramOptionsItem", this.OpenConfigDirItem_Click);
            this.CreateMenuItem("openLastFileItem", "Open last file", "diagramOptionsItem", this.OpenLastFileItem_Click);
            this.CreateMenuItem("setAsDefaultDiagramItem", "Set as default Diagram", "diagramOptionsItem", this.SetAsDefaultDiagram_Click);

            // optionTheme
            this.CreateMenuItem("defaultFontItem", "Default font", "optionTheme", this.DefaultFontItem_Click);
            this.CreateMenuItem("resetFontItem", "Reset font", "optionTheme", this.ResetFontItem_Click);
            this.CreateMenuItem("setIconItem", "Set icon", "optionTheme", this.SetIconItem_Click);
            this.CreateMenuItem("setBackgroundItem", "Set background image", "optionTheme", this.SetBackgroundItem_Click);
            this.CreateMenuItem("setLineColorItem", "Line color", "optionTheme", this.SetLineColorItem_Click);
            this.CreateMenuItem("setNodeColorItem", "Node color", "optionTheme", this.SetNodeColorItem_Click);
            this.CreateMenuItem("selectedNodeColorItem", "Selected node color", "optionTheme", this.SetSelectedNodeColorItem_Click);
            this.CreateMenuItem("setBackgroundColorItem", "Background color", "optionTheme", this.SetBackgroundColorItem_Click);
            this.CreateMenuItem("setGridColorItem", "Grid color", "optionTheme", this.SetGridColorItem_Click);
            this.CreateMenuItem("setScrollbarColorItem", "Scrollbar color", "optionTheme", this.SetScrollbarColorItem_Click);
            this.CreateMenuItem("setSelectionColorItem", "Selection color", "optionTheme", this.SetSelectionColorItem_Click);
            this.CreateMenuItem("gridItem", "Grid", "optionTheme", this.GridItem_Click);
            this.CreateMenuItem("coordinatesItem", "Coordinates", "optionTheme", this.CoordinatesItem_Click);
            this.CreateMenuItem("bordersItem", "Borders", "optionTheme", this.BordersItem_Click);

            // optionImages
            this.CreateMenuItem("linkImageItem", "Link image", "optionImages", this.LinkImage_Click);
            this.CreateMenuItem("embedImageItem", "Embed image", "optionImages", this.EmbedImage_Click);
            this.CreateMenuItem("copyImageItem", "Copy image", "optionImages", this.CopyImage_Click);
            this.CreateMenuItem("copyImagePathItem", "Set copy image destination", "optionImages", this.CopyImagePath_Click);
            this.CreateMenuItem("embedImageConvertItem", "Convert embed images to files", "optionImages", this.EmbedImageConvertItem_Click);

            // optionSecurity
            this.CreateMenuItem("encryptItem", "Encrypt", "optionSecurity", this.EncryptItem_Click);
            this.CreateMenuItem("changePasswordItem", "Change password", "optionSecurity", this.ChangePasswordItem_Click);
            this.CreateMenuItem("takeOwnershipItem", "Take ownership", "optionSecurity", this.TakeOwnershipItem_Click);
            this.CreateMenuItem("lockItem", "Lock diagram", "optionSecurity", this.LockItem_Click);

            // helpItem
            this.CreateMenuItem("aboutItem", "About", "helpItem", this.AboutItem_Click);
            this.CreateMenuItem("consoleItem", "Debug Console", "helpItem", this.ConsoleItem_Click);
            this.CreateMenuItem("visitWebsiteItem", "Visit homesite", "helpItem", this.VisitWebsiteItem_Click);
            
            this.Name = "popupMenu";
            this.Opening += new System.ComponentModel.CancelEventHandler(this.PopupMenu_Opening);

            // add plugins to popup
            this.diagramView.main.plugins.PopupAddItemsAction(diagramView, this);


            //
            // Popup
            //
            this.ResumeLayout(false);
        }


        

        public ToolStripSeparator CreateMenuSeparator(string name, string parent)
        {
            separators.Add(name, new System.Windows.Forms.ToolStripSeparator());
            separators[name].Name = name;
            if (parent != null)
            {
                items[parent].DropDownItems.Add(separators[name]);
            }
            else
            {
                this.Items.Add(separators[name]);
            }

            return separators[name];
        }


        public System.Windows.Forms.ToolStripMenuItem CreateMenuItem(string name, string title, string parent, EventHandler click) {
            
            items.Add(name, new System.Windows.Forms.ToolStripMenuItem());

            items[name].Name = name;
            items[name].Text = title;

            if (click != null) {
                items[name].Click += new System.EventHandler(click);
            }

            if (parent != null)
            {
                items[parent].DropDownItems.Add(items[name]);
            }
            else {
                this.Items.Add(items[name]);
            }

            return items[name];
        }
        
        /*************************************************************************************************************************/

        // MENU Manage                                                                                // POPUP MENU After open
        public void PopupMenu_Opening(object sender, CancelEventArgs e) 
        {
            bool readOnly = this.diagramView.diagram.IsReadOnly();
            bool isNotReadOnly = !readOnly;
            bool isSelectedNoNode = this.diagramView.selectedNodes.Count == 0;
            bool isSelectedOneNode = this.diagramView.selectedNodes.Count == 1;
            bool isSelectedAtLeastOneNode = this.diagramView.selectedNodes.Count > 0;
            bool isSelectedMoreThenOneNode = this.diagramView.selectedNodes.Count > 1;

            items["editItem"].Visible = isNotReadOnly && isSelectedAtLeastOneNode;
            items["colorItem"].Visible = isNotReadOnly && isSelectedAtLeastOneNode;
            items["linkItem"].Visible = isNotReadOnly && isSelectedOneNode && this.diagramView.selectedNodes[0].link.Trim() != "";
            items["openLinkItem"].Enabled = isNotReadOnly && isSelectedOneNode && this.diagramView.selectedNodes[0].link.Trim() != "";
            items["openLinkDirectoryItem"].Visible = isSelectedOneNode 
                && this.diagramView.selectedNodes[0].link.Trim().Length > 0 
                && Os.FileExists(this.diagramView.selectedNodes[0].link);

            items["alignItem"].Visible = isNotReadOnly && isSelectedMoreThenOneNode;
            items["leftItem"].Enabled = isNotReadOnly && isSelectedMoreThenOneNode;
            items["rightItem"].Enabled = isNotReadOnly && isSelectedMoreThenOneNode;
            items["toLineItem"].Enabled = isNotReadOnly && isSelectedMoreThenOneNode;
            items["inColumnItem"].Enabled = isNotReadOnly && isSelectedMoreThenOneNode;
            items["groupVerticalItem"].Enabled = isNotReadOnly && isSelectedMoreThenOneNode;
            items["groupHorizontalItem"].Enabled = isNotReadOnly && isSelectedMoreThenOneNode;
            items["sortItem"].Enabled = isNotReadOnly && isSelectedMoreThenOneNode;

            separators["quickActionSeparator"].Visible = isSelectedAtLeastOneNode;

            items["copyItem"].Enabled = isSelectedAtLeastOneNode;
            items["cutItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode;
            items["pasteItem"].Enabled = isNotReadOnly;

            separators["editSeparator"].Enabled = true;

            items["fileItem"].Enabled = true;
            items["newItem"].Enabled = true;
            items["saveItem"].Enabled = isNotReadOnly;
            items["saveAsItem"].Enabled = true;
            items["exportItem"].Enabled = true;
            items["textItem"].Enabled = true;
            items["exportToPngItem"].Enabled = true;
            items["openItem"].Enabled = true;
            items["recentItem"].Enabled = true;
            items["exitItem"].Enabled = true;

            items["editMenuItem"].Enabled = true;
            items["undoItem"].Enabled = isNotReadOnly && this.diagramView.diagram.undoOperations.CanUndo();
            items["redoItem"].Enabled = isNotReadOnly && this.diagramView.diagram.undoOperations.CanRedo();

            items["copyLinkItem"].Enabled = (isSelectedOneNode && this.diagramView.selectedNodes[0].link.Trim() != "") || isSelectedMoreThenOneNode;
            items["copyNoteItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode;
            items["pasteToLinkItem"].Enabled = isNotReadOnly;
            items["pasteToNoteItem"].Enabled = isNotReadOnly;

            items["nodeItem"].Enabled = true;
            items["transparentItem"].Checked = isSelectedAtLeastOneNode && this.diagramView.IsSelectionTransparent();
            items["transparentItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode;
            items["fontItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode;
            items["fontColorItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode;
            items["editLinkItem"].Enabled = isNotReadOnly && isSelectedOneNode;
            items["bringTopItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode;
            items["bringBottomItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode;
            items["removeShortcutItem"].Visible = isNotReadOnly && isSelectedAtLeastOneNode;
            items["protectItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode;

            items["lineItem"].Enabled = true;
            items["lineColorItem"].Enabled = isNotReadOnly && isSelectedMoreThenOneNode;
            items["lineWidthItem"].Enabled = isNotReadOnly && isSelectedMoreThenOneNode;

            items["imageItem"].Enabled = true;
            items["imageAddItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode;
            items["imageRemoveItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode && this.diagramView.HasSelectionImage();
            items["imageEmbeddedItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode && this.diagramView.HasSelectionNotEmbeddedImage();
            items["imageTransformItem"].Enabled = isNotReadOnly && isSelectedOneNode && this.diagramView.HasSelectionImage();
            items["resetTransformItem"].Enabled = isNotReadOnly && isSelectedOneNode && this.diagramView.HasSelectionImage();
            
            items["attachmentItem"].Enabled = true;
            items["deployAttachmentItem"].Enabled = this.diagramView.HasSelectionAttachment();
            items["includeFileItem"].Enabled = isNotReadOnly;
            items["includeDirectoryItem"].Enabled = isNotReadOnly;
            items["removeAttachmentItem"].Enabled = isNotReadOnly && this.diagramView.HasSelectionAttachment();

            items["viewItem"].Enabled = true;
            items["newViewItem"].Enabled = true;
            items["centerItem"].Enabled = true;
            items["setStartPositionItem"].Enabled = isNotReadOnly;
            items["refreshItem"].Enabled = true;


            items["layerItem"].Enabled = true;
            items["inItem"].Enabled = isSelectedOneNode;
            items["outItem"].Enabled = this.diagramView.IsNotInFisrtLayer();

            separators["helpSeparator"].Enabled = true;

            items["pluginsItem"].Enabled = true;

            items["toolsItem"].Enabled = true;
            items["openDiagramDirectoryItem"].Enabled = !this.diagramView.diagram.IsNew();
            items["splitNodeItem"].Enabled = isNotReadOnly && isSelectedAtLeastOneNode;

            items["optionItem"].Enabled = true;
            items["diagramOptionsItem"].Enabled = true;
            items["readonlyItem"].Enabled = true;
            items["alwaysOnTopItem"].Checked = this.diagramView.diagram.options.alwaysOnTop;
            items["pinWindowItem"].Checked = this.diagramView.diagram.options.pinWindow;
            items["restoreWindowItem"].Enabled = isNotReadOnly;
            items["openLayerInNewViewItem"].Checked = this.diagramView.diagram.options.openLayerInNewView;
            items["openConfigDirItem"].Enabled = true;
            items["openLastFileItem"].Checked = this.diagramView.main.programOptions.openLastFile;
            items["setAsDefaultDiagramItem"].Checked = 
                this.diagramView.main.programOptions.defaultDiagram != "" && this.diagramView.main.programOptions.defaultDiagram == this.diagramView.diagram.FileName;

            items["optionTheme"].Enabled = true;
            items["defaultFontItem"].Enabled = isNotReadOnly;
            items["resetFontItem"].Enabled = isNotReadOnly;
            items["setIconItem"].Enabled = isNotReadOnly;
            items["setBackgroundItem"].Enabled = isNotReadOnly;
            items["setLineColorItem"].Enabled = isNotReadOnly;
            items["setNodeColorItem"].Enabled = isNotReadOnly;
            items["selectedNodeColorItem"].Enabled = isNotReadOnly;
            items["setBackgroundColorItem"].Enabled = isNotReadOnly;
            items["setGridColorItem"].Enabled = isNotReadOnly;
            items["setScrollbarColorItem"].Enabled = isNotReadOnly;
            items["setSelectionColorItem"].Enabled = isNotReadOnly;
            items["gridItem"].Enabled = isNotReadOnly;
            items["coordinatesItem"].Enabled = isNotReadOnly;
            items["bordersItem"].Enabled = isNotReadOnly;

            items["linkImageItem"].Checked = this.diagramView.diagram.options.linkImages;
            items["embedImageItem"].Checked = this.diagramView.diagram.options.embedImages;
            items["copyImageItem"].Checked = this.diagramView.diagram.options.copyImages;

            items["optionSecurity"].Enabled = true;
            items["encryptItem"].Enabled = isNotReadOnly && !this.diagramView.diagram.IsEncrypted();
            items["changePasswordItem"].Enabled = isNotReadOnly && this.diagramView.diagram.IsEncrypted();
            items["takeOwnershipItem"].Enabled = isNotReadOnly;
            items["lockItem"].Enabled = isNotReadOnly && this.diagramView.diagram.IsEncrypted();

            items["helpItem"].Enabled = true;
            items["consoleItem"].Enabled = true;
            items["visitWebsiteItem"].Enabled = true;
            items["aboutItem"].Enabled = true;


            // REMOVE SHORTCUT
            if (isNotReadOnly && isSelectedAtLeastOneNode)
            {
                bool hasShortcut = false;
                foreach (Node node in this.diagramView.selectedNodes)
                {
                    if (node.shortcut > 0)
                    {
                        hasShortcut = true;
                        break;
                    }
                }

                items["removeShortcutItem"].Visible = false;
                if (hasShortcut)
                {
                    items["removeShortcutItem"].Visible = true;
                }
            }

            // CLIPBOARD name by clipboard content
            DataObject retrievedData = (DataObject)Clipboard.GetDataObject();

            if (isNotReadOnly 
                && retrievedData.GetDataPresent("DiagramXml")
                || retrievedData.GetDataPresent(DataFormats.Text)
                || Clipboard.ContainsFileDropList()
                || Clipboard.GetDataObject() != null)
            {
                items["pasteItem"].Text = "Paste";
                items["pasteItem"].Enabled = true;

                if (retrievedData.GetDataPresent("DiagramXml"))
                {
                    items["pasteItem"].Text += " diagram";
                }
                else
                if (retrievedData.GetDataPresent(DataFormats.Text))
                {
                    items["pasteItem"].Text += " text";
                }
                else
                if (Clipboard.ContainsFileDropList())
                {
                    items["pasteItem"].Text += " files";
                }
                else
                if (Clipboard.GetDataObject() != null)
                {
                    IDataObject data = Clipboard.GetDataObject();
                    if (data.GetDataPresent(DataFormats.Bitmap))
                    {
                        items["pasteItem"].Text += " image";
                    }
                }
            }
            else
            {
                items["pasteItem"].Text = "Paste";
                items["pasteItem"].Enabled = false;
            }

            // create recent files items
            if (recentItems != null && recentItems.Length > 0) {
                items["recentItem"].DropDownItems.Clear();
            }

            recentItems = new System.Windows.Forms.ToolStripItem[this.diagramView.main.programOptions.recentFiles.Count];
            long i = 0;
            foreach (String path in this.diagramView.main.programOptions.recentFiles)
            {
                recentItems[i] = new ToolStripMenuItem
                {
                    Name = "recent" + i + "Item",
                    Text = Os.GetFileNameWithoutExtension(path),
                    Tag = path,
                };
                recentItems[i].Click += new System.EventHandler(this.RecentItem_Click);
                i++;
            }

            items["recentItem"].DropDownItems.AddRange(recentItems);

            if (recentItems.Length == 0) {
                items["recentItem"].Enabled = false;
            }
            
            this.diagramView.main.plugins.PopupOpenAction(diagramView, this);
        }

        // QUICK ACTIONS

        // MENU Edit
        public void EditItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count > 0)
            {
                foreach (Node node in this.diagramView.selectedNodes)
                {
                    this.diagramView.diagram.EditNode(node);
                }
            }
        }

        // MENU Change color
        public void ColorItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.SelectColor();
        }

        // LINK

        // MENU Link Open
        public void OpenLinkItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count > 0)
            {
                this.diagramView.OpenLink(this.diagramView.selectedNodes[0]);
            }
        }

        // MENU open directory for file in link
        private void OpenLinkDirectoryItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.OpenLinkDirectory();
        }

        // ALIGN

        // MENU align left
        private void LeftItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count > 0)
            {
                this.diagramView.diagram.AlignLeft(this.diagramView.selectedNodes);
                this.diagramView.diagram.Unsave();
                this.diagramView.diagram.InvalidateDiagram();
            }
        }
        
        // MENU align right
        private void RightItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count > 0)
            {
                this.diagramView.diagram.AlignRight(this.diagramView.selectedNodes);
                this.diagramView.diagram.Unsave();
                this.diagramView.diagram.InvalidateDiagram();
            }
        }

        // MENU align to line
        private void ToLineItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count > 0)
            {
                this.diagramView.diagram.AlignToLine(this.diagramView.selectedNodes);
                this.diagramView.diagram.Unsave();
                this.diagramView.diagram.InvalidateDiagram();
            }
        }

        // MENU align to column
        private void InColumnItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count > 0)
            {
                this.diagramView.diagram.AlignToColumn(this.diagramView.selectedNodes);
                this.diagramView.diagram.Unsave();
                this.diagramView.diagram.InvalidateDiagram();
            }
        }

        // MENU align to group to column
        private void GroupVerticalItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count > 0)
            {
                this.diagramView.diagram.AlignCompact(this.diagramView.selectedNodes);
                this.diagramView.diagram.Unsave();
                this.diagramView.diagram.InvalidateDiagram();
            }
        }

        // MENU align to group to column
        private void GroupHorizontalItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count > 0)
            {
                this.diagramView.diagram.AlignCompactLine(this.diagramView.selectedNodes);
                this.diagramView.diagram.Unsave();
                this.diagramView.diagram.InvalidateDiagram();
            }
        }

        // FILE
       
        // MENU export to txt
        private void TextItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.saveTextFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.diagramView.ExportDiagramToTxt(this.diagramView.saveTextFileDialog.FileName);
            }
        }

        // MENU export to png
        private void ExportToPngItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.exportFile.ShowDialog() == DialogResult.OK)
            {
                this.diagramView.ExportDiagramToPng();
            }
        }

        // MENU Open
        public void RecentItem_Click(object sender, EventArgs e) 
        {
            String path = (string)((ToolStripMenuItem)sender).Tag;
            this.diagramView.OpenDiagramFromFile(path);
        }

        // MENU sort items
        private void SortItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count > 0)
            {
                this.diagramView.diagram.SortNodes(this.diagramView.selectedNodes);
                this.diagramView.diagram.Unsave();
                this.diagramView.diagram.InvalidateDiagram();
            }
        }
        
        // EDIT

        // MENU Copy
        public void CopyItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.Copy();
        }

        // MENU cut
        public void CutItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.Cut();
        }

        // MENU paste
        public void PasteItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.Paste(new Position(this.diagramView.startMousePos));
        }
                
        // MENU New
        public void NewItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.main.OpenDiagram();
        }
        
        // MENU Save
        public void SaveItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.Save();
        }
        
        // MENU Save As
        public void SaveAsItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.Saveas();
        }
        
        // MENU Open
        public void OpenItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.OpenFileDialog();
        }
        
        // MENU Exit
        public void ExitItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.Close();
        }

        // MENU Undo
        public void UndoItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.DoUndo(this.diagramView);
        }

        // MENU Redo
        public void RedoItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.DoRedo(this.diagramView);
        }
        
        // MENU Copy link
        public void CopyLinkItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.CopyLink();
        }

        // MENU Copy note
        public void CopyNoteItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.CopyNote();
        }

        // MENU Copy link
        public void PasteToLinkItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.PasteToLink();
        }

        // MENU Copy note
        public void PasteToNoteItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.PasteToNote();
        }

        // NODE

        // MENU NODE transparent
        private void TransparentItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.MakeSelectionTransparent();
        }

        // MENU NODE set font
        private void FontItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.SelectFont();
        }

        // MENU NODE set font color
        private void FontColorItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.SelectFontColor();
        }

        // MENU NODE edit node link
        private void EditLinkItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.EditLink();
        }

        // MENU NODE edit node link
        private void BringTopItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.MoveNodesToForeground();
        }

        // MENU NODE edit node link
        private void BringBottomItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.MoveNodesToBackground();
        }

        // MENU remove shortcut
        private void RemoveShortcutItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count > 0)
            {
                this.diagramView.RemoveShortcuts(this.diagramView.selectedNodes);
            }
        }
        
        // MENU NODE protect sensitive data in node name
        private void ProtectItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.ProtectNodes();
        }

        // MENU NODE transform text nodes
        private void TransformTextNodeItem_Click(object sender, EventArgs e)
        {
            this.diagramView.TransformTextNode();
        }

        // MENU LINE select line color
        private void LineColorItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.ChangeLineColor();
        }

        // MENU LINE select line color
        private void LineWidthItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.ChangeLineWidth();
        }

        // IMAGE

        // MENU IMAGE add image
        private void ImageAddItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.AddImage();
        }

        // MENU IMAGE image remove from diagram
        private void ImageRemoveItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.RemoveImagesFromSelection();
        }

        // MENU IMAGE image embedded to diagram
        private void ImageEmbeddedItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.MakeImagesEmbedded();
        }

        // MENU IMAGE image embedded to diagram
        private void TransformImageItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.TransformImage();
        }

        // MENU IMAGE image embedded to diagram
        private void ResetTransformImageItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.ResetTransformImage();
        }

        // ATTACHMENT

        // MENU NODE deploy attachment to system
        private void DeployAttachmentItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.AttachmentDeploy();
        }

        // MENU NODE add file attachment to diagram
        private void IncludeFileItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.AttachmentAddFile(new Position(this.diagramView.startMousePos));
        }

        // MENU NODE add directory attachment to diagram
        private void IncludeDirectoryItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.AttachmentAddDirectory(new Position(this.diagramView.startMousePos));
        }

        // MENU NODE remove included data
        private void RemoveFileItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.AttachmentRemove();
        }

        // VIEW

        // MENU VIEW NEW VIEW
        private void NewViewItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.OpenDiagramView();
        }

        // MENU Center
        public void CenterItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.GoToHome();
        }

        // MENU set home position
        private void SetStartPositionItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.SetCurentPositionAsHomePosition();
        }

        // MENU refresh diagram
        private void RefreshItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.RefreshAll();
        }

        // LAYER

        // MENU Layer In
        public void InItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count == 1)
            {
                this.diagramView.LayerIn(this.diagramView.selectedNodes[0]);
            }
        }

        // MENU Layer Out
        public void OutItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.LayerOut();
        }

        // TOOLS

        // MENU Open Directory
        public void OpenDiagramDirectoryItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.OpenDiagramDirectory();
        }

        // MENU split node by lines
        public void SplitNodeItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.SplitNode();
        }

        // OPTIONS

        // MENU Images

        private void SettingsItem_Click(object sender, EventArgs e)
        {
            this.diagramView.diagram.ShowSettings(this.diagramView);
        }

        private void LinkImage_Click(object sender, EventArgs e)
        {
            if (!items["linkImageItem"].Checked) {
                items["linkImageItem"].Checked = true;
                items["embedImageItem"].Checked = false;
                items["copyImageItem"].Checked = false;

                this.diagramView.diagram.options.linkImages = true;
                this.diagramView.diagram.options.embedImages = false;
                this.diagramView.diagram.options.copyImages = false;

                this.diagramView.diagram.Unsave();
            }
        }

        private void EmbedImage_Click(object sender, EventArgs e)
        {
            if (!items["embedImageItem"].Checked)
            {
                items["linkImageItem"].Checked = false;
                items["embedImageItem"].Checked = true;
                items["copyImageItem"].Checked = false;

                this.diagramView.diagram.options.linkImages = false;
                this.diagramView.diagram.options.embedImages = true;
                this.diagramView.diagram.options.copyImages = false;

                this.diagramView.diagram.Unsave();
            }
        }

        private void CopyImage_Click(object sender, EventArgs e)
        {
            if (!items["copyImageItem"].Checked)
            {
                items["linkImageItem"].Checked = false;
                items["embedImageItem"].Checked = false;
                items["copyImageItem"].Checked = true;

                this.diagramView.diagram.options.linkImages = false;
                this.diagramView.diagram.options.embedImages = false;
                this.diagramView.diagram.options.copyImages = true;

                this.diagramView.diagram.Unsave();
            }
        }

        private void CopyImagePath_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();

            dialog.Description = "Select directory for images";
            dialog.ShowNewFolderButton = true;

            if (this.diagramView.diagram.options.copyImagesPath != "" && Directory.Exists(this.diagramView.diagram.options.copyImagesPath))
            {
                dialog.SelectedPath = this.diagramView.diagram.options.copyImagesPath;
            }
            else if (this.diagramView.diagram.FileName != "" && Os.FileExists(this.diagramView.diagram.FileName))
            {
                dialog.SelectedPath = Os.GetDirectoryName(this.diagramView.diagram.FileName);
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = dialog.SelectedPath;

                this.diagramView.diagram.options.copyImagesPath = selectedPath;
                if (this.diagramView.diagram.FileName != "" && Os.FileExists(this.diagramView.diagram.FileName))
                {
                    string relativePath = Os.MakeRelative(selectedPath, this.diagramView.diagram.FileName);
                    this.diagramView.diagram.options.copyImagesPath = relativePath;
                }

                items["linkImageItem"].Checked = false;
                items["embedImageItem"].Checked = false;
                items["copyImageItem"].Checked = true;

                this.diagramView.diagram.options.linkImages = false;
                this.diagramView.diagram.options.embedImages = false;
                this.diagramView.diagram.options.copyImages = true;

                this.diagramView.diagram.Unsave();
            }
        }

        private void EmbedImageConvertItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Do you really want to convert all included images in the file? This will reduce the file size.",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                if (this.diagramView.diagram.options.copyImagesPath != "" && Os.Exists(this.diagramView.diagram.options.copyImagesPath))
                {
                    this.diagramView.diagram.ConvertEmbedImageToFiles();
                }
                else {
                    MessageBox.Show(
                        "First set the path where the images will be saved (Options/Images/Set copy image destination).",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        // MENU Encrypt
        private void EncryptItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.diagram.SetPassword())
            {
                this.diagramView.diagram.Unsave();
            }
        }

        // MENU Change password
        private void ChangePasswordItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.diagram.ChangePassword())
            {
                this.diagramView.diagram.Unsave();
            }
        }

        // MENU take ownership
        private void TakeOwnershipItem_Click(object sender, EventArgs e)
        {
            this.diagramView.diagram.TakeOwnership(true);
            this.diagramView.diagram.SetTitle();
        }

        // MENU take ownership
        private void LockItem_Click(object sender, EventArgs e)
        {
            this.diagramView.diagram.LockDiagram();
        }

        // MENU Read only
        public void ReadonlyItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.options.readOnly = items["readonlyItem"].Checked;
        }

        // MENU Always on top
        public void AlwaysOnTopItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.options.alwaysOnTop = !this.diagramView.diagram.options.alwaysOnTop;
            this.diagramView.TopMost = this.diagramView.diagram.options.alwaysOnTop;
            items["alwaysOnTopItem"].Checked = this.diagramView.diagram.options.alwaysOnTop;

            foreach (DiagramView view in this.diagramView.diagram.DiagramViews)
            {
                view.TopMost = this.diagramView.diagram.options.alwaysOnTop;
            }
        }

        // MENU Pin window
        public void PinWindowItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.options.pinWindow = !this.diagramView.diagram.options.pinWindow;            
            items["pinWindowItem"].Checked = this.diagramView.diagram.options.alwaysOnTop;           
        }
        
        // MENU restore window position
        public void RestoreWindowItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.RememberPosition(items["restoreWindowItem"].Checked);
        }

        // MENU Grid check
        public void GridItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.options.grid = !items["gridItem"].Checked;
            this.diagramView.diagram.InvalidateDiagram();
        }

        // MENU coordinates
        public void CoordinatesItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.options.coordinates = !items["coordinatesItem"].Checked;
            this.diagramView.diagram.InvalidateDiagram();
        }

        // MENU Option Borders
        public void BordersItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.options.borders = items["bordersItem"].Checked;
            this.diagramView.diagram.InvalidateDiagram();
        }

        // MENU Option Default font
        public void DefaultFontItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.SelectDefaultFont();
        }

        // MENU reset font
        private void ResetFontItem_Click(object sender, EventArgs e) 
        {
            if (this.diagramView.selectedNodes.Count > 0)
            {
                this.diagramView.diagram.ResetFont(this.diagramView.selectedNodes);
            }
            else
            {
                this.diagramView.diagram.ResetFont(
                    this.diagramView.diagram.GetAllNodes(), 
                    this.diagramView.shift, 
                    this.diagramView.currentLayer.id
                );
            }
        }

        // MENU set diagram icon
        private void SetIconItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.SetIcon();
        }

        // MENU set diagram line color
        private void SetLineColorItem_Click(object sender, EventArgs e)
        {
            this.diagramView.diagram.SetLineColor();
        }

        // MENU set diagram node color
        private void SetNodeColorItem_Click(object sender, EventArgs e)
        {
            this.diagramView.diagram.SetNodeColor();
        }

        private void SetSelectedNodeColorItem_Click(object sender, EventArgs e)
        {
            this.diagramView.diagram.SetSelectedNodeColor();
        }

        // MENU set diagram background color
        private void SetBackgroundColorItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.SetBackgroundColor();
        }

        // MENU set diagram grid color
        private void SetGridColorItem_Click(object sender, EventArgs e)
        {
            this.diagramView.diagram.SetGridColor();
        }

        private void SetScrollbarColorItem_Click(object sender, EventArgs e)
        {
            this.diagramView.diagram.SetScrollbarColor();
        }


        private void SetSelectionColorItem_Click(object sender, EventArgs e)
        {
            this.diagramView.diagram.SetSelectionColor();
        }

        // MENU set diagram background image
        private void SetBackgroundItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.SetBackgroundImage();
        }

        // MENU set diagram icon
        private void OpenLayerInNewViewItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.diagram.options.openLayerInNewView = !this.diagramView.diagram.options.openLayerInNewView;
            items["openLayerInNewViewItem"].Checked = this.diagramView.diagram.options.openLayerInNewView;
            this.diagramView.diagram.Unsave();
        }
        
        // MENU reset font
        private void OpenConfigDirItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.main.OpenConfigDir();
        }

        // MENU reset font
        private void OpenLastFileItem_Click(object sender, EventArgs e) 
        {
            items["openLastFileItem"].Checked = !items["openLastFileItem"].Checked;
            this.diagramView.main.programOptions.openLastFile = items["openLastFileItem"].Checked;

            if (items["openLastFileItem"].Checked)
            {
                // cancel open last file if default diagram is set
                items["setAsDefaultDiagramItem"].Checked = false;
                this.diagramView.main.programOptions.defaultDiagram = "";
            }
        }

        private void SetAsDefaultDiagram_Click(object sender, EventArgs e) 
        {
            items["setAsDefaultDiagramItem"].Checked = !items["setAsDefaultDiagramItem"].Checked;


            if (items["setAsDefaultDiagramItem"].Checked)
            {
                

                if (this.diagramView.diagram.FileName != "") {
                    this.diagramView.main.programOptions.defaultDiagram = this.diagramView.diagram.FileName;
                }

                // cancel open last file if default diagram is set
                items["openLastFileItem"].Checked = false;
                this.diagramView.main.programOptions.openLastFile = false;
            } else {
                this.diagramView.main.programOptions.defaultDiagram = "";
            }
        }

        // HELP

        // MENU Console
        public void ConsoleItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.main.ShowConsole();
        }

        // MENU visit homepage
        private void VisitWebsiteItem_Click(object sender, EventArgs e) 
        {
            Network.OpenUrl(this.diagramView.main.programOptions.home_page);
        }

        // MENU show About form
        private void AboutItem_Click(object sender, EventArgs e) 
        {
            this.diagramView.main.ShowAbout();
        }
    }
}
