using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Specialized;

#nullable disable

#if DEBUG
using System.Diagnostics;
#endif

namespace Diagram
{

    public partial class DiagramView : Form //UID5701725971
    {
        public Main main = null;
        public DiagramView parentView = null;

        public Popup PopupMenu;
        public SaveFileDialog DSave;
        public OpenFileDialog DOpen;
        public ColorDialog DColor;
        public ColorDialog DFontColor;
        public FontDialog DFont;
        public OpenFileDialog DImage;
        public System.Windows.Forms.Timer MoveTimer;
        public FontDialog defaultfontDialog;
        public SaveFileDialog exportFile;
        public SaveFileDialog saveTextFileDialog;
        public FolderBrowserDialog DSelectDirectoryAttachment;
        public OpenFileDialog DSelectFileAttachment;

        /*************************************************************************************************************************/

#if DEBUG
        // DEBUG TOOLS
        private string lastEvent = ""; // remember last event in console (for remove duplicate events)
#endif

        // ATRIBUTES SCREEN
        public Position shift = new Position();                   // left corner position

        public Position startShift = new Position();              // temporary left corner position before change in diagram

        // ATTRIBUTES MOUSE
        public Position startMousePos = new Position();           // start movse position before change, in view coordinates
        public Position startMousePosInDiagram = new Position();  // start movse position before change, in diagram coordinates
        public Position startNodePos = new Position();            // start node position before change
        public Position vmouse = new Position();                  // vector position in selected node before change
        public Position actualMousePos = new Position();          // actual mouse position in form in drag process

        // ATTRIBUTES KEYBOARDSTATES
        public char key = ' ';                   // last key character - for new node add
        public bool keyshift = false;            // actual shift key state
        public bool keyctrl = false;             // actual ctrl key state
        public bool keyalt = false;              // actual alt key state

        // ATTRIBUTES STATES
        public bool stateDragSelection = false;         // actual drag status
        public bool stateMoveView = false;              // actual move node status
        public bool stateSelectingNodes = false;        // actual selecting node status or creating node by drag
        public bool stateAddingNode = false;            // actual adding node by drag
        public bool stateDblclick = false;              // actual dblclick status
        public bool stateZooming = false;               // actual zooming by space status
        public bool stateSearching = false;             // actual search edit form status
        public bool stateSourceNodeAlreadySelected = false; // actual check if is clicket two time in same node for rename node
        public bool stateCoping = false;             // start copy part of diagram

        // ATTRIBUTES ZOOMING
        public Position zoomShift = new Position();// zoom view - left corner position before zoom space press
        public decimal zoomingDefaultScale = 0;      // zoom view - normal scale
        public decimal zoomingScale = 4;             // zoom view - scale in space preview
        public decimal currentScale = 0;             // zoom viev - scale before space zoom
        public decimal scale = 0;                    // zoom view - actual scale

        // ATTRIBUTES Diagram
        public Diagram diagram = null;             // diagram assigned to current view

        // ATTRIBUTES selected nodes
        public Node sourceNode = null;             // selected node by mouse
        public Nodes selectedNodes = new Nodes();  // all selected nodes by mouse

        // ATTRIBUTES copy nodes
        public Node copySourceNode = null;             // selected node by mouse for copy action
        public Nodes copySelectedNodes = new Nodes();  // all selected nodes by mouse for copy action
        public Lines copySelectedLines = new Lines();  // all selected nodes by mouse for copy action

        // ATTRIBUTES Layers
        public Layer currentLayer = null;
        public Position firstLayereShift = new Position();    // left corner position in zero top layer
        public decimal firstLayereScale = 0;
        public List<Layer> layersHistory = new List<Layer>(); // layer history - last layer is current selected layer

        // COMPONENTS
        public ScrollBar bottomScrollBar = null;  // bottom scroll bar
        public ScrollBar rightScrollBar = null;   // right scroll bar

        // EDITPANEL
        public EditPanel editPanel = null;         // edit panel for add new node name
        public EditLinkPanel editLinkPanel = null; // edit panel for node link

        // SEARCHPANEL
        public int lastFound = -1;            // id of last node found by search panel
        public string searchFor = "";         // string selected by search panel
        public SearchPanel searhPanel = null; // search panel
        public Position currentPosition = new Position();
        public long currentPositionLayer = 0;
        public decimal currentPositionScale = 0;
        public List<long> nodesSearchResult = new List<long>(); // all nodes found by search panel

        // MARKED NODES
        private long lastMarkNode = 0; // last marked node in navigation history

        // BREADCRUMBS
        public Breadcrumbs breadcrumbs = null;

        // MOVETIMER
        private readonly System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer(); //timer for all animations (goto node animation)
        private readonly Position animationTimerStep = new Position();
        private int animationTimerCounter = 0;

        // ZOOMTIMER
        private readonly System.Windows.Forms.Timer zoomTimer = new System.Windows.Forms.Timer(); //zooming animation
        public double zoomTimerScale = 1;
        public double zoomTimerStep = 0;

        // LINEWIDTHFORM
        private readonly LineWidthForm lineWidthForm = new LineWidthForm();

        // COLORPICKERFORM
        private readonly ColorPickerForm colorPickerForm = new ColorPickerForm();

        private FormWindowState oldFormWindowState = new FormWindowState();

        // COMPONENTS
        private IContainer components;

        public bool isFullScreen = false;

        // SECURITY
        public Image lockImage = null;

        // AFTER SHOW ONE TIME TIMER
        private System.Windows.Forms.Timer afterShownTimer = new System.Windows.Forms.Timer();

        // WINDOW PIN
        public bool windowIsPinned = false;
        public int windowWidthBeforePin = 0;
        public int windowHeightBeforePin = 0;
        public FormBorderStyle windowBorderStyleBeforePin;
        public PictureBox windowPinBox = new PictureBox();

        private void InitializeComponent() //UID4012344444
        {
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(DiagramView));
            DSave = new SaveFileDialog();
            DOpen = new OpenFileDialog();
            DColor = new ColorDialog();
            DFontColor = new ColorDialog();
            DFont = new FontDialog();
            DImage = new OpenFileDialog();
            MoveTimer = new System.Windows.Forms.Timer(components);
            defaultfontDialog = new FontDialog();
            exportFile = new SaveFileDialog();
            saveTextFileDialog = new SaveFileDialog();
            DSelectDirectoryAttachment = new FolderBrowserDialog();
            DSelectFileAttachment = new OpenFileDialog();
            SuspendLayout();
            // 
            // DSave
            // 
            DSave.DefaultExt = "*.diagram";
            DSave.Filter = "Diagram (*.diagram)|*.diagram";
            // 
            // DOpen
            // 
            DOpen.DefaultExt = "*.diagram";
            DOpen.Filter = "Diagram (*.diagram)|*.diagram";
            // 
            // DFont
            // 
            DFont.Color = SystemColors.ControlText;
            // 
            // DImage
            // 
            DImage.Filter = "All|*.bmp;*.jpg;*.jpeg;*.png;*.ico|Bmp|*.bmp|Jpg|*.jpg;*.jpeg|Png|*.png|Ico|*.ico";
            // 
            // MoveTimer
            // 
            MoveTimer.Interval = 5;
            MoveTimer.Tick += MoveTimer_Tick;
            // 
            // exportFile
            // 
            exportFile.DefaultExt = "*.png";
            exportFile.Filter = "Image (*.png) | *.png";
            // 
            // saveTextFileDialog
            // 
            saveTextFileDialog.DefaultExt = "*.txt";
            saveTextFileDialog.Filter = "Text file (*.txt)|*.txt";
            // 
            // DSelectFileAttachment
            // 
            DSelectFileAttachment.DefaultExt = "*.*";
            DSelectFileAttachment.Filter = "All files (*.*)|*.*";
            // 
            // DiagramView
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(511, 498);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(5, 4, 5, 4);
            Name = "DiagramView";
            Text = "Diagram";
            Activated += DiagramView_Activated;
            Deactivate += DiagramApp_Deactivate;
            FormClosing += DiagramApp_FormClosing;
            FormClosed += DiagramView_FormClosed;
            Load += DiagramViewLoad;
            Shown += DiagramView_Shown;
            DragDrop += DiagramApp_DragDrop;
            DragEnter += DiagramApp_DragEnter;
            Paint += DiagramApp_Paint;
            KeyDown += DiagramApp_KeyDown;
            KeyPress += DiagramApp_KeyPress;
            KeyUp += DiagramApp_KeyUp;
            MouseDoubleClick += DiagramApp_MouseDoubleClick;
            MouseDown += DiagramApp_MouseDown;
            MouseMove += DiagramApp_MouseMove;
            MouseUp += DiagramApp_MouseUp;
            MouseWheel += DiagramApp_MouseWheel;
            Resize += DiagramApp_Resize;

            // WINDOW PIN
            this.Controls.Add(windowPinBox);
            windowPinBox.Size = new Size(64, 64);
            windowPinBox.Location = new Point(0, 0);
            windowPinBox.SizeMode = PictureBoxSizeMode.StretchImage;            
            windowPinBox.Visible = false;

            ResumeLayout(false);

        }

        /*************************************************************************************************************************/

        // FORM Constructor UID0302011231
        public DiagramView(Main main, Diagram diagram, DiagramView parentView = null)
        {
            this.main = main;
            this.diagram = diagram;
            this.parentView = parentView;

            this.InitializeComponent();

            // initialize popup menu
            this.PopupMenu = new Popup(this.components, this);

            // initialize edit panel
            this.editPanel = new EditPanel(this);
            this.Controls.Add(this.editPanel);

            // initialize edit link panel
            this.editLinkPanel = new EditLinkPanel(this);
            this.Controls.Add(this.editLinkPanel);

            // initialize breadcrumbs
            this.breadcrumbs = new Breadcrumbs(this);

            // initialize layer history
            this.currentLayer = this.diagram.layers.GetLayer(0);
            this.BuildLayerHistory(0);

            // move timer
            this.animationTimer.Tick += new EventHandler(AnimationTimer_Tick);
            this.animationTimer.Interval = 10;
            this.animationTimer.Enabled = false;

            // move timer
            this.zoomTimer.Tick += new EventHandler(ZoomTimer_Tick);
            this.zoomTimer.Interval = 1;
            this.zoomTimer.Enabled = false;

            // lineWidthForm
            this.lineWidthForm.TrackbarStateChanged += this.ResizeLineWidth;

            //colorPickerForm
            this.colorPickerForm.ChangeColor += this.ChangeColor;

            // custom diagram icon
            if (this.diagram.options.icon != "")
            {
                this.Icon = Media.StringToIcon(this.diagram.options.icon);
                if (windowPinBox.Image != null)
                {
                    windowPinBox.Image.Dispose();
                }

                windowPinBox.Image = this.Icon.ToBitmap();
            }

            this.BackColor = this.diagram.options.backgroundColor.Get();



            // Load image for lock file (padlock) from resources
            byte[] imageData = global::Diagram.Properties.Resources.Lock;
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                this.lockImage = Image.FromStream(ms);
            }


            WinProcess.setId();
        }

        // FORM Load event - UID0112423443
        public void DiagramViewLoad(object sender, EventArgs e)
        {
            // predefined window position
            if (this.diagram.options.restoreWindow)
            {
                // RESTORE_WINDOW_POSITION_AND_SIZE

                this.Left = (int)this.diagram.options.Left;
                this.Top = (int)this.diagram.options.Top;
                this.Width = (int)this.diagram.options.Width;
                this.Height = (int)this.diagram.options.Height;
                this.SetWindowsStateCode((int)this.diagram.options.WindowState);
            }
            else
            {
                // DEFAULT_WINDOW_POSITION_AND_SIZE
                this.Left = 50;
                this.Top = 40;

                int screenWidth = Media.ScreenWorkingAreaWidth(this);
                int sreenHeight = Media.ScreenWorkingAreaHeight(this);

                int windowWidth = screenWidth - 150;
                int windowHeight = sreenHeight - 150;

                if (screenWidth > 1920)
                {
                    windowWidth = 1920;
                }

                if (sreenHeight > 1080)
                {
                    windowHeight = 1080;
                }

                this.Width = windowWidth;
                this.Height = windowHeight;

                this.Left = (screenWidth - windowWidth) / 2;
                this.Top = (sreenHeight - windowHeight) / 2;

                this.WindowState = FormWindowState.Normal;

                this.CenterToScreen();
            }

            // scrollbars
            bottomScrollBar = new ScrollBar(this, this.ClientRectangle.Width, this.ClientRectangle.Height, true);
            rightScrollBar = new ScrollBar(this, this.ClientRectangle.Width, this.ClientRectangle.Height, false);

            bottomScrollBar.SetColor(this.diagram.options.scrollbarColor.Get());
            rightScrollBar.SetColor(this.diagram.options.scrollbarColor.Get());

            bottomScrollBar.OnChangePosition += new PositionChangeEventHandler(PositionChangeBottom);
            rightScrollBar.OnChangePosition += new PositionChangeEventHandler(PositionChangeRight);

            // set startup position
            if (this.parentView != null)
            {
                this.shift.Set(this.parentView.shift);
                this.GoToLayer(this.parentView.currentLayer.id);
                this.Width = this.parentView.Width;
                this.Height = this.parentView.Height;
                this.Top = this.parentView.Top;
                this.Left = this.parentView.Left;
                this.WindowState = this.parentView.WindowState;
            }
            else
            {
                this.GoToHome();
            }

            // custom diagram background image
            if (this.diagram.options.backgroundImage != null)
            {
                this.diagram.RefreshBackgroundImages();
            }

            oldFormWindowState = FormWindowState.Normal;

            
        }

        // FORM Shown event
        private void DiagramView_Shown(object sender, EventArgs e)
        {
            afterShownTimer.Interval = 1000;
            afterShownTimer.Tick += AfterShownTick;
            afterShownTimer.Start();
        }

        private void AfterShownTick(object sender, EventArgs e)
        {
            // top most
            if (this.diagram.options.alwaysOnTop)
            {
                foreach (DiagramView view in this.diagram.DiagramViews) {
                    view.TopMost = this.diagram.options.alwaysOnTop;
                }

                this.TopMost = this.diagram.options.alwaysOnTop;
            }

            afterShownTimer.Stop();
        }

        // FORM Quit Close
        public void DiagramApp_FormClosing(object sender, FormClosingEventArgs e) //UID8741811919
        {
            bool close = this.diagram.CloseDiagramViewWithDialog(this);

            if (close)
            {
                this.main.ShowFirstHiddenView(this);
            }

            e.Cancel = !close;
        }

        // FORM CLOSE UID2411004144
        private void DiagramView_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.diagram.CloseView(this);
        }

        // FORM Title - set windows title
        public void SetTitle()
        {
            if (this.diagram.FileName.Trim() != "")
                this.Text = Os.GetFileNameWithoutExtension(this.diagram.FileName);
            else
                this.Text = "Diagram";
            if (this.currentLayer.parentNode != null && this.currentLayer.parentNode.name.Trim() != "")
                this.Text += " - " + this.currentLayer.parentNode.name.Trim();
            if (!this.diagram.SavedFile)
                this.Text = "*" + this.Text;
        }

        // FORM go to home position - center window to home position UID5474632736
        public void GoToHome()
        {
            Nodes nodes = this.diagram.layers.SearchInAllNodes("@home");
            nodes.OrderByIdAsc();

            Node homenode = null;
            foreach (Node node in nodes)
            {
                if (node.link == "@home")
                {
                    homenode = node;
                }
            }

            if (homenode != null)
            {
                this.GoToNode(homenode);
            }
            else
            {
                this.shift.Set(diagram.options.homePosition);
                this.scale = diagram.options.homeScale;
                this.GoToLayer(diagram.options.homeLayer);
            }

            this.diagram.InvalidateDiagram();
        }

        // FORM open view and go to home position
        public void OpenViewAndGoToHome() //UID2147390186
        {
            DiagramView child = this.diagram.OpenDiagramView(this);
            child.GoToHome();
            child.Invalidate();
        }

        // FORM set home position
        public void SetCurentPositionAsHomePosition()
        {
            diagram.options.homePosition.x = this.shift.x;
            diagram.options.homePosition.y = this.shift.y;
            diagram.options.endScale = this.scale;
            diagram.options.homeLayer = this.currentLayer.id;
            this.diagram.Unsave();
        }

        // FORM go to end position - center window to second remembered position
        public void GoToEnd()
        {
            Nodes nodes = this.diagram.layers.SearchInAllNodes("@end");
            nodes.OrderByIdAsc();

            Node homenode = null;
            foreach (Node node in nodes)
            {
                if (node.link == "@end")
                {
                    homenode = node;
                }
            }

            if (homenode != null)
            {
                this.GoToNode(homenode);
            }
            else
            {
                this.shift.Set(diagram.options.endPosition);
                this.scale = diagram.options.endScale;
                this.GoToLayer(diagram.options.endLayer);
            }

            this.diagram.InvalidateDiagram();
        }

        // FORM open view and go to home position
        public void OpenViewAndGoToEnd() //UID0905369008
        {
            DiagramView child = this.diagram.OpenDiagramView(this);
            child.GoToEnd();
            child.Invalidate();
        }

        // FORM set end position
        public void SetCurentPositionAsEndPosition()
        {
            diagram.options.endPosition.x = this.shift.x;
            diagram.options.endPosition.y = this.shift.y;
            diagram.options.endScale = this.scale;
            diagram.options.endLayer = this.currentLayer.id;
            this.diagram.Unsave();
        }

        // FORM cursor position
        public Position CursorPosition()
        {
            Point ptCursor = Cursor.Position;
            ptCursor = PointToClient(ptCursor);
            return new Position(ptCursor.X, ptCursor.Y);
        }

        // FORM hide
        public bool FormHide()
        {
            if (!this.diagram.SavedFile && this.diagram.FileName != "")
            {
                this.diagram.SaveXMLFile(this.diagram.FileName);
                this.diagram.NewFile = false;
                this.diagram.SavedFile = true;
            }
            this.WindowState = FormWindowState.Minimized;
            return true;
        }

        // FORM remember current window position an restore when diagram is opened
        public void RememberPosition(bool state = true)
        {
            this.diagram.options.restoreWindow = state;

            this.diagram.options.Left = this.Left;
            this.diagram.options.Top = this.Top;
            this.diagram.options.Width = this.Width;
            this.diagram.options.Height = this.Height;

            this.diagram.options.WindowState = this.GetWindowsStateCode();
        }

        // FORM get window state role
        public int GetWindowsStateCode()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                return 1;
            }

            if (this.WindowState == FormWindowState.Normal)
            {
                return 2;
            }


            if (this.WindowState == FormWindowState.Minimized)
            {
                return 3;
            }

            return 0;
        }

        // FORM get window state role
        public void SetWindowsStateCode(int code = 0)
        {
            if (code == 1)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            if (code == 2)
            {
                this.WindowState = FormWindowState.Normal;
            }


            if (code == 3)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        // FORM set icon
        public void SetIcon()
        {
            // #icon
            OpenFileDialog openIconDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.bmp, *.ico, *.png, *.gif, *.tiff)| *.jpg; *.jpeg; *.bmp; *.ico; *.png; *.gif; *.tiff",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openIconDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Icon icon = null;

                    if (Os.GetExtension(openIconDialog.FileName).ToLower() == ".ico")
                    {
                        icon = Media.GetIcon(openIconDialog.FileName);
                    }
                    else
                    {
                        Bitmap image = Media.GetImage(openIconDialog.FileName);
                        IntPtr Hicon = image.GetHicon();
                        icon = Icon.FromHandle(Hicon);
                    }

                    this.Icon = icon;
                    this.diagram.options.icon = Media.IconToString(icon);
                    if (windowPinBox.Image != null)
                    {
                        windowPinBox.Image.Dispose();
                    }
                    windowPinBox.Image = this.Icon.ToBitmap();

                    this.diagram.Unsave();
                }
                catch (Exception e)
                {
                    Program.log.Write("DiagramView: setIcon: error:" + e.Message);
                }
            }
            else
            {
                if (this.diagram.options.icon != "" && MessageBox.Show(Translations.confirmRemoveIconQuestion, Translations.confirmRemoveIcon, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
#if DEBUG
                    this.Icon = global::Diagram.Properties.Resources.ico_diagram_debug;
#else
                    this.Icon = global::Diagram.Properties.Resources.ico_diagram;
#endif
                    this.diagram.options.icon = "";
                    this.diagram.Unsave();
                }
            }

            openIconDialog.Dispose();
        }

        // FORM set icon
        public void SetBackgroundColor(Color color)
        {
            this.BackColor = color;
            this.Invalidate();
        }

        // FORM set icon
        public void SetBackgroundImage()
        {
            OpenFileDialog openIconDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.bmp, *.ico, *.png, *.gif, *.tiff)| *.jpg; *.jpeg; *.bmp; *.ico; *.png; *.gif; *.tiff",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openIconDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap background = Media.GetImage(openIconDialog.FileName);
                    string backgroundAsString = Media.ImageToString(background); // convert to string to speed up background rendering (convert to jpeg)
                    background.Dispose();
                    this.diagram.options.backgroundImage = Media.StringToImage(backgroundAsString);
                    this.diagram.RefreshBackgroundImages();
                    this.diagram.Unsave();
                }
                catch (Exception e)
                {
                    Program.log.Write("DiagramView: setIcon: error:" + e.Message);
                }
            }
            else
            {
                //#background
                if (this.diagram.options.backgroundImage != null && MessageBox.Show(Translations.confirmRemoveBackgroundQuestion, Translations.confirmRemoveBackground, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.diagram.options.backgroundImage = null;
                    this.diagram.RefreshBackgroundImages();
                    this.diagram.Unsave();
                }
            }

            openIconDialog.Dispose();
        }

        public void RefreshBackgroundImage()
        {
            this.BackgroundImage = this.diagram.options.backgroundImage;
        }

        /*************************************************************************************************************************/

        // SELECTION Clear selection
        public void ClearSelection()
        {
            if (this.selectedNodes.Count() > 0) // odstranenie mulitvyberu
            {
                foreach (Node rec in this.selectedNodes)
                {
                    rec.selected = false;
                }

                this.selectedNodes.Clear();
            }
        }

        // SELECTION Remove Node from  selection UID2688115953
        public void RemoveNodeFromSelection(Node a)
        {
            if (this.selectedNodes.Count() > 0 && a != null) // odstranenie mulitvyberu
            {
                for (int i = this.selectedNodes.Count() - 1; i >= 0; i--) // Loop through List with foreach
                {
                    if (this.selectedNodes[i] == a)
                    {
                        this.selectedNodes[i].selected = false;
                        this.selectedNodes.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        // SELECTION Clear Selection and Select node
        public void SelectOnlyOneNode(Node rec)
        {
            this.ClearSelection();
            this.SelectNode(rec);
        }

        // SELECTION Select node
        public void SelectNode(Node rec)
        {
            if (rec != null)
            {
                rec.selected = true;
                this.selectedNodes.Add(rec);
            }
        }

        // SELECTION Clear Selection and Select nodes
        public void SelectNodes(Nodes nodes)
        {
            this.ClearSelection();
            foreach (Node rec in nodes)
            {
                if (rec.layer == this.currentLayer.id)
                {
                    this.SelectNode(rec);
                }
            }
            this.diagram.InvalidateDiagram();
        }

        // SELECTION selsect all
        public void SelectAll()
        {
            this.SelectNodes(this.currentLayer.nodes);
        }

        /*************************************************************************************************************************/

        // EVENTS

        // EVENT Paint UID5225221692                                                                   // [PAINT] [EVENT]
        public void DiagramApp_Paint(object sender, PaintEventArgs e)
        {
            this.DrawDiagram(e.Graphics);
        }

        // EVENT Mouse DoubleClick UID5891984730
        public void DiagramApp_MouseDoubleClick(object sender, MouseEventArgs e)                       // [MOUSE] [DBLCLICK] [EVENT] UID3769224705
        {

#if DEBUG
            this.LogEvent("MouseDoubleClick");
#endif

            this.stateDblclick = true;
        }

        // EVENT Mouse Down UID3419722424                                                              // [MOUSE] [DOWN] [EVENT]
        public void DiagramApp_MouseDown(object sender, MouseEventArgs e)
        {
#if DEBUG
            this.LogEvent("MouseDown");
#endif

            bool buttonleft = e.Button == MouseButtons.Left;
            bool buttonright = e.Button == MouseButtons.Right;
            bool buttonmiddle = e.Button == MouseButtons.Middle;
            bool isreadonly = this.diagram.IsReadOnly();
            bool keyalt = this.keyalt;
            bool keyctrl = this.keyctrl;
            bool keyshift = this.keyshift;

            this.actualMousePos.Set(e.X, e.Y);

            if (this.stateSearching)
            {
                this.stateSearching = false;
                this.searhPanel.HidePanel();
            }

            this.Focus();

            if (this.IsEditing())
            {
                this.ActiveControl = null;
            }

            if (this.editPanel.editing) // close edit panel after mouse click to form
            {
                bool selectNode = false;
                this.editPanel.SaveNodeNamePanel(selectNode);
            }
            else
            if (this.editLinkPanel.editing) // close link edit panel after mouse click to form
            {
                bool selectNode = false;
                this.editLinkPanel.SaveNodeLinkPanel(selectNode);
            }

            this.startMousePos.Set(this.actualMousePos);  // starting mouse position
            this.startMousePosInDiagram.Set(this.MouseToDiagramPosition(this.actualMousePos));  // starting mouse position in diagram coordinates
            this.startShift.Set(this.shift);  // starting indent

            this.sourceNode = this.FindNodeInMousePosition(this.actualMousePos);

            this.stateSourceNodeAlreadySelected = this.sourceNode != null && this.sourceNode.selected;


            if (buttonleft && bottomScrollBar != null && bottomScrollBar.MouseDown(e.X, e.Y))
            {
                MoveScreenHorizontal(bottomScrollBar.position);
                this.diagram.InvalidateDiagram();
                return;
            }

            if (buttonleft && rightScrollBar != null && rightScrollBar.MouseDown(e.X, e.Y))
            {
                MoveScreenVertical(rightScrollBar.position);
                this.diagram.InvalidateDiagram();
                return;
            }

            if (buttonleft && this.sourceNode == null)
            {
                if (!isreadonly
                    && ((!this.keyctrl && this.keyalt) || (this.keyctrl && this.keyalt))
                    && !this.keyshift) // add node by drag
                {
                    this.stateAddingNode = true;
                    MoveTimer.Enabled = true;
                    this.ClearSelection();
                }
                else // multiselect
                {
                    this.stateSelectingNodes = true;
                    MoveTimer.Enabled = true;
                }
            }

            if (buttonleft && this.sourceNode != null)
            {
                if (this.keyshift && !this.keyctrl && !this.keyalt
                    && this.sourceNode.link.Trim() != ""
                    && (Os.FileExists(this.sourceNode.link) || Os.DirectoryExists(this.sourceNode.link))) // drag file from diagram
                {
                    string[] array = { this.sourceNode.link };
                    var data = new DataObject(DataFormats.FileDrop, array);
                    this.DoDragDrop(data, DragDropEffects.Copy);
                }
                else if (!isreadonly && !this.stateDblclick)  //informations for draging
                {

                    if (!this.keyctrl && !this.keyshift && !this.keyalt && !this.sourceNode.selected)
                    {
                        this.SelectOnlyOneNode(this.sourceNode);
                        this.diagram.InvalidateDiagram();
                    }

                    if (this.keyctrl && !this.keyshift && !this.keyalt
                        && !this.sourceNode.selected
                        && this.selectedNodes.Count == 0)
                    {
                        this.SelectOnlyOneNode(this.sourceNode);
                        this.diagram.InvalidateDiagram();
                    }

                    if (this.sourceNode.selected)
                    {
                        if (this.keyctrl && !this.keyshift && !this.keyalt)  // start copy item
                        {
                            this.stateCoping = true;

                            this.copySelectedNodes.Copy(this.selectedNodes);
                            foreach (Node node in this.copySelectedNodes)
                            {
                                if (node.id == this.sourceNode.id)
                                {
                                    this.copySourceNode = node;
                                    break;
                                }
                            }
                            this.copySelectedLines.Copy(this.GetSelectedLines());

                            foreach (Line line in this.copySelectedLines)
                            {
                                foreach (Node node in this.copySelectedNodes)
                                {
                                    if (line.end == node.id)
                                    {
                                        line.endNode = node;
                                    }

                                    if (line.start == node.id)
                                    {
                                        line.startNode = node;
                                    }
                                }
                            }
                        }

                        this.stateDragSelection = true;
                        MoveTimer.Enabled = true;
                        this.startNodePos.Set(this.sourceNode.position); // starting position of draging item

                        this.vmouse
                            .Set(this.actualMousePos)
                            .Scale(Tools.GetScale(this.scale))
                            .Subtract(this.shift)
                            .Subtract(this.sourceNode.position); // mouse position in node
                    }
                }
            }

            if (buttonright)
            {
                this.stateMoveView = true; // popupmenu or view move
            }

            if (buttonmiddle && !isreadonly)
            {
                this.sourceNode = this.FindNodeInMousePosition(new Position(e.X, e.Y));
                this.stateAddingNode = true;// add node by drag
                MoveTimer.Enabled = true;
            }

            this.diagram.InvalidateDiagram();
        }

        // EVENT Mouse move UID3677213415                                                              // [MOUSE] [MOVE] [EVENT]
        public void DiagramApp_MouseMove(object sender, MouseEventArgs e)
        {

#if DEBUG
            this.LogEvent("MouseMove");
#endif

            if (this.stateSelectingNodes || this.stateAddingNode)
            {
                this.actualMousePos.Set(e.X, e.Y);
                this.diagram.InvalidateDiagram();
            }
            else
            if (this.stateDragSelection || this.stateAddingNode || this.stateSelectingNodes) // object move
            {
                this.actualMousePos.Set(e.X, e.Y);
            }
            else
            if (this.stateMoveView) // screen moving
            {
                this.shift.x = this.startShift.x + (e.X - this.startMousePos.x) * Tools.GetScale(this.scale);
                this.shift.y = this.startShift.y + (e.Y - this.startMousePos.y) * Tools.GetScale(this.scale);
                this.diagram.InvalidateDiagram();
            }
            else
            if (bottomScrollBar != null && rightScrollBar != null && (bottomScrollBar.MouseMove(e.X, e.Y) || rightScrollBar.MouseMove(e.X, e.Y))) //SCROLLBARS
            {
                bottomScrollBar.SetPosition(GetPositionHorizontal());
                rightScrollBar.SetPosition(GetPositionVertical());
                this.diagram.InvalidateDiagram();
            }
        }

        // EVENT Mouse Up UID3026627853                                                                // [MOUSE] [UP] [EVENT]
        public void DiagramApp_MouseUp(object sender, MouseEventArgs e)
        {

#if DEBUG
            this.LogEvent("MouseUp");
#endif

            this.actualMousePos.Set(e.X, e.Y);
            Position mouseTranslation = new Position(this.actualMousePos).Subtract(this.startMousePos);

            // States
            bool mousemove = ((Math.Abs(this.actualMousePos.x - this.startMousePos.x) > 2) || (Math.Abs(this.actualMousePos.y - this.startMousePos.y) > 2)); // mouse change position
            bool buttonleft = e.Button == MouseButtons.Left;
            bool buttonright = e.Button == MouseButtons.Right;
            bool buttonmiddle = e.Button == MouseButtons.Middle;
            bool isreadonly = this.diagram.IsReadOnly();
            bool keyalt = this.keyalt;
            bool keyctrl = this.keyctrl;
            bool keyshift = this.keyshift;
            bool dblclick = this.stateDblclick;
            bool finishdraging = this.stateDragSelection;
            bool finishadding = this.stateAddingNode;
            bool finishselecting = mousemove && this.stateSelectingNodes;

            MoveTimer.Enabled = false;

            if (this.diagram.IsLocked())
            {
                this.diagram.UnlockDiagram();
                this.ResetStates();
                return;
            }

            // KEY DRAG
            if (finishdraging && !isreadonly) // drag node
            {

                if (this.sourceNode != null && !keyctrl) // return node to starting position after connection is created
                {
                    Position translation = new Position(this.startNodePos)
                        .Subtract(sourceNode.position);

                    if (this.selectedNodes.Count > 0)
                    {
                        foreach (Node node in this.selectedNodes)
                        {
                            node.position.Add(translation);
                        }
                    }

                    this.diagram.InvalidateDiagram();
                }
            }

            if (dblclick)
            {
                this.stateSelectingNodes = false;
            }

            // KEY DRAG-MMIDDLE
            if (finishadding)
            {
                this.diagram.InvalidateDiagram();
            }

            // KEY DRAG+MLEFT select nodes with selection rectangle UID0351799057
            if (finishselecting && mousemove)
            {
                Position a = this.startMousePosInDiagram.Clone();
                Position b = this.MouseToDiagramPosition(this.actualMousePos);

                // swap inverse mouse position
                decimal temp;
                if (b.x < a.x) { temp = a.x; a.x = b.x; b.x = temp; }
                if (b.y < a.y) { temp = b.y; b.y = a.y; a.y = temp; }

                if (!this.keyshift) this.ClearSelection();

                foreach (Node rec in this.currentLayer.nodes)
                {

                    if (rec.layer != this.currentLayer.id && rec.id != this.currentLayer.id)
                    {
                        continue;
                    }

                    decimal nodeScale = Tools.GetScale(rec.scale);


                    bool isNodeInSelection = false;


                    // check if node is in selection
                    if (rec.transparent && rec.name == "") // check if empty transparent node is in selection
                    {
                        decimal sx = rec.position.x + (rec.width / nodeScale) / 2;
                        decimal sy = rec.position.y + (rec.height / nodeScale) / 2;

                        decimal viewScale = Tools.GetScale(this.scale);

                        if (a.x <= sx - (rec.width / 2 * viewScale) / 2
                            && sx + (rec.width / 2 * viewScale) <= b.x
                            && a.y <= sy - (rec.height / 2 * viewScale) / 2
                            && sy + (rec.height / 2 * viewScale) <= +b.y)
                        {
                            isNodeInSelection = true;
                        }

                    }
                    else if ( // check if other node is in selection
                        a.x <= rec.position.x
                        && rec.position.x + rec.width * nodeScale <= b.x
                        && a.y <= rec.position.y
                        && rec.position.y + rec.height * nodeScale <= b.y)
                    {
                        isNodeInSelection = true;
                    }


                    if (isNodeInSelection)
                    {
                        if (keyshift && !keyctrl && !keyalt) // KEY SHIFT+MLEFT Invert selection
                        {
                            if (rec.selected)
                            {
                                this.RemoveNodeFromSelection(rec);
                            }
                            else
                            {
                                this.SelectNode(rec);
                            }
                        }

                        if (!keyshift && !keyctrl && !keyalt) // KEY MLEFT select nodes
                        {
                            this.SelectNode(rec);
                        }
                    }

                }

                this.diagram.InvalidateDiagram();
            }

            Node TargetNode = this.FindNodeInMousePosition(new Position(e.X, e.Y));

            if (buttonleft && this.stateCoping && !isreadonly) // CTRL+DRAG copy part of diagram
            {
                this.stateCoping = false;

                if (mousemove)
                {

                    DiagramBlock newBlock = this.diagram.DuplicatePartOfDiagram(this.selectedNodes, this.currentLayer.id);

                    Position vector = new Position(this.actualMousePos)
                        .Scale(Tools.GetScale(this.scale))
                        .Subtract(this.vmouse)
                        .Subtract(this.shift)
                        .Subtract(this.sourceNode.position);

                    // filter only top nodes fromm all new created nodes. NewNodes containing sublayer nodes.
                    Nodes topNodes = new Nodes();

                    foreach (Node node in newBlock.nodes)
                    {
                        if (node.layer == this.currentLayer.id)
                        {
                            topNodes.Add(node);
                        }
                    }

                    foreach (Node node in topNodes)
                    {
                        node.position.Add(vector);
                    }

                    this.diagram.Unsave("create", newBlock.nodes, newBlock.lines, this.shift, this.scale, this.currentLayer.id);

                    this.SelectNodes(topNodes);

                    this.diagram.Unsave();
                }

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            if (buttonleft
                && !keyalt
                && keyctrl
                && !keyshift
                && TargetNode != null
                && !TargetNode.selected) // CTRL+CLICK add node to selection
            {
                this.SelectNode(TargetNode);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            if (buttonleft
                && bottomScrollBar != null
                && rightScrollBar != null
                && (bottomScrollBar.MouseUp() || rightScrollBar.MouseUp()))
            {
                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY MLEFT clear selection
            if (buttonleft
                && !mousemove
                && TargetNode == null
                && this.sourceNode == null
                && this.selectedNodes.Count() > 0
                && !keyalt
                && !keyctrl
                && !keyshift)
            {
                this.ClearSelection();

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY CTRL+ALT+DRAG create node and conect with existing node
            if (buttonleft
                && !isreadonly
                && !keyshift
                && keyctrl
                && keyalt
                && TargetNode == null
                && this.sourceNode != null)
            {
                var s = this.sourceNode;
                var newNodePosition = new Position(e.X, e.Y);
                var node = this.CreateNode(newNodePosition);
                node.shortcut = s.id;
                this.diagram.Connect(s, node);
                this.diagram.Unsave("create", node, this.shift, this.scale, this.currentLayer.id);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY CTRL+ALT+DRAG create shortcut beetwen objects
            if (buttonleft
                && !isreadonly
                && !keyshift
                && keyctrl
                && keyalt
                && TargetNode != null
                && this.sourceNode != null
                && TargetNode != this.sourceNode)
            {
                this.diagram.Unsave("edit", this.sourceNode, this.shift, this.scale, this.currentLayer.id);
                this.sourceNode.link = "#" + TargetNode.id.ToString();
                this.diagram.Unsave();

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY DRAG move node
            if
            (buttonleft
                && !isreadonly
                && finishdraging
                && !keyshift
                && !keyctrl
                && !keyalt
                && (
                    (TargetNode == null && this.sourceNode != null)
                    || (
                        TargetNode != null
                        && this.sourceNode != TargetNode
                        && TargetNode.selected
                    )
                    || (TargetNode != null && this.sourceNode == TargetNode)
                )
                && Math.Sqrt((double)(mouseTranslation.x * mouseTranslation.x + mouseTranslation.y * mouseTranslation.y)) > 5
            )
            {
                Position vector = new Position(this.actualMousePos)
                    .Scale(Tools.GetScale(this.scale))
                    .Subtract(this.vmouse)
                    .Subtract(this.shift)
                    .Subtract(this.sourceNode.position);

                if (this.selectedNodes.Count > 0)
                {
                    this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);

                    foreach (Node node in this.selectedNodes)
                    {
                        node.position.Add(vector);
                    }

                    this.diagram.Unsave();
                }

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY DRAG+ALT create node and conect with existing node
            if (buttonleft
                && !isreadonly
                && !keyshift
                && !keyctrl
                && keyalt
                && TargetNode != null
                && this.sourceNode == null)
            {
                Node node = this.CreateNode(
                        new Position(
                            +this.shift.x - startShift.x + this.startMousePos.x,
                            +this.shift.y - startShift.y + this.startMousePos.y
                        )
                    );

                Line line = this.diagram.Connect(
                    node,
                    TargetNode
                );

                this.diagram.Unsave("create", node, line, this.shift, this.scale, this.currentLayer.id);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY CTRL+ALT+DRAG create node and make shortcut to target node
            if (buttonleft
                && !isreadonly
                && keyalt
                && keyctrl
                && !keyshift
                && TargetNode != null
                && this.sourceNode == null)
            {
                Position newNodePosition = new Position(this.shift)
                    .Subtract(startShift)
                    .Add(this.startMousePos);

                Node newrec = this.CreateNode(
                    newNodePosition
                );

                newrec.link = "#" + TargetNode.id;
                this.diagram.Unsave("create", newrec, this.shift, this.scale, this.currentLayer.id);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY DBLCLICK open link or edit window after double click on node [dblclick] [open] [edit] //UID8515606919
            if (buttonleft
                && dblclick
                && this.sourceNode != null
                && !keyctrl
                && !keyalt
                && !keyshift)
            {
                this.ResetStates();
                this.OpenLink(this.sourceNode);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY SHIFT+DBLCLICK open node edit form
            if (buttonleft
                && dblclick
                && this.sourceNode != null
                && !keyctrl
                && !keyalt
                && keyshift)
            {
                this.diagram.EditNode(this.sourceNode);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY CTRL+DBLCLICK open link in node
            if (buttonleft
                && dblclick
                && this.sourceNode != null
                && keyctrl
                && !keyalt
                && !keyshift)
            {
                if (this.sourceNode.link != "")
                {
                    Os.OpenPathInSystem(this.sourceNode.link);
                }

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY DBLCLICK+SPACE change position in zoom view mode
            if (buttonleft
                && this.stateZooming
                && dblclick
                && !keyctrl
                && !keyalt
                && !keyshift)
            {
                this.shift
                    .Subtract(
                        this.actualMousePos
                        .Clone()
                        .Scale(Tools.GetScale(this.scale))
                        )
                    .Add(
                        (this.ClientSize.Width * Tools.GetScale(this.scale)) / 2,
                        (this.ClientSize.Height * Tools.GetScale(this.scale)) / 2
                    );

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY CTRL+SHIFT+MLEFT conect with selected nodes new node or selected node
            if (buttonleft
                && !isreadonly
                && keyshift
                && keyctrl
                && this.selectedNodes.Count() > 0
                && e.X == this.startMousePos.x
                && e.Y == this.startMousePos.y)
            {
                // TODO Still working this?
                Node newrec = TargetNode;

                Nodes newNodes = new Nodes();
                if (newrec == null)
                {
                    newrec = this.CreateNode(this.actualMousePos.Clone().Subtract(10), false);
                    newNodes.Add(newrec);
                }

                Lines newLines = new Lines();
                foreach (Node rec in this.selectedNodes)
                {
                    Line line = this.diagram.Connect(rec, newrec);
                    newLines.Add(line);
                }

                this.SelectOnlyOneNode(newrec);
                this.diagram.Unsave("create", newNodes, newLines, this.shift, this.scale, this.currentLayer.id);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY DBLCLICK create new node UID6734640900
            if (buttonleft
                && !isreadonly
                && dblclick
                && !keyalt
                && !keyshift
                && !keyctrl
                && TargetNode == null
                && this.sourceNode == null
                && e.X == this.startMousePos.x
                && e.Y == this.startMousePos.y)
            {
                Node newNode = this.CreateNode(this.actualMousePos.Clone().Subtract(10), false);
                this.diagram.Unsave("create", newNode, this.shift, this.scale, this.currentLayer.id);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // ALT+MLEFT Node click -> center node
            if (buttonleft
                && !isreadonly
                && !dblclick
                && keyalt
                && !keyshift
                && !keyctrl
                && TargetNode != null
                && this.sourceNode == TargetNode
                && e.X == this.startMousePos.x
                && e.Y == this.startMousePos.y)
            {
                this.GoToNode(TargetNode);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY DRAG+ALT copy style from node to other node
            if (buttonleft
                && !isreadonly
                && !keyshift
                && !keyctrl
                && keyalt
                && TargetNode != null
                && this.sourceNode != null
                && this.sourceNode != TargetNode)
            {
                if (this.selectedNodes.Count() > 1)
                {
                    this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                    foreach (Node rec in this.selectedNodes)
                    {
                        rec.CopyNodeStyle(TargetNode);
                    }
                    this.diagram.Unsave();
                }

                if (this.selectedNodes.Count() == 1
                    || (this.selectedNodes.Count() == 0 && this.sourceNode != null))
                {
                    this.diagram.undoOperations.Add("edit", TargetNode, this.shift, this.scale, this.currentLayer.id);

                    TargetNode.CopyNodeStyle(this.sourceNode);

                    if (this.selectedNodes.Count() == 1 && this.selectedNodes[0] != this.sourceNode)
                    {
                        this.ClearSelection();
                        this.SelectNode(this.sourceNode);
                    }
                    this.diagram.Unsave();
                }

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY DRAG make link between two nodes
            if (buttonleft
                && !isreadonly
                && !keyctrl
                && !keyshift
                && !keyalt
                && TargetNode != null
                && this.sourceNode != null
                && this.sourceNode != TargetNode)
            {
                bool arrow = false;
                if (keyshift)
                {
                    arrow = true;
                }

                Lines newLines = new Lines();
                Lines removeLines = new Lines();
                if (this.selectedNodes.Count() > 0)
                {
                    foreach (Node rec in this.selectedNodes)
                    {
                        if (rec != TargetNode)
                        {
                            if (this.diagram.HasConnection(rec, TargetNode))
                            {
                                Line removeLine = this.diagram.GetLine(rec, TargetNode);
                                removeLines.Add(removeLine);
                                this.diagram.Disconnect(rec, TargetNode);

                            }
                            else
                            {
                                Line newLine = this.diagram.Connect(rec, TargetNode, arrow, null);
                                newLines.Add(newLine);
                            }
                        }
                    }
                }

                if (newLines.Count() > 0 && removeLines.Count() > 0)
                {
                    this.diagram.undoOperations.StartGroup();
                    this.diagram.undoOperations.Add("create", null, newLines, this.shift, this.scale, this.currentLayer.id);
                    this.diagram.undoOperations.Add("delete", null, removeLines, this.shift, this.scale, this.currentLayer.id);
                    this.diagram.undoOperations.EndGroup();
                    this.diagram.Unsave();
                }
                else if (newLines.Count() > 0)
                {
                    this.diagram.undoOperations.Add("create", null, newLines, this.shift, this.scale, this.currentLayer.id);
                }
                else if (removeLines.Count() > 0)
                {
                    this.diagram.undoOperations.Add("delete", null, removeLines, this.shift, this.scale, this.currentLayer.id);
                }


                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY SHIFT+MLEFT add node to selected nodes
            if (buttonleft
                && !keyctrl
                && keyshift
                && !keyalt
                && this.sourceNode == TargetNode
                && TargetNode != null
                && !TargetNode.selected)
            {
                this.SelectNode(TargetNode);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY SHIFT+MLEFT remove node from selected nodes
            if (buttonleft
                && !keyctrl
                && keyshift
                && !keyalt
                && TargetNode != null
                && (this.sourceNode == TargetNode || TargetNode.selected))
            {
                this.RemoveNodeFromSelection(TargetNode);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            if (buttonleft
                && !isreadonly
                && !keyctrl
                && !keyshift
                && !keyalt
                && this.sourceNode == TargetNode
                && this.stateSourceNodeAlreadySelected)
            {
                this.Rename(); //UID3101342400

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            if (buttonright
                && !keyctrl
                && !keyshift
                && !keyalt) // KEY MRIGHT
            {
                this.stateMoveView = false; // show popup menu

                if (e.X == this.startMousePos.x
                    && e.Y == this.startMousePos.y
                    && this.startShift.x == this.shift.x
                    && this.startShift.y == this.shift.y)
                {
                    Node temp = this.FindNodeInMousePosition(new Position(e.X, e.Y));


                    if (temp == null)
                    {
                        this.ClearSelection();
                    }

                    if (temp != null && !temp.selected)
                    {
                        this.ClearSelection();
                        this.SelectOnlyOneNode(temp);
                    }

                    PopupMenu.Show(this.Left + e.X, this.Top + e.Y); // [POPUP] show popup
                }
                else
                { // KEY DRAG+MRIGHT move view
                    this.shift.x = this.startShift.x + (e.X - this.startMousePos.x) * Tools.GetScale(this.scale);
                    this.shift.y = this.startShift.y + (e.Y - this.startMousePos.y) * Tools.GetScale(this.scale);
                }

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }


            // KEY DRAG+MMIDDLE conect two existing nodes
            if (buttonmiddle
                && !isreadonly
                && this.sourceNode != null
                && TargetNode != null
            )
            {

                Line newLine = this.diagram.Connect(
                    sourceNode,
                    TargetNode
                );

                if (sourceNode.name == "")
                {
                    sourceNode.transparent = true;
                }
                if (TargetNode.name == "")
                {
                    TargetNode.transparent = true;
                }

                if (newLine != null)
                {
                    this.diagram.Unsave("create", newLine, this.shift, this.scale, this.currentLayer.id);
                    this.diagram.InvalidateDiagram();
                }

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY DRAG+MMIDDLE connect exixting node with new node
            if (buttonmiddle
                && !isreadonly
                && this.sourceNode != null
                && TargetNode == null
                )
            {

                Node newNode = this.CreateNode(this.actualMousePos.Clone().Subtract(10));

                Line newLine = this.diagram.Connect(
                    sourceNode,
                    newNode
                );

                if (sourceNode.name == "")
                {
                    sourceNode.transparent = true;
                }
                newNode.transparent = true;

                this.diagram.Unsave("create", newNode, newLine, this.shift, this.scale, this.currentLayer.id);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY DRAG+MMIDDLE create new node and conect id with existing node
            if (buttonmiddle
                && !isreadonly
                && this.sourceNode == null
                && TargetNode != null)
            {
                Node newNode = this.CreateNode(
                    (new Position(this.shift)).Subtract(this.startShift).Add(this.startMousePos)
                );

                Line newLine = this.diagram.Connect(
                    newNode,
                    TargetNode
                );

                newNode.transparent = true;

                this.diagram.Unsave("create", newNode, newLine, this.shift, this.scale, this.currentLayer.id);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            // KEY DRAG+MMIDDLE create new node and conect with new node (create line)
            if (buttonmiddle
                && !isreadonly
                && this.sourceNode == null
                && TargetNode == null)
            {
                Nodes nodes = new Nodes();
                Lines lines = new Lines();

                Node node1 = this.CreateNode(
                        (new Position(this.shift)).Subtract(this.startShift).Add(this.startMousePos),
                        false
                    );

                nodes.Add(node1);

                Node node2 = this.CreateNode(
                    this.actualMousePos.Clone().Subtract(10)
                );

                nodes.Add(node2);

                lines.Add(this.diagram.Connect(
                    node1,
                    node2
                ));

                node1.transparent = true;
                node2.transparent = true;

                this.diagram.Unsave("create", nodes, lines, this.shift, this.scale, this.currentLayer.id);

                this.diagram.InvalidateDiagram();
                this.ResetStates();
                return;
            }

            this.ResetStates();
        }

        // EVENT Mouse Whell UID1111344210
        public void DiagramApp_MouseWheel(object sender, MouseEventArgs e)                             // [MOUSE] [WHELL] [EVENT]
        {
#if DEBUG
            this.LogEvent("MouseWheel");
#endif
            decimal newScale = 0;
            //throw new NotImplementedException();
            if (e.Delta > 0) // MWHELL
            {
                if (this.keyctrl) // zoom in
                {
                    Position m = this.GetMousePosition();
                    Position r = m.Clone().Scale(Tools.GetScale(this.scale)).Subtract(this.shift);

                    newScale = this.scale + 0.5m;

                    if (newScale > 80)
                    {
                        newScale = 80;
                    }

                    Position sh = m.Scale(Tools.GetScale(newScale)).Subtract(r);

                    this.scale = newScale;
                    this.shift.x = sh.x;
                    this.shift.y = sh.y;

                    zoomTmerTime = 100;
                    this.zoomTimer.Enabled = true;
                    this.diagram.InvalidateDiagram();
                }
                else
                if (this.keyshift) // move view
                {
                    this.shift.x += 50 * Tools.GetScale(this.scale);
                    this.diagram.InvalidateDiagram();
                }
                else // move view
                {
                    this.shift.y += 50 * Tools.GetScale(this.scale);
                    this.diagram.InvalidateDiagram();
                }
            }
            else
            {
                if (this.keyctrl) // zoom out
                {
                    Position m = this.GetMousePosition();
                    Position r = m.Clone().Scale(Tools.GetScale(this.scale)).Subtract(this.shift);

                    newScale = this.scale - 0.5m;

                    if (newScale < -80)
                    {
                        newScale = -80;
                    }

                    Position sh = m.Scale(Tools.GetScale(newScale)).Subtract(r);

                    this.scale = newScale;
                    this.shift.x = sh.x;
                    this.shift.y = sh.y;

                    zoomTmerTime = 100;
                    this.zoomTimer.Enabled = true;
                    this.diagram.InvalidateDiagram();
                }
                else
                if (this.keyshift) // move view
                {
                    this.shift.x -= 50 * Tools.GetScale(this.scale);
                    this.diagram.InvalidateDiagram();
                }
                else // move view
                {
                    this.shift.y -= 50 * Tools.GetScale(this.scale);
                    this.diagram.InvalidateDiagram();
                }
            }

            if (newScale != 0 && newScale != Tools.GetScale(this.scale))
            {
                /*this.scale = newScale;
                this.zoomTimerStep = Math.Abs((newScale - Tools.GetScale(this.scale)) / 30);
                if (this.zoomTimerStep <= 0) {
                    this.zoomTimerStep = 0.001f;
                }

                this.zoomTimerScale = newScale;
                this.zoomTimer.Enabled = true;
                this.diagram.InvalidateDiagram();*/
            }
        }

        // EVENT revert states to default UID5045070650
        private void ResetStates()
        {
            this.MoveTimer.Enabled = false;
            this.stateDragSelection = false;
            this.stateMoveView = false;
            this.stateSelectingNodes = false;
            this.stateAddingNode = false;
            this.stateDblclick = false;
            //this.stateZooming = false;
            this.stateSearching = false;
            this.stateSourceNodeAlreadySelected = false;
            this.stateCoping = false;
        }

        // EVENT Shortcuts UID1444131132
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)                           // [KEYBOARD] [EVENT]
        {

#if DEBUG
            this.LogEvent("ProcessCmdKey");
#endif

            if (this.IsEditing() || this.stateSearching)
            {
                return false;
            }

            bool isreadonly = this.diagram.IsReadOnly();
            bool stopNextAction = this.main.plugins.KeyPressAction(this.diagram, this, keyData); //UID0290845814

            /*
             * order : ProcessCmdKey, DiagramApp_KeyDown, DiagramApp_KeyPress, DiagramApp_KeyUp;
             */

            if (KeyMap.ParseKey(KeyMap.selectAllElements, keyData)) // [KEY] [CTRL+A] select all
            {
                this.SelectAll();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.alignToLine, keyData)) // [KEY] [CTRL+L] align to line
            {
                this.AlignToLine();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.alignToColumn, keyData)) // [KEY] [CTRL+H] align to column
            {
                this.AlignToColumn();
                return true;
            }


            if (!isreadonly && KeyMap.ParseKey(KeyMap.alignToGroup, keyData)) // [KEY] [CTRL+K] align to group
            {
                this.AlignToGroup();

                return true;
            }

            if (KeyMap.ParseKey(KeyMap.markNodes, keyData)) // [KEY] [CTRL+M] mark node for navigation history
            {
                this.SwitchMarkForSelectedNodes();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.prevMarkNode, keyData)) // [KEY] [ALT+LEFT] find prev marked node
            {
                this.PrevMarkedNode();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.nextMarkNode, keyData)) // [KEY] [ALT+RIGHT] find next marked node
            {
                this.NextMarkedNode();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.alignToLineGroup, keyData)) // [KEY] [CTRL+SHIFT+K] align to group
            {
                this.AlignToLineGroup();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.copy, keyData))  // [KEY] [CTRL+C]
            {
                this.Copy();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.copyLinks, keyData))  // [KEY] [CTRL+SHIFT+C] copy links from selected nodes
            {
                this.CopyLink();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.copyNotes, keyData))  // [KEY] [CTRL+ALT+SHIFT+C] copy notes from selected nodes
            {
                this.CopyNote();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.cut, keyData))  // [KEY] [CTRL+X] Cut diagram
            {
                this.Cut();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.paste, keyData))  // [KEY] [CTRL+V] [PASTE] Paste text from clipboard
            {
                Point ptCursor = Cursor.Position;
                ptCursor = PointToClient(ptCursor);
                this.Paste(new Position(ptCursor.X, ptCursor.Y));
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.pasteToNote, keyData))  // [KEY] [CTRL+SHIFT+V] paste to note
            {
                this.PasteToNote();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.pasteToLink, keyData))  // [KEY] [CTRL+SHIFT+INS] paste to node link
            {
                this.PasteToLink();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.undo, keyData))  // [KEY] [CTRL+Z]
            {
                this.diagram.DoUndo(this);
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.redo, keyData))  // [KEY] [CTRL+SHIFT+Z]
            {
                this.diagram.DoRedo(this);
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.newDiagram, keyData))  // [KEY] [CTRL+N] New Diagram
            {
                main.OpenDiagram();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.newDiagramView, keyData))  // [KEY] [F7] New Diagram view
            {
                this.diagram.OpenDiagramView(this);
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.switchViews, keyData))  // [KEY] [F8] hide views
            {
                this.main.SwitchViews(this);
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.save, keyData))  // [KEY] [CTRL+S] save diagram UID4672553712
            {
                this.Save();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.open, keyData))  // [KEY] [CTRL+O] open diagram dialog window UID7674842403
            {
                this.OpenFileDialog();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.search, keyData))  // [KEY] [CTRL+F] Search form UID0886546362
            {
                this.ShowSearchPanel();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.date, keyData))  // [KEY] [CTRL+D] date
            {
                this.EvaluateDate();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.promote, keyData)) // [KEY] [CTRL+SHIFT+P] Promote node
            {
                this.Promote();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.refresh, keyData)) // [KEY] [CTRL+R] Refresh
            {
                if (this.selectedNodes.Count > 0)
                {
                    this.diagram.RefreshNodes(this.selectedNodes);
                }
                else
                {
                    this.diagram.RefreshAll();
                }

                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.hideBackground, keyData)) // [KEY] [F6] Hide background
            {
                this.HideBackground();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.searchNext, keyData)) // [KEY] [F3] reverse search
            {
                if (this.searhPanel != null)
                {
                    this.searhPanel.SearchNext();
                }
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.reverseSearch, keyData)) // [KEY] [SHIFT+F3] reverse search
            {
                this.SearchPrev();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.home, keyData)) // KEY [HOME] go to home position
            {
                this.GoToHome();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.openViewHome, keyData)) // KEY [CTRL+HOME] open view and go to home position
            {
                this.OpenViewAndGoToHome();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.setHome, keyData))  // [KEY] [SHIFT+HOME] Move start point
            {
                this.SetCurentPositionAsHomePosition();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.end, keyData)) // KEY [END] go to end position
            {
                this.GoToEnd();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.setEnd, keyData))  // [KEY] [SHIFT+END] Move end point
            {
                this.SetCurentPositionAsEndPosition();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.openViewEnd, keyData)) // KEY [CTRL+END] open view and go to home position
            {
                this.OpenViewAndGoToEnd();
                return true;
            }

            /*
             [DOCUMENTATION]
             Shortcut F5
            -otvorenie adresara vybranej nody
            -prejdu sa vybrane nody a ak je to adresar alebo subor otvori sa adresar
            -ak nie su vybrane ziadne nody otvori sa adresar diagrammu
            */
            if (KeyMap.ParseKey(KeyMap.openDrectory, keyData)) // KEY [F5] Open link directory or diagram directory
            {
                OpenLinkDirectory();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.console, keyData)) // [KEY] [F12] show Debug console
            {
                this.main.ShowConsole();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.moveNodeUp, keyData)) // KEY [CTRL+PAGEUP] move node up to foreground
            {
                this.MoveNodesToForeground();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.moveNodeDown, keyData)) // [KEY] [CTRL+PAGEDOWN] move node down to background
            {
                this.MoveNodesToBackground();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.pageUp, keyData)) // [KEY] [PAGEUP] change current position in diagram view
            {
                this.PageUp();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.pageDown, keyData)) // [KEY] [PAGEDOWN] change current position in diagram view
            {
                this.PageDown();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.editNodeName, keyData)) // [KEY] [F2] edit node name
            {
                this.Rename();
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.editNodeLink, keyData)) // [KEY] [F4] edit node link
            {
                this.EditLink();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.fullScreean, keyData)) // [KEY] [F11] show full screen
            {
                this.FullScreenSwitch();
                return true;
            }


            if (KeyMap.ParseKey(KeyMap.openEditForm, keyData)) // [KEY] [CTRL+E] open edit form
            {
                this.Edit();
                return true;
            }

            // prevent catch keys while node is creating or renaming
            if (this.IsEditing())
            {
                return false;
            }

            if (KeyMap.ParseKey(KeyMap.editOrLayerIn, keyData)) // [KEY] [ENTER] open edit form or layer in UID6919250456
            {
                this.LayerInOrEdit();
                return true;
            }


            if (KeyMap.ParseKey(KeyMap.layerIn, keyData)) // [KEY] [PLUS] Layer in
            {
                this.LayerIn();
                return true;
            }

            // [KEY] [BACK] or [MINUS] Layer out UID1557077053
            if (KeyMap.ParseKey(KeyMap.layerOut, keyData) || KeyMap.ParseKey(KeyMap.layerOut2, keyData))
            {
                this.LayerOut();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.minimalize, keyData)) // [KEY] [ESC] minimalize diagram view
            {
                if (this.animationTimer.Enabled)
                {
                    this.animationTimer.Enabled = false; // stop move animation if exist
                }
                else
                if (this.isFullScreen)
                {
                    this.FullScreenSwitch();
                }
                else
                {
                    return this.FormHide();
                }
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.delete, keyData)) // [KEY] [DEL] [DELETE] delete
            {
                this.DeleteSelectedNodes(this);
                return true;
            }

            if (!isreadonly && (KeyMap.ParseKey(KeyMap.moveLeft, keyData) || KeyMap.ParseKey(KeyMap.moveLeftFast, keyData)))  // [KEY] [left] [SHIFT+LEFT] [ARROW] Move node
            {

                this.MoveNodesToLeft(KeyMap.ParseKey(KeyMap.moveLeftFast, keyData));
                return true;
            }

            if (!isreadonly && (KeyMap.ParseKey(KeyMap.moveRight, keyData) || KeyMap.ParseKey(KeyMap.moveRightFast, keyData)))  // [KEY] [right] [SHIFT+RIGHT] [ARROW] Move node
            {
                this.MoveNodesToRight(KeyMap.ParseKey(KeyMap.moveRightFast, keyData));
                return true;
            }

            if (!isreadonly && (KeyMap.ParseKey(KeyMap.moveUp, keyData) || KeyMap.ParseKey(KeyMap.moveUpFast, keyData)))  // [KEY] [up] [SHIFT+UP] [ARROW] Move node
            {
                this.MoveNodesUp(KeyMap.ParseKey(KeyMap.moveUpFast, keyData));
                return true;
            }

            if (!isreadonly && (KeyMap.ParseKey(KeyMap.moveDown, keyData) || KeyMap.ParseKey(KeyMap.moveDownFast, keyData)))  // [KEY] [down] [SHIFT+DOWN] [ARROW] Move node
            {
                this.MoveNodesDown(KeyMap.ParseKey(KeyMap.moveDownFast, keyData));
                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.alignLeft, keyData)) // [KEY] [TAB] align selected nodes to left
            {

                if (this.selectedNodes.Count() > 1)
                {
                    this.AlignLeft();
                }
                else
                {
                    this.AddNodeAfterNode();
                }

                return true;
            }

            if (!isreadonly && KeyMap.ParseKey(KeyMap.alignRight, keyData))  // [KEY] [SHIFT+TAB] align selected nodes to right
            {
                this.AlignRight();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.resetZoom, keyData))  // [KEY] [CTRL+0] reset zoom level to default
            {
                this.ResetZoom();
                return true;
            }

            if (KeyMap.ParseKey(KeyMap.switchSecurityLock, keyData)) // [KEY] [CTRL+ALT+L] lock encrypted diagram UID6442152339
            {
                if (this.diagram.IsEncrypted())
                {
                    this.main.LockDiagrams();
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // EVENT Key down UID3114212444
        public void DiagramApp_KeyDown(object sender, KeyEventArgs e)                                  // [KEYBOARD] [DOWN] [EVENT]
        {

#if DEBUG
            this.LogEvent("KeyDown");
#endif

            if (this.IsEditing() || this.stateSearching)
            {
                return;
            }

            if (e.Shift)
            {
                this.keyshift = true;
            }

            if (e.Control)
            {
                this.keyctrl = true;
            }

            if (e.Alt)
            {
                this.keyalt = true;
            }

            if (this.IsEditing())
            {
                return;
            }

            if (e.KeyCode == Keys.Space && !this.stateZooming) // KEY [SPACE] [SPACEBAR] [zoom] zoom preview
            {
                this.stateSelectingNodes = false;
                this.MoveTimer.Enabled = false;
                this.zoomTimer.Enabled = false;

                this.stateZooming = true;
                Position tmp = new Position(this.shift);

                tmp.Add(
                    -(this.ClientSize.Width / 2 - this.ClientSize.Width / 2 / Tools.GetScale(this.scale)) * Tools.GetScale(this.scale),
                    -(this.ClientSize.Height / 2 - this.ClientSize.Height / 2 / Tools.GetScale(this.scale)) * Tools.GetScale(this.scale)
                );

                this.currentScale = this.scale;
                this.scale += 2;

                tmp.Add(
                    (this.ClientSize.Width / 2 - this.ClientSize.Width / 2 / Tools.GetScale(this.scale)) * Tools.GetScale(this.scale),
                    (this.ClientSize.Height / 2 - this.ClientSize.Height / 2 / Tools.GetScale(this.scale)) * Tools.GetScale(this.scale)
                );

                this.shift.Set(tmp);

                this.diagram.InvalidateDiagram();
            }

        }

        // EVENT Key up UID4343244331
        public void DiagramApp_KeyUp(object sender, KeyEventArgs e)
        {

#if DEBUG
            this.LogEvent("KeyUp");
#endif

            this.keyshift = false;
            this.keyctrl = false;
            this.keyalt = false;

            if (this.IsEditing() || this.stateSearching)
            {
                return;
            }

            if (this.stateZooming)
            {
                MoveTimer.Enabled = false;  // zrusenie prebiehajucich operácii
                this.stateMoveView = false;
                this.stateAddingNode = false;
                this.stateDragSelection = false;
                this.stateSelectingNodes = false;

                this.stateZooming = false; // KEY SPACE cancel space zoom and restore prev zoom

                shift.Add(
                    -(this.ClientSize.Width / 2 - this.ClientSize.Width / 2 / Tools.GetScale(this.scale)) * Tools.GetScale(this.scale),
                    -(this.ClientSize.Height / 2 - this.ClientSize.Height / 2 / Tools.GetScale(this.scale)) * Tools.GetScale(this.scale)
                );

                this.scale = this.currentScale;

                shift.Add(
                    (this.ClientSize.Width / 2 - this.ClientSize.Width / 2 / Tools.GetScale(this.scale)) * Tools.GetScale(this.scale),
                    (this.ClientSize.Height / 2 - this.ClientSize.Height / 2 / Tools.GetScale(this.scale)) * Tools.GetScale(this.scale)
                );

                this.diagram.InvalidateDiagram();
            }
        }                                 // [KEYBOARD] [UP] [EVENT]

        // EVENT Keypress UID1343430442
        public void DiagramApp_KeyPress(object sender, KeyPressEventArgs e) // [KEYBOARD] [PRESS] [EVENT]
        {

#if DEBUG
            this.LogEvent("KeyPress");
#endif

            bool isreadonly = this.diagram.IsReadOnly();

            if (this.IsEditing() || this.stateSearching)
            {
                return;
            }

            this.key = e.KeyChar;

            // KEY PLUS In to layer
            if (!this.keyctrl
                && !this.keyalt
                && this.key == '+'
                && this.selectedNodes.Count() == 1
                && this.selectedNodes[0].haslayer)
            {
                this.LayerIn(this.selectedNodes[0]);
                return;
            }

            // KEY MINUS Out to layer
            if (!this.keyctrl
                && !this.keyalt
                && this.currentLayer != null
                && this.key == '-')
            {
                this.LayerOut();
                return;
            }

            // KEY OTHER create new node
            if (!isreadonly
                && !this.keyctrl
                && !this.keyalt
                && this.key != ' '
                && this.key != '\t'
                && this.key != '\r'
                && this.key != '\n'
                && this.key != '`'
                && this.key != (char)27
            )
            {
                this.editPanel.ShowEditPanel(this.CursorPosition(), this.key);
            }

        }

        // EVENT DROP file UID3440232213
        public void DiagramApp_DragDrop(object sender, DragEventArgs e)                                // [DROP] [DROP-FILE] [EVENT]
        {

#if DEBUG
            this.LogEvent("DragDrop");
#endif

            bool acceptedAction = this.main.plugins.DropAction(this, e);

            if (acceptedAction)
            {
                return;
            }

            try
            {
                Nodes newNodes = new Nodes();

                string[] formats = e.Data.GetFormats();
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                Position mousePosition = this.GetMousePosition();

                foreach (string file in files)
                {
                    Node newrec = this.CreateNode(mousePosition);
                    newNodes.Add(newrec);
                    newrec.SetName(Os.GetFileName(file));

                    newrec.link = file;
                    if (Os.DirectoryExists(file)) // directory
                    {
                        newrec.link = Os.MakeRelative(file, this.diagram.FileName);
                        newrec.color.Set(Media.GetColor(diagram.options.colorDirectory));
                    }
                    else
                    if (Os.Exists(file))
                    {
                        newrec.color.Set(Media.GetColor(diagram.options.colorFile));

                        if (this.diagram.FileName != "" && Os.FileExists(this.diagram.FileName)) // DROP FILE - skratenie cesty k suboru
                        {
                            newrec.link = Os.MakeRelative(file, this.diagram.FileName);
                        }

                        string ext = "";
                        ext = Os.GetExtension(file).ToLower();

                        if (ext == ".jpg" || ext == ".png" || ext == ".ico" || ext == ".bmp") // DROP IMAGE skratenie cesty k suboru
                        {
                            newrec.isimage = true;
                            newrec.imagepath = Os.MakeRelative(file, this.diagram.FileName);
                            newrec.image = Media.GetImage(file);
                            if (ext != ".ico") newrec.image.MakeTransparent(Color.White);
                            newrec.height = newrec.image.Height;
                            newrec.width = newrec.image.Width;
                        }
                        else
                        if (ext == ".lnk") // [LINK] [DROP] extract target
                        {
                            try
                            {
                                string[] shortcutInfo = Os.GetShortcutTargetFile(file);

                                Bitmap icoLnk = Media.ExtractLnkIcon(file);
                                if (icoLnk != null)// extract icon
                                {
                                    newrec.isimage = true;
                                    newrec.embeddedimage = true;
                                    newrec.image = icoLnk;
                                    newrec.image.MakeTransparent(Color.White);
                                    newrec.height = newrec.image.Height;
                                    newrec.width = newrec.image.Width;
                                }
                                else if (shortcutInfo[0] != "" && Os.FileExists(shortcutInfo[0]))
                                {
                                    Bitmap icoExe = Media.ExtractSystemIcon(shortcutInfo[0]);

                                    if (icoExe != null)// extract icon
                                    {
                                        newrec.isimage = true;
                                        newrec.embeddedimage = true;
                                        newrec.image = icoExe;
                                        newrec.image.MakeTransparent(Color.White);
                                        newrec.height = newrec.image.Height;
                                        newrec.width = newrec.image.Width;
                                    }
                                }

                                newrec.link = Os.MakeRelative(file, this.diagram.FileName);
                            }
                            catch (Exception ex)
                            {
                                Program.log.Write("extract icon from lnk error: " + ex.Message);
                            }
                        }
                        else
                        {
                            Bitmap ico = Media.ExtractSystemIcon(file);
                            if (ico != null)// extract icon
                            {
                                newrec.isimage = true;
                                newrec.embeddedimage = true;
                                newrec.image = ico;
                                newrec.image.MakeTransparent(Color.White);
                                newrec.height = newrec.image.Height;
                                newrec.width = newrec.image.Width;
                            }
                        }
                    }
                }

                if (newNodes.Count > 0)
                {
                    this.SelectNodes(newNodes);
                    this.diagram.AlignCompact(newNodes);
                    this.diagram.SortNodesAsc(newNodes);
                    this.diagram.Unsave("create", newNodes, null, this.shift, this.scale, this.currentLayer.id);
                }


            }
            catch (Exception ex)
            {
                Program.log.Write("drop file goes wrong: error: " + ex.Message);
            }
        }

        // EVENT DROP drag enter UID0040214033
        public void DiagramApp_DragEnter(object sender, DragEventArgs e)                               // [DRAG] [EVENT]
        {

#if DEBUG
            this.LogEvent("DragEnter");
#endif

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        // EVENT Resize UID2004324112
        public void DiagramApp_Resize(object sender, EventArgs e)                                      // [RESIZE] [EVENT]
        {

#if DEBUG
            this.LogEvent("Resize");
#endif

            if (WindowState == FormWindowState.Maximized)
            {
                oldFormWindowState = FormWindowState.Maximized;
            }

            if (WindowState == FormWindowState.Normal)
            {
                oldFormWindowState = FormWindowState.Normal;
            }

            if (this.stateZooming)
            {
                this.stateZooming = false;
                this.scale = this.zoomingDefaultScale;
                this.diagram.InvalidateDiagram();
            }

            // scrollbar - obnova po zmene šírky obrazovky
            if (bottomScrollBar != null && rightScrollBar != null)
            {
                bottomScrollBar.Resize(this.ClientRectangle.Width, this.ClientRectangle.Height);
                rightScrollBar.Resize(this.ClientRectangle.Width, this.ClientRectangle.Height);
            }

            if (this.diagram != null)
            {
                this.diagram.InvalidateDiagram();
            }
        }

        // RESTORE FORM
        public void RestoreFormWindowState()
        {
            Media.BringToFront(this);

            if (this.oldFormWindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            if (this.oldFormWindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        // EVENT MOVE TIMER for move view when node is draged to window edge UID2144001341
        public void MoveTimer_Tick(object sender, EventArgs e)
        {

#if DEBUG
            this.LogEvent("MoveTimer_Tick");
#endif

            if (this.stateDragSelection || this.stateSelectingNodes || this.stateAddingNode)
            {
                bool changed = false;

                if (this.ClientSize.Width - 20 < this.actualMousePos.x)
                {
                    this.shift.x -= 10 * Tools.GetScale(this.scale);
                    changed = true;
                }

                if (this.actualMousePos.x < 20)
                {
                    this.shift.x += 10 * Tools.GetScale(this.scale);
                    changed = true;
                }

                if (this.ClientSize.Height - 50 < this.actualMousePos.y)
                {
                    this.shift.y -= 10 * Tools.GetScale(this.scale);
                    changed = true;
                }

                if (this.actualMousePos.y < 10)
                {
                    this.shift.y += 10 * Tools.GetScale(this.scale);
                    changed = true;
                }

                if (this.stateDragSelection) // drag selected  nodes
                {
                    if (this.sourceNode != null && this.sourceNode.selected)
                    {
                        Position vector = new Position();

                        if (this.stateCoping)
                        {
                            // calculate shift between start node position and current sourceNode position
                            vector
                                .Set(this.actualMousePos)
                                .Scale(Tools.GetScale(this.scale))
                                .Subtract(this.vmouse)
                                .Subtract(this.shift)
                                .Subtract(this.copySourceNode.position);

                            foreach (Node node in this.copySelectedNodes)
                            {
                                node.position.Add(vector);
                            }
                        }
                        else
                        if (this.selectedNodes.Count > 0)
                        {
                            // calculate shift between start node position and current sourceNode position
                            vector
                                .Set(this.actualMousePos)
                                .Scale(Tools.GetScale(this.scale))
                                .Subtract(this.vmouse)
                                .Subtract(this.shift)
                                .Subtract(this.sourceNode.position);

                            foreach (Node node in this.selectedNodes)
                            {
                                node.position.Add(vector);
                            }
                        }

                        changed = true;
                    }
                }

                if (changed) this.diagram.InvalidateDiagram();
            }
        }                                      // [MOVE] [TIMER] [EVENT]

        // EVENT Deactivate - lost focus UID0104120032
        public void DiagramApp_Deactivate(object sender, EventArgs e)                                  // [FOCUS]
        {

#if DEBUG
            this.LogEvent("Deactivate");
#endif

            this.keyctrl = false;
            this.keyalt = false;
            this.keyshift = false;
            if (this.stateZooming)
            {
                this.stateZooming = false;
                this.scale = this.zoomingDefaultScale;
            }
            this.stateDragSelection = false;
            this.stateAddingNode = false;
            this.stateSelectingNodes = false;
            this.stateMoveView = false;

            this.diagram.InvalidateDiagram();

            if (this.diagram.options.pinWindow &&
                this.WindowState != FormWindowState.Minimized &&
                this.WindowState != FormWindowState.Maximized &&
                this.Visible &&
                this.Width > 64 &&
                this.Height > 64)
            {



                windowIsPinned = true;
                windowWidthBeforePin = this.Width;
                windowHeightBeforePin = this.Height;
                windowBorderStyleBeforePin = this.FormBorderStyle;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Width = 64;
                this.Height = 64;
                this.TopMost = true;
                this.windowPinBox.Visible = true;

            }
        }

        /*************************************************************************************************************************/

        // LAYER layer in or open edit form UID4538903767
        public void LayerInOrEdit()
        {
            if (this.selectedNodes.Count() == 1)
            {
                if (this.selectedNodes[0].haslayer)
                {
                    this.LayerIn(this.selectedNodes[0]);
                }
                else
                {
                    this.diagram.EditNode(this.selectedNodes[0]);
                }
            }
        }

        // LAYER layer in UID5010004621
        public void LayerIn()
        {
            if (this.selectedNodes.Count() == 1)
            {
                this.LayerIn(this.selectedNodes[0]);
            }
        }

        // LAYER IN UID3904383109                                                                     // [LAYER]
        public void LayerIn(Node node)
        {
            if (this.currentLayer.parentNode == null)
            {
                this.firstLayereShift.Set(shift);
                this.firstLayereScale = this.scale;
            }
            else
            {
                this.currentLayer.parentNode.layerShift.Set(this.shift);
                this.currentLayer.parentNode.layerScale = this.scale;
            }

            this.currentLayer = this.diagram.layers.GetOrCreateLayer(node);
            this.currentLayer.parentNode.haslayer = true;
            this.layersHistory.Add(this.currentLayer);
            this.shift.Set(this.currentLayer.parentNode.layerShift);
            this.scale = this.currentLayer.parentNode.layerScale;
            this.SetTitle();
            this.breadcrumbs.Update();
            this.diagram.InvalidateDiagram();
        }

        // LAYER OUT UID4661843385
        public void LayerOut()
        {
            if (this.currentLayer.parentLayer != null)
            { //this layer is not top layer

                this.currentLayer.parentNode.layerShift.Set(this.shift);
                this.currentLayer.parentNode.layerScale = this.scale;

                if (this.currentLayer.nodes.Count() == 0)
                {
                    this.currentLayer.parentNode.haslayer = false;
                }

                this.currentLayer = this.currentLayer.parentLayer;

                layersHistory.RemoveAt(layersHistory.Count() - 1);

                if (this.currentLayer.parentNode == null)
                {
                    this.shift.Set(this.firstLayereShift);
                    this.scale = this.firstLayereScale;
                }
                else
                {
                    this.shift.Set(this.currentLayer.parentNode.layerShift);
                    this.scale = this.currentLayer.parentNode.layerScale;
                }

                this.SetTitle();
                this.breadcrumbs.Update();
                this.diagram.InvalidateDiagram();
            }
        }

        // LAYER Is not in first layer
        public bool isNotInFisrtLayer()
        {
            if (this.currentLayer.parentLayer != null)
            {
                return true;
            }

            return false;
        }


        // LAYER HISTORY Buld laier history from UID3310785252
        public void BuildLayerHistory(long id)
        {
            Layer layer = this.diagram.layers.GetLayer(id);

            if (layer == null)
            {
                return;
            }

            this.currentLayer = layer;

            layersHistory.Clear();

            Layer temp = this.currentLayer;
            while (temp != null)
            {
                layersHistory.Add(temp);
                temp = temp.parentLayer;
            }

            layersHistory.Reverse(0, layersHistory.Count());

            this.breadcrumbs.Update();
        }

        // LAYER check if node is parent trought layer history UID4653357181
        public bool IsNodeInLayerHistory(Node rec)
        {
            foreach (Layer layer in this.layersHistory)
            {
                if (layer.id == rec.id)
                {
                    return true;
                }
            }

            return false;
        }



        /*************************************************************************************************************************/

        // SEARCHPANEL action UID7186033387
        public void SearchPanelChanged(string action, string search)
        {
            if (action == "search")
            {
                this.SearchFirst(search);
            }

            if (action == "searchNext")
            {
                this.SearchNext();
            }

            if (action == "searchPrev")
            {
                this.SearchPrev();
            }

            if (action == "cancel")
            {
                this.SearchCancel();
            }

            if (action == "close")
            {
                this.SearchClose();
            }
        }

        // SEARCHPANEL SHOW UID3966468665
        private void ShowSearchPanel()
        {
            if (searhPanel == null)
            {
                searhPanel = new SearchPanel(this);
                this.searhPanel.SearchpanelStateChanged += this.SearchPanelChanged;
                this.Controls.Add(this.searhPanel);
            }

            currentPosition.x = this.shift.x;
            currentPosition.y = this.shift.y;
            currentPositionLayer = this.currentLayer.id;
            currentPositionScale = this.scale;

            searhPanel.ShowPanel();
            this.stateSearching = true;
        }

        /// <summary>
        ///  SEARCH FIRST
        ///
        /// Build array of search results and then select first Node.
        ///
        /// </summary>
        /// <param name="find">Search string</param>
        public void SearchFirst(string find) //UID1194762485
        {

            Nodes foundNodes = new Nodes();

            this.lastFound = -1;
            this.searchFor = find;

            // get all nodes containing string
            foreach (Node node in this.diagram.GetAllNodes())
            {
                if (node.note.ToUpper().IndexOf(searchFor.ToUpper()) != -1
                    || node.name.ToUpper().IndexOf(searchFor.ToUpper()) != -1
                    || node.link.ToUpper().IndexOf(searchFor.ToUpper()) != -1)
                {
                    foundNodes.Add(node);
                }
            }

            this.searhPanel.Highlight(foundNodes.Count() == 0);

            // get center ov view
            Position middle = new Position();
            middle.Copy(this.currentPosition);
            middle.x -= this.ClientSize.Width / 2;
            middle.y -= this.ClientSize.Height / 2;

            long currentLayerId = this.currentLayer.id;

            foundNodes.Sort((first, second) =>
            {
                // sort by layers
                if (first.layer < second.layer)
                {
                    // current layer first
                    if (currentLayerId == second.layer)
                    {
                        return 1;
                    }

                    return -1;
                }

                // sort by layers
                if (first.layer > second.layer)
                {
                    // current layer first
                    if (currentLayerId == first.layer)
                    {
                        return -1;
                    }

                    return 1;
                }

                // compare distance from center of view when shearch panel is open

                Node parent = this.diagram.layers.GetLayer(first.layer).parentNode;
                Position m = (currentLayerId == first.layer) ? middle : (parent != null) ? parent.layerShift : firstLayereShift;
                double d1 = first.position.ConvertToStandard().Distance(m);
                double d2 = second.position.ConvertToStandard().Distance(m);

                // sort by distance if is same layer
                if (d1 < d2)
                {
                    return -1;
                }
                else
                {
                    if (d1 > d2)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            });

            nodesSearchResult.Clear();
            for (int i = 0; i < foundNodes.Count(); i++)
            {
                nodesSearchResult.Add(foundNodes[i].id);
            }

            this.SearchNext();

        }

        /// <summary>
        /// SEARCH NEXT
        ///
        /// Search node in cycle. Find first in array or start in begining of array
        /// </summary>
        public void SearchNext() //UID2131053451
        {
            Node node = null;

            if (nodesSearchResult.Count() == 0)
                return;

            // search from last position to end
            for (int i = lastFound + 1; i < nodesSearchResult.Count(); i++)
            {
                node = this.diagram.GetNodeByID(nodesSearchResult[i]);

                if (node != null)
                {
                    lastFound = i;
                    break;
                }
            }

            // search from fist search result to actual position (in cycle)
            if (node == null)
            {
                for (int i = 0; i < lastFound; i++)
                {
                    node = this.diagram.GetNodeByID(nodesSearchResult[i]);

                    if (node != null)
                    {
                        lastFound = i;
                        break;
                    }
                }
            }

            if (node != null)
            {
                this.GoToNodeWithAnimation(node);
                this.SelectOnlyOneNode(node);
                this.diagram.InvalidateDiagram();
            }

        }

        // SEARCH PREV
        public void SearchPrev() //UID5583938471
        {
            Node node = null;

            if (nodesSearchResult.Count() == 0)
                return;

            if (lastFound == -1)
            {
                lastFound = 0;
            }

            // search from lastfound to begging of search results
            for (int i = lastFound - 1; i >= 0; i--)
            {
                node = this.diagram.GetNodeByID(nodesSearchResult[i]);

                if (node != null)
                {
                    lastFound = i;
                    break;
                }
            }

            // search from end of search result to last found (in cycle)
            if (node == null)
            {
                for (int i = nodesSearchResult.Count() - 1; i >= lastFound; i--)
                {
                    node = this.diagram.GetNodeByID(nodesSearchResult[i]);

                    if (node != null)
                    {
                        lastFound = i;
                        break;
                    }
                }
            }

            if (node != null)
            {
                this.GoToNodeWithAnimation(node);
                this.SelectOnlyOneNode(node);
                this.diagram.InvalidateDiagram();
            }
        }



        // SEARCHPANEL CANCEL - restore beggining search position
        private void SearchCancel()
        {
            this.GoToLayer(currentPositionLayer);
            this.shift.x = currentPosition.x;
            this.shift.y = currentPosition.y;
            this.scale = currentPositionScale;


            this.SearchClose();
        }

        // SEARCHPANEL Close - close search panel
        private void SearchClose()
        {
            this.Focus();
            this.stateSearching = false;
            this.diagram.InvalidateDiagram();
        }



        /*************************************************************************************************************************/

        // CLIPBOARD Copy link to clipboard
        public void CopyLinkToClipboard(Node node)
        {
            Clipboard.SetText(node.link);
        }

        /*************************************************************************************************************************/

        // SCROLLBAR MOVE LEFT-RIGHT set current position in view with relative (0-1) number          // SCROLLBAR
        public void MoveScreenHorizontal(double per)
        {
            decimal minx = decimal.MaxValue;
            decimal maxx = decimal.MinValue;
            foreach (Node rec in this.currentLayer.nodes)
            {
                if (rec.layer == this.currentLayer.id || rec.id == this.currentLayer.id)
                {
                    if (rec.position.x < minx) minx = rec.position.x;
                    if (maxx < rec.position.x + rec.width) maxx = rec.position.x + rec.width;
                }
            }

            if (minx != decimal.MaxValue && maxx != decimal.MinValue)
            {
                minx -= 100 * Tools.GetScale(this.scale);
                maxx += 100 * Tools.GetScale(this.scale) - this.ClientSize.Width * Tools.GetScale(this.scale);
                this.shift.x = (-(minx + (maxx - minx) * (decimal)per));
            }
            else
            {
                this.shift.x = 0;
            }
        }

        // SCROLLBAR GET POSITION LEFT-RIGHT calculate current position in view as relative (0-1) number
        public double GetPositionHorizontal()
        {
            decimal per;
            decimal minx = decimal.MaxValue;
            decimal maxx = decimal.MinValue;
            foreach (Node rec in this.currentLayer.nodes)
            {
                if (rec.layer == this.currentLayer.id || rec.id == this.currentLayer.id)
                {
                    if (rec.position.x < minx) minx = rec.position.x;
                    if (maxx < rec.position.x + rec.width) maxx = rec.position.x + rec.width;
                }
            }

            if (minx != decimal.MaxValue && maxx != decimal.MinValue)
            {
                minx -= 100 * Tools.GetScale(this.scale);
                maxx += 100 * Tools.GetScale(this.scale) - this.ClientSize.Width * Tools.GetScale(this.scale);
                per = (-this.shift.x - minx) / (maxx - minx);
                if (per < 0) per = 0;
                if (per > 1) per = 1;
                return (double)per;
            }
            else
            {
                return 0;
            }

        }

        // SCROLLBAR MOVE UP-DOWN set current position in view with relative (0-1) number
        public void MoveScreenVertical(double per)
        {
            decimal miny = decimal.MaxValue;
            decimal maxy = decimal.MinValue;
            foreach (Node rec in this.currentLayer.nodes)
            {
                if (rec.layer == this.currentLayer.id || rec.id == this.currentLayer.id)
                {
                    if (rec.position.y < miny) miny = rec.position.y;
                    if (maxy < rec.position.y + rec.height) maxy = rec.position.y + rec.height;
                }
            }

            if (miny != decimal.MaxValue && maxy != decimal.MinValue)
            {
                miny -= 100 * Tools.GetScale(this.scale);
                maxy += 100 * Tools.GetScale(this.scale) - this.ClientSize.Height * Tools.GetScale(this.scale);
                this.shift.y = -(miny + (maxy - miny) * (decimal)per);
            }
            else
            {
                this.shift.y = 0;
            }
        }

        // SCROLLBAR GET POSITION LEFT-RIGHT calculate current position in view as relative (0-1) number
        public double GetPositionVertical()
        {
            decimal per;
            decimal miny = decimal.MaxValue;
            decimal maxy = decimal.MinValue;
            foreach (Node rec in this.currentLayer.nodes)
            {
                if (rec.layer == this.currentLayer.id || rec.id == this.currentLayer.id)
                {
                    if (rec.position.y < miny) miny = rec.position.y;
                    if (maxy < rec.position.y + rec.height) maxy = rec.position.y + rec.height;
                }
            }

            if (miny != decimal.MaxValue && maxy != decimal.MinValue)
            {
                miny -= 100 * Tools.GetScale(this.scale);
                maxy += 100 * Tools.GetScale(this.scale) - this.ClientSize.Height * Tools.GetScale(this.scale);
                per = (decimal)(-this.shift.y - miny) / (maxy - miny);
                if (per < 0) per = 0;
                if (per > 1) per = 1;
                return (double)per;
            }
            else
            {
                return 0;
            }
        }

        // SCROLLBAR EVENT position is changed by horizontal scrollbar
        public void PositionChangeBottom(object source, PositionEventArgs e)
        {
            MoveScreenHorizontal(e.GetPosition());
        }

        // SCROLLBAR EVENT position is changed by vertical scrollbar
        public void PositionChangeRight(object source, PositionEventArgs e)
        {
            MoveScreenVertical(e.GetPosition());
        }

        public void SetScrollbarColor(ColorType color)
        {
            this.rightScrollBar.SetColor(color.Get());
            this.bottomScrollBar.SetColor(color.Get());
        }

        /*************************************************************************************************************************/

        // FILE Save - Save diagram
        public void Save() //UID1784785672
        {
            if (!this.diagram.IsLocked())
            {
                if (!this.diagram.Save())
                {
                    this.Saveas();
                }
            }
        }

        // FILE SAVEAS - Save as diagram 
        public bool Saveas() //UID4040264682
        {
            if (this.DSave.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            this.diagram.Saveas(this.DSave.FileName);
            this.main.programOptions.AddRecentFile(this.DSave.FileName);
            return true;
        }

        // FILE Open - Open diagram dialog UID5922343203
        public void OpenFileDialog()
        {
            if (DOpen.ShowDialog() == DialogResult.OK)
            {
                if (Os.FileExists(DOpen.FileName))
                {
                    if (Os.GetExtension(DOpen.FileName).ToLower() == ".diagram")
                    {
                        this.OpenDiagramFromFile(DOpen.FileName);
                    }
                    else
                    {
                        MessageBox.Show(Translations.wrongFileExtenson);
                    }
                }
            }
        }

        // FILE Open - Open diagram UID4892447333
        public void OpenDiagramFromFile(String path)
        {
            if (Os.FileExists(path))
            {
                this.main.OpenDiagram(path);
                if (this.diagram.IsNew())
                {
                    this.Close();
                }
            }
        }

        /*************************************************************************************************************************/

        // EXPORT Export diagram to png
        public void ExportDiagramToPng()
        {
            Nodes nodes;

            if (this.selectedNodes.Count == 0)
            {
                nodes = this.currentLayer.nodes;
            }
            else
            {
                nodes = this.selectedNodes;
            }

            if (nodes.Count > 0)
            {

                decimal minx = nodes[0].position.x;
                decimal maxx = nodes[0].position.x + nodes[0].width;
                decimal miny = nodes[0].position.y;
                decimal maxy = nodes[0].position.y + nodes[0].height;

                foreach (Node rec in nodes) // Loop through List with foreach
                {
                    if (rec.position.x < minx)
                    {
                        minx = rec.position.x;
                    }

                    if (maxx < rec.position.x + rec.width)
                    {
                        maxx = rec.position.x + rec.width;
                    }

                    if (rec.position.y < miny)
                    {
                        miny = rec.position.y;
                    }

                    if (maxy < rec.position.y + rec.height)
                    {
                        maxy = rec.position.y + rec.height;
                    }
                }

                minx -= 100;
                maxx += 100;
                miny -= 100;
                maxy += 100;

                Bitmap bmp = new Bitmap((int)(maxx - minx), (int)(maxy - miny));
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(this.BackColor);
                this.DrawDiagram(g, new Position(this.shift).Invert().Subtract(minx, miny), true);
                g.Dispose();
                bmp.Save(exportFile.FileName, ImageFormat.Png);
                bmp.Dispose();
            }
        }

        // EXPORT Export diagram to txt
        public void ExportDiagramToTxt(string filePath)
        {

            Nodes nodes;

            if (this.selectedNodes.Count == 0)
            {
                nodes = this.diagram.GetAllNodes();
            }
            else
            {
                nodes = this.selectedNodes;
            }

            string outtext = "";

            foreach (Node rec in nodes)
            {
                outtext += rec.name + "\n" + (rec.link != "" ? rec.link + "\n" : "") + "\n" + rec.note + "\n---\n";
            }
            Os.WriteAllText(filePath, outtext);
        }

        /*************************************************************************************************************************/

        // DRAW UID4637488042                                                                                     // [DRAW]
        private void DrawDiagram(Graphics gfx, Position correction = null, bool export = false)
        {
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            if (this.diagram.options.grid && !export)
            {
                this.DrawGrid(gfx);
            }

            if (this.diagram.IsLocked())
            {
                this.DrawLockScreen(gfx);
                return;
            }

            this.DrawLines(gfx, this.currentLayer.lines, correction, export);

            if (!export && this.stateCoping)
            {
                this.DrawLines(gfx, this.copySelectedLines, correction, export);
            }



            // DRAW addingnode
            if (!export && this.stateAddingNode && !this.stateZooming && (this.actualMousePos.x != this.startMousePos.x || this.actualMousePos.y != this.startMousePos.y))
            {
                this.DrawAddNode(gfx);
            }

            this.DrawNodes(gfx, this.currentLayer.nodes, correction, export);

            if (!export && this.stateCoping)
            {
                this.DrawNodes(gfx, this.copySelectedNodes, correction, export);
            }

            // DRAW select - select nodes by mouse drag (blue rectangle - multiselect)
            if (!export && this.stateSelectingNodes && (this.actualMousePos.x != this.startMousePos.x || this.actualMousePos.y != this.startMousePos.y))
            {
                this.DrawNodesSelectArea(gfx);
            }

            // PREVIEW draw zoom mini screen
            if (!export && this.stateZooming)
            {
                this.DrawMiniScreen(gfx);
            }

            // DRAW coordinates
            if (this.diagram.options.coordinates)
            {
                this.DrawCoordinates(gfx);
            }

            // DRAW addingnode
            if (!export)
            {
                this.breadcrumbs.Draw(gfx);
            }

            // DRAW addingnode
            if (this.zoomTimer.Enabled)
            {
                this.DrawZoomScaleInfo(gfx);
            }
        }

        // DRAW grid UID7187365714
        private void DrawGrid(Graphics gfx)
        {
            decimal s = Tools.GetScale(this.scale);


            decimal sw = this.ClientSize.Width;  // get windows dize
            decimal sh = this.ClientSize.Height;

            decimal sqaresize = 100;
            decimal m = sqaresize / s; // get 100px size in current scale
            while (m < 20)
            {
                sqaresize *= 10;
                m = sqaresize / s;
            }

            while (m > sw)
            {
                sqaresize /= 10;
                m = sqaresize / s;
            }

            // skip drawwing to small pr to high grid
            if (m < 5 || sw < m || sh < m)
            {
                return;
            }

            decimal lwc = sw / m + 1; // count how meny lines can by written in current view
            decimal lhc = sh / m + 1;
            decimal six = this.shift.x / s % m; // calculate first line position
            decimal siy = this.shift.y / s % m;

            Pen myPen = new Pen(this.diagram.options.gridColor.Get(), 1);
            using (myPen)
            {
                for (int i = 0; i <= lwc; i++) // dreaw vertical lines
                {
                    PointF[] points =
                    {
                     new PointF((float)(six + i * m),  0),
                     new PointF((float)(six + i * m), (float)sh),
                 };

                    gfx.DrawLines(myPen, points);
                }

                for (int i = 0; i <= lhc; i++) // draw horizontal lines
                {
                    PointF[] points =
                    {
                     new PointF(0, (float)(siy + i * m)),
                     new PointF((float)sw, (float)(siy + i * m)),
                 };

                    gfx.DrawLines(myPen, points);
                }
            }
        }

        // DRAW lock screen  UID7187365714
        private void DrawLockScreen(Graphics gfx)
        {
            if (lockImage == null)
            {
                return;
            }

            int X = (this.ClientSize.Width - this.lockImage.Width) / 2;
            int Y = (this.ClientSize.Height - this.lockImage.Height) / 2;
            gfx.DrawImage(this.lockImage, new Point(X, Y));
        }

        // DRAW diagram mini screen in zoom mode UID9733202717
        private void DrawMiniScreen(Graphics gfx)
        {
            decimal s = Tools.GetScale(this.scale);
            Pen myPen = new Pen(Color.Gray, 1);

            RectangleF rectBorder = new RectangleF(
                (float)((this.ClientSize.Width / 2 - this.ClientSize.Width / 2 / s * Tools.GetScale(this.currentScale))),
                (float)((this.ClientSize.Height / 2 - this.ClientSize.Height / 2 / s * Tools.GetScale(this.currentScale))),
                (float)(this.ClientSize.Width / s * Tools.GetScale(this.currentScale)),
                (float)(this.ClientSize.Height / s * Tools.GetScale(this.currentScale))
            );

            RectangleF[] rects = { rectBorder };

            gfx.DrawRectangles(
                myPen,
                rects
            );
        }

        // DRAW coordinates for debuging UID7119976091
        private void DrawCoordinates(Graphics gfx)
        {
            decimal s = Tools.GetScale(this.scale);

            Font drawFont = new Font("Arial", 10);
            SolidBrush drawBrush = new SolidBrush(this.diagram.options.backgroundColor.Invert());
            gfx.DrawString(
                this.shift.x.ToString() + "sx," +
                        this.shift.y.ToString() +
                        "sy (" + this.ClientSize.Width.ToString() + "w x " + this.ClientSize.Height.ToString() + "h) " +
                        "" + s.ToString() + "s," + this.currentScale.ToString() + "cs",
                drawFont,
                drawBrush,
                10,
                10
            );
        }

        // DRAW draw info if zooming UID4537424673
        private void DrawZoomScaleInfo(Graphics gfx)
        {
            string text = this.scale.ToString();
            Font drawFont = new Font("Arial", 25);
            SolidBrush drawBrush = new SolidBrush(this.diagram.options.backgroundColor.Invert());
            gfx.DrawString(
                text,
                drawFont,
                drawBrush,
                30,
                this.Height - 100
            );
        }

        // DRAW select node by mouse drag (blue rectangle) UID1806594258
        private void DrawNodesSelectArea(Graphics gfx)
        {
            SolidBrush brush = new SolidBrush(Color.FromArgb(200, this.diagram.options.selectionColor.Get()));

            decimal a = this.shift.x - this.startShift.x + this.startMousePos.x * Tools.GetScale(this.scale);
            decimal b = this.shift.y - this.startShift.y + this.startMousePos.y * Tools.GetScale(this.scale);
            decimal c = this.actualMousePos.x * Tools.GetScale(this.scale);
            decimal d = this.actualMousePos.y * Tools.GetScale(this.scale);
            decimal temp;
            if (c < a) { temp = a; a = c; c = temp; }
            if (d < b) { temp = d; d = b; b = temp; }


            RectangleF rect = new RectangleF(
                    (float)(a / Tools.GetScale(this.scale)),
                    (float)(b / Tools.GetScale(this.scale)),
                    (float)((c - a) / Tools.GetScale(this.scale)),
                    (float)((d - b) / Tools.GetScale(this.scale))
            );

            gfx.FillRectangle(brush, rect);
        }

        // DRAW add new node by drag UID3527460113
        private void DrawAddNode(Graphics gfx)
        {
            Pen myPen = new Pen(this.diagram.options.lineColor.Get(), 1);

            if (this.sourceNode == null)
            {
                Position p = this.startMousePos.Clone()
                    .Subtract(this.startShift)
                    .Add(this.shift)
                    .Add(10);// TODO: missing scale

                PointF[] points =
                {
                    new PointF((float)p.x, (float)p.y),
                    new PointF((float)this.actualMousePos.x, (float)this.actualMousePos.y),
                };

                gfx.DrawLines(
                    myPen,
                    points
                );
            }
            else
            {
                Position p = this.sourceNode.position.Clone()
                    .Add(this.shift)
                    .Split(Tools.GetScale(this.scale))
                    .Add(10);

                PointF[] points =
                {
                    new PointF((float)p.x, (float)p.y),
                    new PointF((float)this.actualMousePos.x, (float)this.actualMousePos.y),
                };

                gfx.DrawLines(
                    myPen,
                    points
                );
            }
        }

        // DRAW nodes UID4202302087
        private void DrawNodes(Graphics gfx, Nodes nodes, Position correction = null, bool export = false)
        {
            decimal s = Tools.GetScale(this.scale);

            Pen nodeBorder = new Pen(this.diagram.options.selectedNodeColor.Get(), 1);
            Pen nodeSelectBorder = new Pen(this.diagram.options.selectedNodeColor.Get(), 3);
            Pen nodeLinkBorder = new Pen(Color.DarkRed, 3);
            Pen nodeMarkBorder = new Pen(Color.Navy, 3);

            // fix position for image file export
            decimal cx = 0;
            decimal cy = 0;
            if (correction != null)
            {
                cx = correction.x;
                cy = correction.y;
            }

            // DRAW nodes
            foreach (Node rec in nodes) // Loop through List with foreach
            {

                if ((export || this.NodeIsVisible(rec)) && rec.visible)
                {
                    if (rec.isimage)
                    {
                        // DRAW Image

                        RectangleF imageRec = new RectangleF(
                            (float)((this.shift.x + cx + rec.position.x) / s),
                            (float)((this.shift.y + cy + rec.position.y) / s),
                            (float)((rec.width) / (s / Tools.GetScale(rec.scale))),
                            (float)((rec.height) / (s / Tools.GetScale(rec.scale)))
                        );

                        gfx.DrawImage(rec.image, imageRec);

                        if (rec.selected && !export)
                        {
                            RectangleF rectBorder = new RectangleF(
                                (float)((this.shift.x + cx + rec.position.x) / s),
                                (float)((this.shift.y + cy + rec.position.y) / s),
                                (float)((rec.width) / (s / Tools.GetScale(rec.scale))),
                                (float)((rec.height) / (s / Tools.GetScale(rec.scale)))
                            );

                            RectangleF[] rects = { rectBorder };

                            gfx.DrawRectangles(
                                nodeSelectBorder,
                                rects
                            );
                        }

                    }
                    else
                    {
                        if (this.diagram.options.coordinates) // draw debug information
                        {
                            decimal size = 10 / (s / Tools.GetScale(rec.scale));
                            if (0 < size && size < 200)
                            {
                                Font drawFont = new Font("Arial", (float)size);
                                SolidBrush drawBrush = new SolidBrush(this.diagram.options.backgroundColor.Invert());

                                PointF infoPosition = new PointF(
                                    (float)((this.shift.x + rec.position.x) / s),
                                    (float)((this.shift.y + rec.position.y) / s - ((decimal)20 / (s / Tools.GetScale(rec.scale))))
                                );

                                gfx.DrawString(
                                    rec.id.ToString() + "i:" + rec.scale.ToString() + "s:" + (rec.position.x).ToString() + "x," + (rec.position.y).ToString() + "y",
                                    drawFont,
                                    drawBrush,
                                    infoPosition
                                );

                            }
                        }


                        // DRAW border

                        if (rec.name.Trim() == "") // draw empty point
                        {
                            if (!rec.transparent) // draw fill point
                            {
                                RectangleF rect1 = new RectangleF(
                                    (float)((this.shift.x + cx + rec.position.x) / s),
                                    (float)((this.shift.y + cy + rec.position.y) / s),
                                    (float)((rec.width) / (s / Tools.GetScale(rec.scale))),
                                    (float)((rec.height) / (s / Tools.GetScale(rec.scale)))
                                );

                                gfx.FillEllipse(new SolidBrush(rec.color.color), rect1);
                                if (this.diagram.options.borders) gfx.DrawEllipse(nodeBorder, rect1);
                            }

                            if (rec.haslayer && !export) // draw layer indicator
                            {
                                gfx.DrawEllipse(nodeBorder, new RectangleF(
                                        (float)((this.shift.x + cx + rec.position.x) / s),
                                        (float)((this.shift.y + cy + rec.position.y) / s),
                                        (float)((rec.width) / (s / Tools.GetScale(rec.scale))),
                                        (float)((rec.height) / (s / Tools.GetScale(rec.scale)))
                                    )
                                );
                            }

                            if (rec.selected && !export && !rec.transparent)
                            {
                                gfx.DrawEllipse(
                                    (rec.link != "") ? nodeLinkBorder : ((rec.mark) ? nodeMarkBorder : nodeSelectBorder),
                                    new RectangleF(
                                        (float)((this.shift.x + cx + rec.position.x) / s),
                                        (float)((this.shift.y + cy + rec.position.y) / s),
                                        (float)((rec.width) / (s / Tools.GetScale(rec.scale))),
                                        (float)((rec.height) / (s / Tools.GetScale(rec.scale)))
                                    )
                                );
                            }

                            if (rec.selected && !export && rec.transparent)
                            {

                                float r1x = (float)((this.shift.x + cx + rec.position.x + (rec.width * Tools.GetScale(rec.scale)) / 2) / s);
                                float r1y = (float)((this.shift.y + cy + rec.position.y + (rec.height * Tools.GetScale(rec.scale)) / 2) / s);

                                gfx.DrawEllipse(
                                    (rec.link != "") ? nodeLinkBorder : ((rec.mark) ? nodeMarkBorder : nodeSelectBorder),
                                    new RectangleF(
                                        r1x - (float)rec.width / 2,
                                        r1y - (float)rec.height / 2,
                                        (float)rec.width,
                                        (float)rec.height
                                    )
                                );
                            }
                        }
                        else
                        {
                            // draw filled node rectangle
                            if (!rec.transparent)
                            {
                                RectangleF rect1 = new RectangleF(
                                    (float)((this.shift.x + cx + rec.position.x) / s),
                                    (float)((this.shift.y + cy + rec.position.y) / s),
                                    (float)((rec.width) / (s / Tools.GetScale(rec.scale))),
                                    (float)((rec.height) / (s / Tools.GetScale(rec.scale)))
                                );

                                RectangleF[] rects = { rect1 };

                                gfx.FillRectangle(new SolidBrush(rec.color.color), rect1);
                                if (this.diagram.options.borders) gfx.DrawRectangles(nodeBorder, rects);
                            }

                            // draw layer indicator
                            if (rec.haslayer && !export)
                            {
                                RectangleF[] rects =
                                {
                                      new RectangleF(
                                        (float)((this.shift.x + cx + rec.position.x) / s),
                                        (float)((this.shift.y + cy + rec.position.y) / s),
                                        (float)((rec.width) / (s / Tools.GetScale(rec.scale))),
                                        (float)((rec.height) / (s / Tools.GetScale(rec.scale)))
                                    )
                                 };

                                gfx.DrawRectangles(
                                    nodeBorder,
                                    rects
                                );
                            }

                            // draw selected node border
                            if (rec.selected && !export)
                            {
                                RectangleF[] rects =
                                {
                                     new RectangleF(
                                        (float)((this.shift.x + cx + rec.position.x) / s),
                                        (float)((this.shift.y + cy + rec.position.y) / s),
                                        (float)((rec.width) / (s / Tools.GetScale(rec.scale))),
                                        (float)((rec.height) / (s / Tools.GetScale(rec.scale)))
                                    )
                                 };

                                gfx.DrawRectangles(
                                    (rec.link != "") ? nodeLinkBorder : ((rec.mark) ? nodeMarkBorder : nodeSelectBorder),
                                    rects
                                );
                            }


                            // DRAW text
                            RectangleF rect2 = new RectangleF(
                                (float)((this.shift.x + cx + rec.position.x + (Node.NodePadding * Tools.GetScale(rec.scale))) / s),
                                (float)((this.shift.y + cy + rec.position.y + (Node.NodePadding * Tools.GetScale(rec.scale))) / s),
                                (float)((rec.width - Node.NodePadding) / (s / Tools.GetScale(rec.scale))),
                                (float)((rec.height - Node.NodePadding) / (s / Tools.GetScale(rec.scale)))
                            );

                            decimal size = (decimal)rec.font.Size / (s / Tools.GetScale(rec.scale));
                            if (1 < size && size < 200) //check if is not to small after zoom or too big
                            {
                                gfx.DrawString(
                                    (rec.protect) ? Node.protectedName : rec.name,
                                    new Font(
                                        rec.font.FontFamily,
                                        (float)size,
                                        rec.font.Style,
                                        GraphicsUnit.Point,
                                        ((byte)(0))
                                    ),
                                    new SolidBrush(rec.fontcolor.color),
                                    rect2
                                );
                            }
                        }
                    }
                }
            }


            nodeBorder.Dispose();
            nodeSelectBorder.Dispose();
            nodeLinkBorder.Dispose();
            nodeMarkBorder.Dispose();
        }

        // DRAW lines UID4936881338
        private void DrawLines(Graphics gfx, Lines lines, Position correction = null, bool export = false)
        {
            bool isvisible; // drawonly visible elements
            decimal s = Tools.GetScale(this.scale);

            // fix position for image file export
            decimal cx = 0;
            decimal cy = 0;
            if (correction != null)
            {
                cx = correction.x;
                cy = correction.y;
            }

            // DRAW lines
            foreach (Line lin in lines) // Loop through List with foreach
            {

                Node r1 = lin.startNode;
                Node r2 = lin.endNode;


                float r1x = (float)((this.shift.x + cx + r1.position.x + (r1.width * Tools.GetScale(r1.scale)) / 2) / s);
                float r1y = (float)((this.shift.y + cy + r1.position.y + (r1.height * Tools.GetScale(r1.scale)) / 2) / s);

                float r2x = (float)((this.shift.x + cx + r2.position.x + (r2.width * Tools.GetScale(r2.scale)) / 2) / s);
                float r2y = (float)((this.shift.y + cy + r2.position.y + (r2.height * Tools.GetScale(r2.scale)) / 2) / s);


                isvisible = false;
                if (export)
                {
                    isvisible = true;
                }
                else
                if ((r1.scale < this.scale - 16 || this.scale + 16 < r1.scale) && (r2.scale < this.scale - 16 || this.scale + 16 < r2.scale)) // remove to small or to big objects
                {
                    isvisible = false;
                }
                else
                if (0 + this.ClientSize.Width <= r1x && 0 + this.ClientSize.Width <= r2x)
                {
                    isvisible = false;
                }
                else
                if (r1x <= 0 && r2x <= 0)
                {
                    isvisible = false;
                }
                else
                if (0 + this.ClientSize.Height <= r1y && 0 + this.ClientSize.Height <= r2y)
                {
                    isvisible = false;
                }
                else
                if (r1y <= 0 && r2y <= 0)
                {
                    isvisible = false;
                }
                else
                {
                    isvisible = true;
                }


                if (isvisible)
                {

                    if (lin.arrow) // draw line as arrow
                    {
                        decimal x1 = (this.shift.x + cx + r1.position.x + (r1.width * Tools.GetScale(r1.scale)) / 2) / s;
                        decimal y1 = (this.shift.y + cy + r1.position.y + (r1.height * Tools.GetScale(r1.scale)) / 2) / s;
                        decimal x2 = (this.shift.x + cx + r2.position.x + (r2.width * Tools.GetScale(r2.scale)) / 2) / s;
                        decimal y2 = (this.shift.y + cy + r2.position.y + (r2.height * Tools.GetScale(r2.scale)) / 2) / s;
                        decimal nx1 = ((decimal)Math.Cos(Math.PI / 2) * (x2 - x1) - (decimal)Math.Sin(Math.PI / 2) * (y2 - y1) + x1);
                        decimal ny1 = ((decimal)Math.Sin(Math.PI / 2) * (x2 - x1) + (decimal)Math.Cos(Math.PI / 2) * (y2 - y1) + y1);
                        decimal nx2 = ((decimal)Math.Cos(-Math.PI / 2) * (x2 - x1) - (decimal)Math.Sin(-Math.PI / 2) * (y2 - y1) + x1);
                        decimal ny2 = ((decimal)Math.Sin(-Math.PI / 2) * (x2 - x1) + (decimal)Math.Cos(-Math.PI / 2) * (y2 - y1) + y1);
                        decimal size = (decimal)Math.Sqrt((double)((nx1 - x1) * (nx1 - x1) + (ny1 - y1) * (ny1 - y1)));
                        nx1 = x1 + (((nx1 - x1) / size) * (7 * Tools.GetScale(r1.scale))) / s;
                        ny1 = y1 + (((ny1 - y1) / size) * (7 * Tools.GetScale(r1.scale))) / s;
                        nx2 = x1 + (((nx2 - x1) / size) * (7 * Tools.GetScale(r1.scale))) / s;
                        ny2 = y1 + (((ny2 - y1) / size) * (7 * Tools.GetScale(r1.scale))) / s;

                        // Create points that define polygon.
                        Point point1 = new Point((int)nx1, (int)ny1);
                        Point point2 = new Point((int)nx2, (int)ny2);
                        Point point3 = new Point((int)x2, (int)y2);
                        PointF[] curvePoints = { point1, point2, point3 };

                        // Define fill mode.
                        FillMode newFillMode = FillMode.Winding;

                        // Fill polygon to screen.
                        gfx.FillPolygon(
                            new SolidBrush(lin.color.color),
                            curvePoints,
                            newFillMode
                        );
                    }
                    else
                    {
                        int linewidth = (int)lin.width;


                        if (linewidth > 1)
                        {
                            linewidth = lin.width * Tools.GetScale(lin.scale) / s > 1 ? (int)(lin.width * Tools.GetScale(lin.scale) / s) : 1;

                            if (linewidth > 100)
                            {
                                linewidth = 100;
                            }
                        }

                        PointF[] points =
                         {
                             new PointF(r1x, r1y),
                             new PointF(r2x, r2y)
                         };

                        // draw line
                        gfx.DrawLines(
                            new Pen(lin.color.color, linewidth),
                            points
                        );
                    }

                }

            }
        }

        /*************************************************************************************************************************/

        // DIAGRAM Set model
        public void SetDiagram(Diagram diagram)
        {
            this.diagram = diagram;
        }

        // DIAGRAM Get model
        public Diagram GetDiagram()
        {
            return this.diagram;
        }

        /*************************************************************************************************************************/

        // VIEW REFRESH UID0421401402
        private void DiagramView_Activated(object sender, EventArgs e)
        {
            /*if (this.diagram.isLocked())
            {
                this.diagram.unlockDiagram();
            }*/

            this.Invalidate();

            if (this.windowIsPinned) {
                this.windowIsPinned = false;
                this.windowPinBox.Visible = false;
                this.FormBorderStyle = this.windowBorderStyleBeforePin;
                this.Width = this.windowHeightBeforePin;
                this.Height = this.windowHeightBeforePin;
                this.TopMost = this.diagram.options.alwaysOnTop;
            }
        }

        // VIEW page up
        public void PageUp()
        {
            this.shift.y += this.ClientSize.Height;
            this.diagram.InvalidateDiagram();
        }

        // VIEW page down
        public void PageDown()
        {
            this.shift.y -= this.ClientSize.Height;
            this.diagram.InvalidateDiagram();
        }

        // VIEW reset zoom
        public void ResetZoom()
        {
            this.currentScale = this.zoomingDefaultScale;
            this.scale = this.zoomingDefaultScale;
            this.diagram.InvalidateDiagram();
        }

        // VIEW get mouse position
        public Position GetMousePosition()
        {
            Point ptCursor = Cursor.Position;
            ptCursor = this.PointToClient(ptCursor);
            return new Position(ptCursor.X, ptCursor.Y);
        }

        // VIEW full screen
        private void FullScreenSwitch()
        {
            if (this.isFullScreen)
            {
                this.isFullScreen = false;
                WindowState = FormWindowState.Normal;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

            }
            else
            {
                this.isFullScreen = true;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }


        }

        // VIEW full screen
        public void LockView() //UID1708360605
        {
            if (editPanel != null && editPanel.Visible)
            {
                editPanel.ClosePanel();
            }

            if (editLinkPanel != null && editLinkPanel.Visible)
            {
                editLinkPanel.ClosePanel();
            }
        }

        // VIEW Convert mouse position to diagram coordinates 
        public Position MouseToDiagramPosition(Position mousePosition)
        {
            return mousePosition.Clone().Scale(Tools.GetScale(this.scale)).Subtract(this.shift);
        }

        /*************************************************************************************************************************/

        // NODE create
        public Node CreateNode(Position position, bool SelectAfterCreate = true)
        {
            var rec = this.diagram.CreateNode(
                position.Clone().Scale(Tools.GetScale(this.scale)).Subtract(this.shift),
                "",
                this.currentLayer.id
            );

            if (rec != null)
            {
                rec.scale = this.scale;
                if (SelectAfterCreate) this.SelectOnlyOneNode(rec);
            }

            return rec;
        }

        // NODE create after
        public void AddNodeAfterNode()
        {
            if (this.selectedNodes.Count() == 1)
            {
                if (!this.editPanel.Visible)
                {
                    int spaceBetweenNodes = 10;

                    Node selectedNode = this.selectedNodes[0];

                    decimal s = Tools.GetScale(this.scale);

                    float x = (float)((this.shift.x + selectedNode.position.x + (Node.NodePadding * Tools.GetScale(selectedNode.scale))) / s);
                    float y = (float)((this.shift.y + selectedNode.position.y) / s);
                    float w = (float)((selectedNode.width - Node.NodePadding) / (s / Tools.GetScale(selectedNode.scale)));

                    Position newNodePosition = new Position(x, y);

                    this.editPanel.prevSelectedNode = selectedNode;
                    this.editPanel.ShowEditPanel(
                        newNodePosition.Clone().Add(w + spaceBetweenNodes, 0),
                        ' ',
                        false
                    );
                }
            }
        }

        // NODE create below
        public void AddNodeBelowNode()
        {
            if (this.selectedNodes.Count() == 1)
            {
                if (!this.editPanel.Visible)
                {
                    this.editPanel.prevSelectedNode = this.selectedNodes[0];
                    this.editPanel.ShowEditPanel(
                        this.selectedNodes[0]
                            .position
                            .Clone()
                            .Add(this.shift)
                            .Add(0, this.selectedNodes[0].height + 10),
                        ' ',
                        false
                    );
                }
            }
        }

        // NODE open link directory
        public void OpenLinkDirectory()
        {
            if (this.selectedNodes.Count() > 0)
            {
                foreach (Node node in this.selectedNodes)
                {
                    if (node.link.Trim().Length > 0)
                    {
                        if (Os.DirectoryExists(node.link)) // open directory of selected nods
                        {
                            Os.ShowDirectoryInExternalApplication(node.link);
                        }
                        else
                        if (Os.FileExists(node.link)) // open directory of selected files
                        {
                            string parent_diectory = Os.GetFileDirectory(this.selectedNodes[0].link);
                            Os.ShowDirectoryInExternalApplication(parent_diectory);
                        }
                    }
                }
            }
            else
            {
                this.diagram.OpenDiagramDirectory(); // open main directory of diagram
            }
        }

        // NODES DELETE SELECTION UID6677508921
        public void DeleteSelectedNodes(DiagramView DiagramView)
        {
            if (!this.diagram.IsReadOnly())
            {
                if (DiagramView.selectedNodes.Count() > 0)
                {
                    this.diagram.DeleteNodes(DiagramView.selectedNodes, this.shift, this.currentLayer.id);
                    this.ClearSelection();
                }
            }
        }

        // NODE Go to node position UID0896814291
        public void GoToNode(Node rec)
        {
            if (rec != null)
            {
                this.scale = rec.scale;
                this.GoToLayer(rec.layer);
                this.GoToPosition(rec.position);
            }
        }

        // NODE Go to position
        public void GoToPosition(Position position)
        {
            if (position != null)
            {
                Position temp = new Position(this.ClientSize.Width, this.ClientSize.Height);
                temp.Split(2).Scale(Tools.GetScale(this.scale)).Subtract(position);
                this.shift.Set(temp);
            }
        }

        // NODE Go to position
        public void GoToPosition(Position shift = null, decimal scale = 0, long layerid = 0)
        {
            if (shift != null)
            {
                this.GoToLayer(layerid);
                this.shift.Set(shift);
                this.scale = scale;
            }
        }

        // NODE Check to shift
        public bool IsOnPosition(Position shift, decimal scale, long layer)
        {
            if (shift.x == this.shift.x && shift.y == this.shift.y && this.scale == scale && this.currentLayer.id == layer)
            {
                return true;
            }

            return false;
        }

        // NODE Go to shift
        public void GoToShift(Position shift)
        {
            if (shift != null)
            {
                this.shift.Set(shift);
            }
        }

        // NODE Go to node layer UID5640777236
        public void GoToLayer(long layer = 0)
        {
            Layer l = this.diagram.layers.GetLayer(layer);
            if (l != null)
            {
                this.currentLayer = l;
                this.BuildLayerHistory(layer);
            }
        }

        // NODE find node in mouse cursor position
        public Node FindNodeInMousePosition(Position mousePosition, Node skipNode = null)
        {
            Nodes nodes = new Nodes();

            decimal scale;

            Position position = this.MouseToDiagramPosition(mousePosition);
            long layer = this.currentLayer.id;

            foreach (Node node in this.diagram.layers.GetLayer(layer).nodes.Reverse<Node>()) // Loop through List with foreach
            {
                if (layer == node.layer || layer == node.id)
                {
                    if (skipNode == null || skipNode.id != node.id)
                    {
                        if (this.NodeIsVisible(node))
                        {
                            scale = Tools.GetScale(node.scale);
                            if (node.name == "" && node.transparent)
                            {

                                decimal sx = node.position.x + (node.width / scale) / 2;
                                decimal sy = node.position.y + (node.height / scale) / 2;

                                decimal viewScale = Tools.GetScale(this.scale);

                                if (sx - (node.width / 2 * viewScale) / 2 <= position.x && position.x <= sx + (node.width / 2 * viewScale) &&
                                    sy - (node.height / 2 * viewScale) / 2 <= position.y && position.y <= sy + (node.height / 2 * viewScale))
                                {
                                    return node;
                                }
                            }
                            else if
                            (
                                node.position.x <= position.x && position.x <= node.position.x + (node.width * scale) &&
                                node.position.y <= position.y && position.y <= node.position.y + (node.height * scale)
                            )
                            {
                                return node;
                            }
                        }
                    }
                }
            }

            return null;
        }

        // NODE Open Link
        public void OpenLink(Node rec) //UID9292140736
        {
            if (rec == null) // prevent execution of scripts when curent user is not owner of document
            {
                return;
            }

            if (rec.haslayer)
            {
                if (this.diagram.options.openLayerInNewView) //UID1964118363
                {
                    this.diagram.OpenDiagramView(
                        this,
                        this.diagram.layers.GetLayer(
                            rec.id
                        )
                    );

                    return;
                }
                else
                {
                    this.LayerIn(rec);
                    return;
                }
            }
            else if (rec.shortcut > 0) // GO TO LINK
            {
                Node target = this.diagram.GetNodeByID(rec.shortcut);
                this.GoToNode(target);
                this.diagram.InvalidateDiagram();
                return;
            }
            else if (Patterns.IsGotoIdCommand(rec.link)) // GOTO position
            {
                Program.log.Write("go to position in diagram " + rec.link);
                Node node = this.diagram.GetNodeByID(Patterns.GetGotoIdCommand(rec.link));
                if (node != null)
                {
                    this.GoToNodeWithAnimation(node);
                }
                return;
            }
            else if (Patterns.IsGotoStringCommand(rec.link)) // GOTO position
            {
                Program.log.Write("go to position in diagram " + rec.link);
                String searchFor = Patterns.GetGotoStringCommand(rec.link);
                Nodes nodes = this.diagram.layers.SearchInAllNodes(searchFor);
                if (nodes.Count() >= 2)
                {
                    if (rec != nodes[0])
                    {
                        this.GoToNodeWithAnimation(nodes[0]);
                    }
                    else
                    {
                        this.GoToNodeWithAnimation(nodes[1]);
                    }
                }

                return;
            }


            if (this.diagram.isSigned())
            {
                bool stopNextAction = this.main.plugins.ClickOnNodeAction(this.diagram, this, rec); //UID0290845815

                if (stopNextAction)
                {
                    // stop execution from plugin
                    return;
                }

                if (rec.attachment != "")
                {
                    this.SelectOnlyOneNode(rec); // deploy attachment
                    this.AttachmentDeploy();
                    return;
                }
                else
                if (rec.link.Length > 0)
                {

                    string fileName = "";
                    string searchString = "";

                    if (Patterns.IsEmail(rec.link)) // OPEN EMAIL
                    {
                        Os.OpenEmail(rec.link);

                        return;
                    }
                    else if (Patterns.IsURL(rec.link)) // OPEN URL
                    {
                        try
                        {
                            Program.log.Write("diagram: openlink: open url " + rec.link);
                            Network.OpenUrl(rec.link);
                        }
                        catch (Exception ex)
                        {
                            Program.log.Write("open link as url error: " + ex.Message);
                        }

                        return;
                    }
                    else if (Os.DirectoryExists(rec.link))  // OPEN DIRECTORY
                    {
                        try
                        {
                            Program.log.Write("diagram: openlink: open directory " + Os.NormalizePath(rec.link));
                            Os.OpenDirectory(rec.link);
                        }
                        catch (Exception ex)
                        {
                            Program.log.Write("open link as directory error: " + ex.Message);
                        }

                        return;
                    }
                    else if (Os.FileExists(rec.link))       // OPEN FILE
                    {
                        try
                        {
                            if (Os.IsDiagram(rec.link))
                            {
                                Program.log.Write("diagram: openlink: open diagram " + Os.NormalizePath(rec.link));
                                this.main.OpenDiagram(rec.link);
                            }
                            else
                            {
                                Program.log.Write("diagram: openlink: open file " + Os.NormalizePath(rec.link));
                                Os.OpenFileInExplorer(rec.link);
                            }
                        }
                        catch (Exception ex)
                        {
                            Program.log.Write("open link as file error: " + ex.Message);
                        }

                        return;
                    }
                    else if (Patterns.HasHastag(rec.link.Trim(), ref fileName, ref searchString)
                        && Os.FileExists(Os.NormalizedFullPath(fileName)))       // OPEN FILE ON LINE POSITION
                    {
                        try
                        {
                            // if is not set search strin, only hastag after link then use node name as string for search
                            if (searchString.Trim() == "")
                            {
                                searchString = rec.name;
                            }

                            // if search string is not number then search for first line with search string
                            if (!Patterns.IsNumber(searchString))
                            {
                                searchString = Os.FndLineNumber(fileName, searchString).ToString();
                            }

                            // get external editor path from global configuration saved in user configuration directory
                            String editFileCmd = this.main.programOptions.texteditor;
                            editFileCmd = editFileCmd.Replace("%FILENAME%", Os.NormalizedFullPath(fileName));
                            editFileCmd = editFileCmd.Replace("%LINE%", searchString);

                            Program.log.Write("diagram: openlink: open file on position " + editFileCmd);
                            Os.RunSilentCommand(editFileCmd);
                        }
                        catch (Exception ex)
                        {
                            Program.log.Write("open link as file error: " + ex.Message);
                        }

                        return;
                    }
                    else // run as command 
                    {

                        // set current directory to current diagrm file destination
                        if (Os.FileExists(this.diagram.FileName))
                        {
                            Os.SetCurrentDirectory(Os.GetFileDirectory(this.diagram.FileName));
                        }

                        /*
                        - stamps in command
                            %TEXT%     - name of node
                            %NAME%     - name of node
                            %LINK%     - link in node
                            %NOTE%     - note in node
                            %ID%       - id of actual node
                            %FILENAME% - file name  of diagram
                            %DIRECTORY% - current diagram directory
                        */

                        // replace stamps in link
                        string cmd = rec.link
                        .Replace("%TEXT%", rec.name)
                        .Replace("%NAME%", rec.name)
                        .Replace("%LINK%", rec.link)
                        .Replace("%NOTE%", rec.note)
                        .Replace("%ID%", rec.id.ToString())
                        .Replace("%FILENAME%", this.diagram.FileName)
                        .Replace("%DIRECTORY%", Os.GetFileDirectory(this.diagram.FileName));

                        Program.log.Write("diagram: openlink: run command: " + cmd);


                        Task.Run(async () =>
                        {
                            await Os.RunCommandWithTimeout(cmd, Os.GetFileDirectory(this.diagram.FileName));
                        });

                        //Os.RunCommand(cmd, Os.GetFileDirectory(this.diagram.FileName)); // RUN COMMAND UID5087096741

                        return;
                    }
                }
            }

            this.diagram.EditNode(rec);
            return;
        }

        // NODE Remove shortcuts
        public void RemoveShortcuts(Nodes Nodes)
        {
            foreach (Node rec in Nodes) // Loop through List with foreach
            {
                this.diagram.RemoveShortcut(rec);
            }

            this.diagram.InvalidateDiagram();
        }

        // NODE Select node color
        public void SelectColor()
        {
            if (selectedNodes.Count() > 0 && !this.diagram.IsReadOnly())
            {
                // show color picker and move it to vsible area of screen
                var screen = Screen.FromPoint(Cursor.Position);
                colorPickerForm.StartPosition = FormStartPosition.Manual;
                colorPickerForm.Left = Cursor.Position.X - 50;
                colorPickerForm.Top = Cursor.Position.Y - 50;

                if (Cursor.Position.X - 50 < 0)
                {
                    colorPickerForm.Left = 0;
                }

                if (Cursor.Position.Y - 50 < 0)
                {
                    colorPickerForm.Top = 0;
                }

                if (Cursor.Position.X - 50 + colorPickerForm.Width > screen.Bounds.Width)
                {
                    colorPickerForm.Left = screen.Bounds.Width - colorPickerForm.Width;
                }

                if (Cursor.Position.Y - 50 + colorPickerForm.Height > screen.Bounds.Height)
                {
                    colorPickerForm.Top = screen.Bounds.Height - colorPickerForm.Height;
                }

                colorPickerForm.ShowDialog();
            }
        }

        public void ChangeColor(ColorType color)
        {
            if (!this.diagram.IsReadOnly())
            {
                if (selectedNodes.Count() > 0)
                {
                    if (!this.diagram.undoOperations.IsSame("changeNodeColor", this.selectedNodes, null))
                    {
                        this.diagram.Unsave("changeNodeColor", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                    }

                    foreach (Node rec in this.selectedNodes)
                    {
                        rec.color.Set(color);
                    }

                    this.Invalidate();
                }
            }
        }

        // NODE Select node font color
        public void SelectFontColor()
        {
            if (selectedNodes.Count() > 0 && !this.diagram.IsReadOnly())
            {
                DFontColor.Color = this.selectedNodes[0].color.color;

                if (DColor.ShowDialog() == DialogResult.OK)
                {
                    if (!this.diagram.IsReadOnly())
                    {
                        if (selectedNodes.Count() > 0)
                        {
                            foreach (Node rec in this.selectedNodes)
                            {
                                rec.fontcolor.Set(DColor.Color);
                            }
                        }
                    }

                    this.diagram.InvalidateDiagram();
                }
            }
        }

        // NODE Select node font
        public void SelectFont()
        {
            if (selectedNodes.Count() > 0 && !this.diagram.IsReadOnly())
            {
                DFont.Font = this.selectedNodes[0].font;

                if (DFont.ShowDialog() == DialogResult.OK)
                {
                    if (!this.diagram.IsReadOnly())
                    {
                        if (selectedNodes.Count() > 0)
                        {
                            foreach (Node rec in this.selectedNodes)
                            {
                                rec.font = DFont.Font;
                                rec.Resize();
                            }
                        }
                    }

                    this.diagram.InvalidateDiagram();
                }
            }
        }

        // NODE Check if selected nodes are transparent
        public bool IsSelectionTransparent()
        {
            bool isTransparent = false;

            if (selectedNodes.Count() > 0)
            {
                foreach (Node rec in this.selectedNodes)
                {
                    if (rec.transparent)
                    {
                        isTransparent = true;
                        break;
                    }
                }
            }

            return isTransparent;
        }

        // NODE Make selected node transparent
        public void MakeSelectionTransparent()
        {
            if (selectedNodes.Count() > 0 && !this.diagram.IsReadOnly())
            {
                bool isTransparent = this.IsSelectionTransparent();

                foreach (Node rec in this.selectedNodes)
                {
                    rec.transparent = !isTransparent;
                }

                this.diagram.InvalidateDiagram();

            }
        }

        // NODE Select node default font
        public void SelectDefaultFont()
        {
            this.defaultfontDialog.Font = this.diagram.FontDefault;
            if (this.defaultfontDialog.ShowDialog() == DialogResult.OK)
            {
                if (!this.diagram.IsReadOnly())
                {
                    this.diagram.FontDefault = this.defaultfontDialog.Font;
                }
            }
        }

        // NODE Select node image
        public void AddImage()
        {
            if (selectedNodes.Count() > 0 && !this.diagram.IsReadOnly())
            {
                if (this.DImage.ShowDialog() == DialogResult.OK && Os.FileExists(this.DImage.FileName))
                {
                    this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);

                    foreach (Node rec in this.selectedNodes)
                    {
                        this.diagram.SetImage(rec, Os.GetFullPath(this.DImage.FileName));
                    }

                    this.diagram.Unsave();
                }

                this.diagram.InvalidateDiagram();
            }
            else
            {
                if (this.DImage.ShowDialog() == DialogResult.OK && Os.FileExists(this.DImage.FileName))
                {

                    Node newrec = this.CreateNode(this.startMousePos);
                    this.diagram.SetImage(newrec, this.DImage.FileName);
                    this.diagram.Unsave("create", newrec, this.shift, this.scale, this.currentLayer.id);
                }

                this.diagram.InvalidateDiagram();
            }
        }

        // NODE Select node image
        public void RemoveImagesFromSelection()
        {
            if (selectedNodes.Count() > 0 && !this.diagram.IsReadOnly())
            {
                this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                foreach (Node rec in this.selectedNodes)
                {
                    if (rec.isimage)
                    {
                        this.diagram.RemoveImage(rec);
                    }
                }

                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE Check if selected nodes are transparent
        public bool HasSelectionImage()
        {
            bool hasImage = false;

            if (selectedNodes.Count() > 0)
            {
                foreach (Node rec in this.selectedNodes)
                {
                    if (rec.isimage)
                    {
                        hasImage = true;
                        break;
                    }
                }
            }

            return hasImage;
        }

        // NODE Check if selected nodes are transparent
        public bool HasSelectionNotEmbeddedImage()
        {
            bool hasImage = false;

            if (selectedNodes.Count() > 0)
            {
                foreach (Node rec in this.selectedNodes)
                {
                    if (rec.isimage && !rec.embeddedimage)
                    {
                        hasImage = true;
                        break;
                    }
                }
            }

            return hasImage;
        }

        // NODE Make selected node transparent
        public void MakeImagesEmbedded()
        {
            if (selectedNodes.Count() > 0 && !this.diagram.IsReadOnly())
            {
                bool hasImage = this.HasSelectionNotEmbeddedImage();

                if (hasImage)
                {
                    this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);

                    foreach (Node rec in this.selectedNodes)
                    {
                        this.diagram.SetImageEmbedded(rec);
                    }

                    this.diagram.Unsave();
                }

                this.diagram.InvalidateDiagram();
            }
        }

        // NODE copy UID4434132203
        public bool Copy()
        {
            if (this.selectedNodes.Count() > 0)
            {
                DataObject data = new DataObject();

                string copytext = "";
                foreach (Node rec in this.selectedNodes)
                {
                    copytext += rec.name;

                    if (this.selectedNodes.Count() > 1)
                    {
                        copytext += "\n";
                    }
                }

                data.SetData(copytext);

                data.SetData("DiagramXml", this.diagram.GetDiagramPart(this.selectedNodes));//create and copy xml

                Clipboard.SetDataObject(data);

                return true;
            }

            return false;
        }

        // NODE cut UID4343312404
        public bool Cut()
        {
            if (this.selectedNodes.Count() > 0)  // kopirovanie textu objektu
            {

                this.Copy();
                this.DeleteSelectedNodes(this);
                this.diagram.InvalidateDiagram();
            }

            return true;
        }

        // NODE paste UID3240032142
        public bool Paste(Position position)
        {
            DataObject retrievedData = (DataObject)Clipboard.GetDataObject();

            if (retrievedData.GetDataPresent("DiagramXml"))  // [PASTE] [DIAGRAM] [CLIPBOARD OBJECT] insert diagram
            {
                DiagramBlock newBlock = this.diagram.AddDiagramPart(
                    retrievedData.GetData("DiagramXml") as string,
                    position.Clone().Scale(Tools.GetScale(this.scale)).Subtract(this.shift),
                    this.currentLayer.id,
                    this.scale
                );

                this.diagram.Unsave("create", newBlock.nodes, newBlock.lines, this.shift, this.scale, this.currentLayer.id);

                // filter only top nodes fromm all new created nodes. NewNodes containing sublayer nodes.
                Nodes topNodes = new Nodes();
                foreach (Node node in newBlock.nodes)
                {
                    if (node.layer == this.currentLayer.id)
                    {
                        topNodes.Add(node);
                    }
                }

                this.SelectNodes(topNodes);
                this.diagram.InvalidateDiagram();
            }
            else
            if (retrievedData.GetDataPresent(DataFormats.StringFormat))  // [PASTE] [TEXT] insert text
            {
                Node newrec = this.CreateNode(position);

                string ClipText = retrievedData.GetData(DataFormats.StringFormat) as string;

                if (Patterns.IsColor(ClipText))
                {
                    newrec.SetName(ClipText);
                    newrec.color.Set(Media.GetColor(ClipText));
                    this.diagram.Unsave("create", newrec, this.shift, this.scale, this.currentLayer.id);
                }
                else if (Patterns.IsURL(ClipText))  // [PASTE] [URL] [LINK] paste link from clipboard
                {
                    newrec.link = ClipText;
                    newrec.SetName(ClipText);

                    this.SetNodeNameByLink(newrec, ClipText);

                    this.diagram.Unsave("create", newrec, this.shift, this.scale, this.currentLayer.id);
                }
                else
                {
                    newrec.SetName(ClipText);

                    // set link to node as path to file
                    if (Os.FileExists(ClipText))
                    {
                        newrec.SetName(Os.GetFileName(ClipText));
                        newrec.link = Os.MakeRelative(ClipText, this.diagram.FileName);
                        newrec.color.Set(Media.GetColor(diagram.options.colorFile));
                    }

                    // set link to node as path to directory
                    if (Os.DirectoryExists(ClipText))
                    {
                        newrec.SetName(Os.GetFileName(ClipText));
                        newrec.link = Os.MakeRelative(ClipText, this.diagram.FileName);
                        newrec.color.Set(Media.GetColor(diagram.options.colorDirectory));
                    }

                    this.diagram.Unsave("create", newrec, this.shift, this.scale, this.currentLayer.id);
                }

                this.diagram.InvalidateDiagram();
            }
            else
            if (Clipboard.ContainsFileDropList()) // [FILES] [PASTE] insert files from clipboard
            {
                StringCollection returnList = Clipboard.GetFileDropList();
                Nodes nodes = new Nodes();
                foreach (string file in returnList)
                {
                    Node newrec = this.CreateNode(position);
                    nodes.Add(newrec);
                    newrec.SetName(Os.GetFileNameWithoutExtension(file));

                    string ext = Os.GetExtension(file);

                    if (ext == ".jpg" || ext == ".png" || ext == ".ico" || ext == ".bmp") // paste image file direct to diagram as image instead of link
                    {
                        this.diagram.SetImage(newrec, file);
                    }
                    else
                    if (
                        this.diagram.FileName != ""
                        && Os.FileExists(this.diagram.FileName)
                        && file.IndexOf(Os.GetDirectoryName(this.diagram.FileName)) == 0
                    ) // [PASTE] [FILE]
                    {
                        // make path relative to saved diagram path
                        int start = Os.GetDirectoryName(this.diagram.FileName).Length;
                        int finish = file.Length - start;
                        newrec.link = "." + file.Substring(start, finish);
                    }
                    else
                    if (this.diagram.FileName != "" && Os.DirectoryExists(this.diagram.FileName)) // [PASTE] [DIRECTORY]
                    {
                        // make path relative to saved diagram path
                        int start = Os.GetDirectoryName(this.diagram.FileName).Length;
                        int finish = file.Length - start;
                        newrec.link = "." + file.Substring(start, finish);
                    }
                    else
                    {
                        newrec.link = file;
                    }
                }

                if (nodes.Count() > 0)
                {
                    this.diagram.Unsave("create", nodes, null, this.shift, this.scale, this.currentLayer.id);
                    this.diagram.InvalidateDiagram();
                }
            }
            else if (Clipboard.GetDataObject() != null)  // [PASTE] [IMAGE] [CLIPBOARD OBJECT] paste image
            {
                IDataObject data = Clipboard.GetDataObject();

                if (data.GetDataPresent(DataFormats.Bitmap))
                {
                    // paste image as embedded data direct inside diagram
                    try
                    {
                        Node newrec = this.CreateNode(position);

                        newrec.image = (Bitmap)data.GetData(DataFormats.Bitmap, true);
                        newrec.height = newrec.image.Height;
                        newrec.width = newrec.image.Width;
                        newrec.isimage = true;
                        newrec.embeddedimage = true;

                        this.diagram.Unsave("create", newrec, this.shift, this.scale, this.currentLayer.id);
                        this.diagram.InvalidateDiagram();

                    }
                    catch (Exception e)
                    {
                        Program.log.Write("paste immage error: " + e.Message);
                    }
                }

            }

            return true;
        }

        // NODE paste to note
        public bool PasteToNote()
        {
            DataObject retrievedData = (DataObject)Clipboard.GetDataObject();

            if (retrievedData.GetDataPresent(DataFormats.Text))
            {
                string ClipText = retrievedData.GetData(DataFormats.Text) as string;

                if (this.selectedNodes.Count() == 0)
                {
                    // paste text as new node
                    Node newrec = this.CreateNode(this.GetMousePosition());

                    newrec.note = ClipText;
                    this.diagram.Unsave("create", newrec, this.shift, this.scale, this.currentLayer.id);
                }
                else
                {
                    // append text to all selected nodes
                    this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                    foreach (Node rec in this.selectedNodes)
                    {
                        if (rec.note != "") // append to end of note
                        {
                            rec.note += "\n";
                        }

                        rec.note += ClipText;
                    }

                }
            }

            this.diagram.InvalidateDiagram();
            return true;
        }

        // NODE paste to link
        public bool PasteToLink()
        {
            DataObject retrievedData = (DataObject)Clipboard.GetDataObject();

            if (retrievedData.GetDataPresent(DataFormats.Text))
            {
                string ClipText = retrievedData.GetData(DataFormats.Text) as string;

                if (this.selectedNodes.Count() == 0)
                {
                    Node newrec = this.CreateNode(this.GetMousePosition());

                    newrec.link = ClipText;
                    this.diagram.Unsave("create", newrec, this.shift, this.scale, this.currentLayer.id);
                }
                else
                {
                    this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                    foreach (Node rec in this.selectedNodes)
                    {

                        rec.link += ClipText;
                    }

                }
            }

            this.diagram.InvalidateDiagram();
            return true;
        }

        // NODE align to line
        public void AlignToLine()
        {
            if (this.selectedNodes.Count() > 0)
            {
                this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                this.diagram.AlignToLine(this.selectedNodes);
                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE align to column
        public void AlignToColumn()
        {
            if (this.selectedNodes.Count() > 0)
            {
                this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                this.diagram.AlignToColumn(this.selectedNodes);
                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE align to group
        public void AlignToGroup()
        {
            if (this.selectedNodes.Count() > 0)
            {
                this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                this.diagram.AlignCompact(this.selectedNodes);
                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE align to group and sort
        public void SortNodes()
        {
            if (this.selectedNodes.Count() > 0)
            {
                this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                this.diagram.SortNodes(this.selectedNodes);
                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE split node by line and grou
        public void SplitNode()
        {
            if (this.selectedNodes.Count() > 0)
            {
                Nodes newNodes = this.diagram.SplitNode(this.selectedNodes);
                this.diagram.Unsave("create", newNodes, null, this.shift, this.scale, this.currentLayer.id);
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE align to group
        public void AlignToLineGroup()
        {
            if (this.selectedNodes.Count() > 0)
            {
                this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                this.diagram.AlignCompactLine(this.selectedNodes);
                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE align left
        public void AlignLeft()
        {
            if (this.selectedNodes.Count() > 1)
            {
                this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                this.diagram.AlignLeft(this.selectedNodes);
                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE align right
        public void AlignRight()
        {
            if (this.selectedNodes.Count() > 1)
            {
                this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                this.diagram.AlignRight(this.selectedNodes);
                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
            else
            {
                this.AddNodeBelowNode();
            }
        }

        // NODE copy node link
        public bool CopyLink()
        {
            if (this.selectedNodes.Count() > 0)
            {
                string copytext = "";
                foreach (Node rec in this.selectedNodes)
                {
                    if (rec.link != null)
                    {
                        copytext += rec.link;

                        if (this.selectedNodes.Count() > 1)
                        { //separate nodes
                            copytext += "\n";
                        }
                    }
                }

                if (copytext != "")
                {
                    Clipboard.SetText(copytext);
                }
            }

            return true;
        }

        // NODE copy note
        public bool CopyNote()
        {
            if (this.selectedNodes.Count() > 0)
            {
                string copytext = "";
                foreach (Node rec in this.selectedNodes)
                {
                    if (rec.note != null)
                    {
                        copytext += rec.note;

                        if (this.selectedNodes.Count() > 1)
                        { //separate nodes
                            copytext += "\n";
                        }
                    }
                }

                if (copytext != "")
                {
                    Clipboard.SetText(copytext);
                }
            }

            return true;
        }

        // NODE evaluate date
        public bool EvaluateDate()
        {
            bool insertdate = true;
            string insertdatestring = "";

            if (this.selectedNodes.Count() > 0)
            {
                bool aretimes = true;
                foreach (Node rec in this.selectedNodes) // Loop through List with foreach
                {
                    if (!Regex.Match(rec.name, @"^[0-9]{2}:[0-9]{2}:[0-9]{2}$", RegexOptions.IgnoreCase).Success)
                    {
                        aretimes = false;
                        break;
                    }
                }

                if (aretimes) // sum times
                {
                    try
                    {
                        TimeSpan timesum = TimeSpan.Parse("00:00:00");
                        foreach (Node rec in this.selectedNodes)
                        {
                            timesum = timesum.Add(TimeSpan.Parse(rec.name));
                            insertdate = false;
                        }
                        insertdatestring = timesum.ToString();
                        insertdate = false;
                    }
                    catch (Exception ex)
                    {
                        Program.log.Write("time span error: " + ex.Message);
                    }
                }
                else if (this.selectedNodes.Count() == 2)  // count difference between two dates
                {
                    try
                    {
                        DateTime d1 = Converter.ToDateAndTime(this.selectedNodes[0].name);
                        DateTime d2 = Converter.ToDateAndTime(this.selectedNodes[1].name);
                        insertdatestring = ((d1 < d2) ? d2 - d1 : d1 - d2).ToString();
                        insertdate = false;
                    }
                    catch (Exception ex)
                    {
                        Program.log.Write("time diff error: " + ex.Message);
                    }
                }
            }

            if (insertdate) // insert date
            {
                DateTime dt = DateTime.Now;
                insertdatestring =
                    dt.Year + "-" +
                    ((dt.Month < 10) ? "0" : "") + dt.Month + "-" +
                    ((dt.Day < 10) ? "0" : "") + dt.Day + " " +
                    ((dt.Hour < 10) ? "0" : "") + dt.Hour + ":" +
                    ((dt.Minute < 10) ? "0" : "") + dt.Minute + ":" +
                    ((dt.Second < 10) ? "0" : "") + dt.Second;
            }

            Node newrec = this.CreateNode(this.GetMousePosition());
            newrec.SetName(insertdatestring);

            this.diagram.Unsave("create", newrec, this.shift, this.scale, this.currentLayer.id);
            this.diagram.InvalidateDiagram();
            return true;
        }

        // NODE promote
        public bool Promote()
        {
            if (this.selectedNodes.Count() == 1)
            {
                Node selectedNode = this.selectedNodes[0];
                Node newrec = this.CreateNode(this.GetMousePosition());
                newrec.CopyNode(selectedNode, true, true);

                string expression = newrec.name;
                string[] days = { "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" };
                int dayPosition = Array.IndexOf(days, newrec.name);

                var matchesFloat = Regex.Matches(expression, @"(\d+(?:\.\d+)?)");
                var matchesDate = Regex.Matches(expression, @"^(\d{4}-\d{2}-\d{2})$");

                if (dayPosition != -1)
                { //get next day
                    dayPosition += 1;
                    if (dayPosition == 7)
                    {
                        dayPosition = 0;
                    }

                    newrec.SetName(days[dayPosition]);
                }
                else if (matchesDate.Count > 0) // add day to date
                {
                    DateTime theDate = Converter.ToDate(expression);
                    theDate = theDate.AddDays(1);
                    string newnDateValue = Converter.DateToString(theDate);
                    newrec.SetName(newnDateValue);
                }
                else if (matchesFloat.Count > 0) //add to number
                {
                    string number = matchesFloat[0].Groups[1].Value;
                    string newnumber = (double.Parse(number) + 1).ToString();
                    newrec.SetName(expression.Replace(number, newnumber));
                }

                this.diagram.InvalidateDiagram();
                return true;

            }
            return true;
        }

        // NODE hide background
        public bool HideBackground()
        {
            if (this.selectedNodes.Count > 0)
            {
                this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);

                //first all hide then show
                bool someHasImmages = false;
                foreach (Node rec in this.selectedNodes)
                {
                    if (rec.isimage)
                    {
                        someHasImmages = true;
                        break;
                    }
                }

                //first all hide then show
                bool allHidden = true;
                foreach (Node rec in this.selectedNodes)
                {
                    if (!rec.transparent)
                    {
                        allHidden = false;
                        break;
                    }
                }

                bool someEmptyUnHidden = false;
                foreach (Node rec in this.selectedNodes)
                {
                    if (!rec.transparent && rec.name == "")
                    {
                        someEmptyUnHidden = true;
                        break;
                    }
                }

                if (someHasImmages)
                {
                    foreach (Node rec in this.selectedNodes)
                    {
                        if (rec.isimage)
                        {
                            this.diagram.RemoveImage(rec);
                        }
                    }
                }
                else if (someEmptyUnHidden)
                {
                    foreach (Node rec in this.selectedNodes)
                    {

                        if (!rec.transparent && rec.name == "")
                        {
                            rec.transparent = true;
                        }
                    }
                }
                else
                {
                    foreach (Node rec in this.selectedNodes)
                    {

                        if (allHidden)
                        {
                            rec.transparent = true;
                        }
                        else
                        {
                            rec.transparent = false;
                        }

                        rec.transparent = !rec.transparent;
                    }
                }

                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }

            return true;
        }

        // NODE open edit form
        public void Edit()
        {
            if (this.selectedNodes.Count() == 1)
            {
                Node rec = this.selectedNodes[0];
                this.diagram.EditNode(rec);
            }
        }

        // NODE rename
        public void Rename() //UID1498635893
        {
            if (this.selectedNodes.Count() == 1)
            {
                Node rec = this.selectedNodes[0];
                Position position = new Position(this.shift).Add(rec.position).Split(Tools.GetScale(this.scale));
                this.editPanel.EditNode(position, this.selectedNodes[0]);
            }
        }

        // NODE edit link
        public void EditLink()
        {
            if (this.selectedNodes.Count() == 1)
            {
                Node rec = this.selectedNodes[0];
                Position position = this.shift.Clone().Add(rec.position).Split(Tools.GetScale(this.scale));
                this.editLinkPanel.EditNode(position, this.selectedNodes[0]);
            }
        }

        // NODE move nodes to foreground
        public void MoveNodesToForeground()
        {
            if (this.selectedNodes.Count() > 0)
            {
                foreach (Node rec in this.selectedNodes)
                {
                    this.diagram.layers.MoveToForeground(rec);
                }
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE move nodes to background
        public void MoveNodesToBackground()
        {
            if (this.selectedNodes.Count() > 0)
            {
                foreach (Node rec in this.selectedNodes)
                {
                    this.diagram.layers.MoveToBackground(rec);
                }
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE protect sesitive data in node name
        public void ProtectNodes()
        {
            if (this.selectedNodes.Count() > 0)
            {
                bool allProtected = true;
                foreach (Node rec in this.selectedNodes)
                {
                    if (rec.protect == false)
                    {
                        allProtected = false;
                        break;
                    }
                }

                foreach (Node rec in this.selectedNodes)
                {
                    rec.SetProtect(!allProtected);
                }

                this.diagram.InvalidateDiagram();
            }
        }

        // NODE move nodes to left
        public void MoveNodesToLeft(bool quick = false)
        {
            if (this.selectedNodes.Count() > 0)
            {
                if (!this.diagram.undoOperations.IsSame("move", this.selectedNodes, null))
                {
                    this.diagram.undoOperations.Add("move", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                }
                long speed = (quick) ? this.diagram.options.keyArrowFastMoveNodeSpeed : this.diagram.options.keyArrowSlowMoveNodeSpeed;
                foreach (Node rec in this.selectedNodes)
                {
                    decimal s = Tools.GetScale(this.scale);
                    rec.position.x -= speed * s;
                }
                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
            else // MOVE SCREEN
            {
                decimal s = Tools.GetScale(this.scale);
                long speed = (quick) ? this.ClientSize.Width : this.diagram.options.keyArrowSlowSpeed;
                this.shift.x += speed * s;
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE move nodes to right
        public void MoveNodesToRight(bool quick = false)
        {
            if (this.selectedNodes.Count() > 0)
            {
                if (!this.diagram.undoOperations.IsSame("move", this.selectedNodes, null))
                {
                    this.diagram.undoOperations.Add("move", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                }
                long speed = (quick) ? this.diagram.options.keyArrowFastMoveNodeSpeed : this.diagram.options.keyArrowSlowMoveNodeSpeed;
                foreach (Node rec in this.selectedNodes)
                {
                    decimal s = Tools.GetScale(this.scale);
                    rec.position.x += speed * s;
                }
                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
            else // MOVE SCREEN
            {
                decimal s = Tools.GetScale(this.scale);
                long speed = (quick) ? this.ClientSize.Width : this.diagram.options.keyArrowSlowSpeed;
                this.shift.x -= speed * s;
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE move nodes to up
        public void MoveNodesUp(bool quick = false)
        {
            if (this.selectedNodes.Count() > 0)
            {
                if (!this.diagram.undoOperations.IsSame("move", this.selectedNodes, null))
                {
                    this.diagram.undoOperations.Add("move", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                }
                long speed = (quick) ? this.diagram.options.keyArrowFastMoveNodeSpeed : this.diagram.options.keyArrowSlowMoveNodeSpeed;
                foreach (Node rec in this.selectedNodes)
                {
                    decimal s = Tools.GetScale(this.scale);
                    rec.position.y -= speed * s;
                }
                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
            else // MOVE SCREEN
            {
                decimal s = Tools.GetScale(this.scale);
                long speed = (quick) ? this.ClientSize.Height : this.diagram.options.keyArrowSlowSpeed;
                this.shift.y += speed * s;
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE move nodes to up
        public void MoveNodesDown(bool quick = false)
        {
            if (this.selectedNodes.Count() > 0)
            {
                if (!this.diagram.undoOperations.IsSame("move", this.selectedNodes, null))
                {
                    this.diagram.undoOperations.Add("move", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                }
                long speed = (quick) ? this.diagram.options.keyArrowFastMoveNodeSpeed : this.diagram.options.keyArrowSlowMoveNodeSpeed;
                foreach (Node rec in this.selectedNodes)
                {
                    decimal s = Tools.GetScale(this.scale);
                    rec.position.y += speed * s;
                }
                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
            else // MOVE SCREEN
            {
                decimal s = Tools.GetScale(this.scale);
                long speed = (quick) ? this.ClientSize.Height : this.diagram.options.keyArrowSlowSpeed;
                this.shift.y -= speed * s;
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE node is editet with edit panel
        public bool IsEditing()
        {
            if (this.editPanel.IsEditing()
                || this.editLinkPanel.IsEditing())
            {
                return true;
            }

            return false;
        }

        // NODE node is editet with edit panel
        public void CancelEditing()
        {
            if (this.editPanel.IsEditing())
            {
                this.editPanel.ClosePanel();
            }

            if (this.editLinkPanel.IsEditing())
            {
                this.editPanel.ClosePanel();
            }
        }

        // NODE remove attachment from nodes
        public bool HasSelectionAttachment()
        {
            if (this.selectedNodes.Count() > 0)
            {
                foreach (Node node in this.selectedNodes)
                {
                    if (node.attachment != "")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // NODE save attachment from diagram to system
        public void AttachmentDeploy()
        {
            if (this.HasSelectionAttachment())
            {
                if (this.DSelectDirectoryAttachment.ShowDialog() == DialogResult.OK)
                {
                    foreach (Node node in this.selectedNodes)
                    {
                        if (node.attachment != "")
                        {
                            Compress.DecompressPath(node.attachment, this.DSelectDirectoryAttachment.SelectedPath);
                        }
                    }
                }
            }
        }

        // NODE add file to diagram as attachment
        public void AttachmentAddFile(Position position)
        {
            if (this.DSelectFileAttachment.ShowDialog() == DialogResult.OK)
            {
                string data = Compress.CompressPath(this.DSelectFileAttachment.FileName);

                if (this.selectedNodes.Count() > 0)
                {
                    this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                    foreach (Node node in this.selectedNodes)
                    {
                        node.attachment = data;
                    }
                    this.diagram.Unsave();
                }
                else
                {
                    Node newrec = this.CreateNode(position, true);
                    newrec.attachment = data;
                    newrec.color.Set(diagram.options.colorAttachment);
                    newrec.SetName(Os.GetFileName(this.DSelectFileAttachment.FileName));
                    this.diagram.Unsave("create", newrec, this.shift, this.scale, this.currentLayer.id);
                }

                this.diagram.InvalidateDiagram();
            }
        }

        // NODE add directory to diagram as attachment
        public void AttachmentAddDirectory(Position position)
        {
            if (this.DSelectDirectoryAttachment.ShowDialog() == DialogResult.OK)
            {
                string data = Compress.CompressPath(this.DSelectDirectoryAttachment.SelectedPath);

                if (this.selectedNodes.Count() > 0)
                {
                    this.diagram.undoOperations.Add("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);
                    foreach (Node node in this.selectedNodes)
                    {
                        node.attachment = data;
                    }
                    this.diagram.Unsave();
                }
                else
                {
                    Node newrec = this.CreateNode(position, true);
                    newrec.attachment = data;
                    newrec.color.Set(diagram.options.colorAttachment);
                    newrec.SetName(Os.GetFileName(this.DSelectDirectoryAttachment.SelectedPath));
                    this.diagram.Unsave("create", newrec, this.shift, this.scale, this.currentLayer.id);
                }

                this.diagram.InvalidateDiagram();
            }
        }

        // NODE remove attachment from nodes
        public void AttachmentRemove()
        {
            if (this.selectedNodes.Count() > 0)
            {
                foreach (Node node in this.selectedNodes)
                {
                    node.attachment = "";
                }
            }
        }

        // NODE get lines which are connected with current selected nodes
        public Lines GetSelectedLines()
        {
            Lines SelectedLinesTemp = new Lines();
            long id;

            foreach (Node srec in this.selectedNodes)
            {
                id = srec.id;
                foreach (Line lin in this.diagram.layers.GetAllLines())
                {
                    if (lin.start == id)
                    {
                        SelectedLinesTemp.Add(lin);
                    }
                }
            }

            Lines SelectedLines = new Lines();

            foreach (Line lin in SelectedLinesTemp)
            {
                id = lin.end;
                foreach (Node srec in this.selectedNodes)
                {
                    if (id == srec.id)
                    {
                        SelectedLines.Add(lin);
                    }
                }
            }

            return SelectedLines;
        }

        // NODE MARK nodes for navigation history
        public void SwitchMarkForSelectedNodes()
        {
            if (this.selectedNodes.Count > 0)
            {
                this.diagram.Unsave("edit", this.selectedNodes, null, this.shift, this.scale, this.currentLayer.id);

                bool found = false;
                foreach (Node node in this.selectedNodes)
                {
                    if (node.mark)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    this.UnMarkSelectedNodes();
                }
                else
                {
                    this.MarkSelectedNodes();
                }

                this.diagram.Unsave();
                this.diagram.InvalidateDiagram();
            }
        }

        // NODE MARK nodes for navigation history
        public void MarkSelectedNodes()
        {
            foreach (Node node in this.selectedNodes)
            {
                node.mark = true;
            }
        }

        // NODE MARK unmark nodes for navigation history
        public void UnMarkSelectedNodes()
        {
            foreach (Node node in this.selectedNodes)
            {
                node.mark = false;
            }
        }

        // NODE MARK find next marked node
        public void NextMarkedNode()
        {
            Nodes nodes = this.diagram.GetAllNodes();
            Nodes markedNodes = new Nodes();

            // get all marked nodes
            foreach (Node node in nodes)
            {
                if (node.mark)
                {
                    markedNodes.Add(node);
                }
            }

            // no marked node found
            if (markedNodes.Count == 0)
            {
                return;
            }

            // find last marked node
            if (lastMarkNode == 0)
            {
                lastMarkNode = markedNodes[0].id;
                this.GoToNode(markedNodes[0]);
                this.diagram.InvalidateDiagram();
                return;
            }

            //lastMarkNode position in markedNodes
            bool found = false;
            int i;
            for (i = 0; i < markedNodes.Count; i++)
            {
                if (lastMarkNode == markedNodes[i].id)
                {
                    found = true;
                    break;
                }
            }

            //node is not longer marked then start from beginning
            if (!found)
            {
                lastMarkNode = markedNodes[0].id;
                this.GoToNode(markedNodes[0]);
                this.diagram.InvalidateDiagram();
                return;
            }

            // find next node
            if (i < (markedNodes.Count - 1))
            {
                lastMarkNode = markedNodes[i + 1].id;
                this.GoToNode(markedNodes[i + 1]);
                this.diagram.InvalidateDiagram();
                return;
            }

            // go to fisrt node
            lastMarkNode = markedNodes[0].id;
            this.GoToNode(markedNodes[0]);
            this.diagram.InvalidateDiagram();
        }

        // NODE MARK find prev marked node
        public void PrevMarkedNode()
        {
            Nodes nodes = this.diagram.GetAllNodes();
            Nodes markedNodes = new Nodes();

            // get all marked nodes
            foreach (Node node in nodes)
            {
                if (node.mark)
                {
                    markedNodes.Add(node);
                }
            }

            // no marked node found
            if (markedNodes.Count == 0)
            {
                return;
            }

            // find last marked node
            if (lastMarkNode == 0)
            {
                lastMarkNode = markedNodes[0].id;
                this.GoToNode(markedNodes[0]);
                this.diagram.InvalidateDiagram();
                return;
            }

            //lastMarkNode position in markedNodes
            bool found = false;
            int i;
            for (i = 0; i < markedNodes.Count; i++)
            {
                if (lastMarkNode == markedNodes[i].id)
                {
                    found = true;
                    break;
                }
            }

            //node is not longer marked then start from beginning
            if (!found)
            {
                lastMarkNode = markedNodes[0].id;
                this.GoToNode(markedNodes[0]);
                this.diagram.InvalidateDiagram();
                return;
            }

            // find prev node
            if (0 < i)
            {
                lastMarkNode = markedNodes[i - 1].id;
                this.GoToNode(markedNodes[i - 1]);
                this.diagram.InvalidateDiagram();
                return;
            }

            // go to last node
            lastMarkNode = markedNodes[markedNodes.Count - 1].id;
            this.GoToNode(markedNodes[markedNodes.Count - 1]);
            this.diagram.InvalidateDiagram();
        }

        // NODE LINK NAME get link name from web
        public void SetNodeNameByLink(Node node, string url)
        {
            // get page title async in thread
            Job.DoJob(
                new DoWorkEventHandler(
                    delegate (object o, DoWorkEventArgs args)
                    {
                        node.SetName(
                            Network.GetWebPageTitle(
                                url,
                                this.main.programOptions.proxy_uri,
                                this.main.programOptions.proxy_password,
                                this.main.programOptions.proxy_username
                            )
                        );

                    }
                ),
                new RunWorkerCompletedEventHandler(
                    delegate (object o, RunWorkerCompletedEventArgs args)
                    {
                        if (node.name == null) node.SetName("url");
                        this.diagram.InvalidateDiagram();
                    }
                )
            );
        }

        // NODE VISIBILITY check visibility of node in current view
        public bool NodeIsVisible(Node rec)
        {

            decimal s = Tools.GetScale(this.scale);

            bool isvisible = true; ;

            if (rec.scale < this.scale - 6 || this.scale + 6 < rec.scale) // remove to small or to big objects
            {
                isvisible = false;
            }
            else
            if (0 + this.ClientSize.Width <= (this.shift.x + rec.position.x) / s)
            {
                isvisible = false;
            }
            else
            if ((this.shift.x + rec.position.x + rec.width) / s <= 0)
            {
                isvisible = false;
            }
            else
            if (0 + this.ClientSize.Height <= (this.shift.y + rec.position.y) / s)
            {
                isvisible = false;
            }
            else
            if ((this.shift.y + rec.position.y + rec.height) / s <= 0)
            {
                isvisible = false;
            }

            return isvisible;
        }

        // LINE change color of lines
        public void ChangeLineColor()
        {
            if (!this.diagram.IsReadOnly())
            {
                if (this.selectedNodes.Count() > 0)
                {
                    Lines SelectedLines = GetSelectedLines();

                    DColor.Color = SelectedLines[0].color.color;

                    if (DColor.ShowDialog() == DialogResult.OK)
                    {
                        if (!this.diagram.undoOperations.IsSame("changeLineColor", null, SelectedLines))
                        {
                            this.diagram.Unsave("changeLineColor", null, SelectedLines, this.shift, this.scale, this.currentLayer.id);
                        }

                        foreach (Line lin in SelectedLines)
                        {
                            lin.color.Set(DColor.Color);
                        }

                        this.diagram.InvalidateDiagram();
                    }
                }
            }
        }

        // LINE change line width
        public void ChangeLineWidth()
        {
            if (!this.diagram.IsReadOnly())
            {
                if (this.selectedNodes.Count() > 0)
                {
                    Lines SelectedLines = GetSelectedLines();
                    lineWidthForm.SetValue(SelectedLines[0].width); // set trackbar to first selected line width
                    lineWidthForm.ShowDialog();

                }
            }
        }

        // LINE resize line with event called by line width form
        public void ResizeLineWidth(int width = 1)
        {
            if (!this.diagram.IsReadOnly())
            {
                if (this.selectedNodes.Count() > 0)
                {
                    Lines SelectedLines = GetSelectedLines();

                    if (!this.diagram.undoOperations.IsSame("changeLineWidth", null, SelectedLines))
                    {
                        this.diagram.Unsave("changeLineWidth", null, SelectedLines, this.shift, this.scale, this.currentLayer.id);
                    }

                    foreach (Line lin in SelectedLines)
                    {
                        lin.width = width;
                    }

                    this.diagram.InvalidateDiagram();
                }
            }
        }

        /*************************************************************************************************************************/

        // MOVE TIMER Go to node position UID7284214377
        public void GoToNodeWithAnimation(Node node)
        {
            if (node != null)
            {
                if (node.layer != this.currentLayer.id || (node.scale != this.scale && Math.Abs(node.scale - this.scale) > 2)) // if node is in different layer then move instantly
                {
                    this.GoToNode(node);
                }
                else
                {
                    Position nodePositionOnScreen = node.position.Clone().Add(this.shift).Split(Tools.GetScale(this.scale));

                    Position centerOfViewPosition = new Position(this.ClientRectangle.Width, this.ClientRectangle.Height).Split(2);

                    double distance = nodePositionOnScreen.Distance(centerOfViewPosition);

                    if (distance > 10000)
                    {
                        this.GoToNode(node);
                    }
                    else
                    {

                        this.animationTimerCounter = 30;
                        this.animationTimerStep.Set(nodePositionOnScreen.Clone().Subtract(centerOfViewPosition).Split(30)).Scale(Tools.GetScale(this.scale));
                        this.animationTimer.Enabled = true;

                    }

                }

                this.SelectOnlyOneNode(node);
            }
        }

        // MOVE TIMER Go to node position
        public void AnimationTimer_Tick(object sender, EventArgs e)
        {

#if DEBUG
            this.LogEvent("animationTimer");
#endif
            this.shift.Subtract(this.animationTimerStep);

            if (--this.animationTimerCounter <= 0)
            {
                this.animationTimer.Enabled = false;
            }

            this.diagram.InvalidateDiagram();
        }

        int zoomTmerTime = 0;

        // ZOOM TIMER zoom animation
        public void ZoomTimer_Tick(object sender, EventArgs e)
        {

#if DEBUG
            this.LogEvent("zoomTimer");
#endif
            if (zoomTmerTime-- == 0)
            {
                this.zoomTimer.Enabled = false;
                this.diagram.InvalidateDiagram();
            }

        }

        /*************************************************************************************************************************/
#if DEBUG
        // DEBUG log event to output console and prevent duplicate events display
        public void LogEvent(string lastEvetMessage = "")
        {
            if (this.lastEvent != lastEvetMessage)
            {
                Debug.WriteLine(lastEvetMessage);
                this.lastEvent = lastEvetMessage;
            }
        }
#endif
    }
}
