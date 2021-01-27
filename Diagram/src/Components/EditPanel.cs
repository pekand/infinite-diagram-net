using System;
using System.Drawing;
using System.Windows.Forms;

namespace Diagram
{
    public class EditPanel : Panel //UID2281296902
    {
        public DiagramView diagramView = null;       // diagram ktory je previazany z pohladom

        public bool editing = false; // panel je zobrazený
        public RichTextBox edit = null; // edit pre nove meno nody

        public Node prevSelectedNode = null;
        public Node editedNode = null;

        public EditPanel(DiagramView diagramView)
        {
            this.diagramView = diagramView;

            InitializeComponent();

            this.Hide();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.edit = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();

            //
            // edit
            //
            this.edit.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFB8");
            this.edit.BorderStyle = BorderStyle.None;
            this.edit.Location = new System.Drawing.Point(4, 4);
            this.edit.Name = "edit";
            this.edit.Size = new System.Drawing.Size(100, 13);
            this.edit.TabIndex = 0;
            this.edit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NodeNameEdit_KeyDown);
            this.edit.TextChanged += new EventHandler(NodeNameEdit_TextChanged);
            this.edit.AcceptsTab = true;
            this.edit.Multiline = true;
            this.edit.Left = 12;
            this.edit.Top = 8;
            this.edit.AutoSize = true;
            this.edit.DetectUrls = false;

            //
            // EditPanel
            //
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFB8");
            this.Controls.Add(this.edit);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        // EDITPANEL SHOW add new node
        public void ShowEditPanel(Position currentPosition, char FirstKey = ' ', bool addKey = true)
        {
            if (!this.Visible)
            {
                int padding = Node.NodePadding;

                this.Left = (int)currentPosition.x;
                this.Top = (int)currentPosition.y;

                this.edit.Left = padding + 1;
                this.edit.Top = padding + 0;

                Font defaultFont = this.diagramView.diagram.FontDefault;
                Font font = new Font(defaultFont.FontFamily, defaultFont.Size, defaultFont.Style);

                this.edit.Font = font;

                this.edit.Text = "";
                if (addKey)
                {
                    this.edit.Text += (FirstKey).ToString(); // add first character
                }

                this.SetPanelSize();

                this.BackColor = this.diagramView.diagram.options.nodeColor.Get();
                this.edit.BackColor = this.BackColor;

                this.editing = true;
                this.Show();
                this.edit.Show();
                this.edit.Focus();
                this.edit.SelectionStart = edit.Text.Length;
            }
        }

        // EDITPANEL SHOW edit existing node
        public void EditNode(Position currentPosition, Node editedNode)
        {
            if (!this.Visible)
            {
                int padding = Node.NodePadding;

                this.editedNode = editedNode;
                this.editedNode.visible = false;
                this.diagramView.diagram.InvalidateDiagram();

                this.Left = (int)currentPosition.x; // TODO: scale long to int
                this.Top = (int)currentPosition.y;

                this.edit.Left = padding + 1;
                this.edit.Top = padding + 0;

                Font nodeFont = this.editedNode.font;
                Font font = new Font(nodeFont.FontFamily, nodeFont.Size, nodeFont.Style);

                this.edit.Font = font;
                this.edit.Text = this.editedNode.name; // add first character

                this.SetPanelSize();

                this.BackColor = this.editedNode.color.Get();
                this.edit.BackColor = this.editedNode.color.Get();
                this.editing = true;
                this.Show();
                this.edit.Show();
                this.edit.Focus();
                this.edit.SelectionStart = edit.Text.Length;
            }
        }

        // EDITPANEL SAVE
        public void SaveNodeNamePanel(bool selectNode = true)
        {

            if (this.editedNode == null) {
                this.editedNode = this.diagramView.CreateNode(new Position(this.Left, this.Top));
                this.editedNode.SetName(edit.Text);
                this.diagramView.diagram.undoOperations.Add("create", this.editedNode, this.diagramView.shift, this.diagramView.scale, this.diagramView.currentLayer.id);
                this.diagramView.diagram.Unsave();
            }
            else if (this.editedNode.name != edit.Text)
            {
                this.diagramView.diagram.undoOperations.Add("edit", this.editedNode, this.diagramView.shift, this.diagramView.scale, this.diagramView.currentLayer.id);
                this.editedNode.SetName(edit.Text);
                this.diagramView.diagram.Unsave();
            }

            this.editedNode.visible = true;

            if (this.prevSelectedNode != null)
            {
                this.diagramView.diagram.Connect(this.prevSelectedNode, this.editedNode);
                this.prevSelectedNode = null;
            }

            this.Hide();
            this.editedNode = null;
            editing = false;
            this.diagramView.diagram.InvalidateDiagram();
            this.diagramView.Focus();

            if (!selectNode)
            {
                this.diagramView.ClearSelection();
            }
        }

        // EDITPANEL RESIZE change panel with after text change
        private void NodeNameEdit_TextChanged(object sender, EventArgs e)
        {
            this.SetPanelSize();
        }

        private void SetPanelSize() {

            int padding = Node.NodePadding;

            SizeF s = Fonts.MeasureString(this.edit.Text == "" ? "X" : this.edit.Text, this.edit.Font);

            this.edit.Height = (int)Math.Round(s.Height, 0) + 2 * padding;
            this.Height = this.edit.Height;

            this.edit.Width = (int)Math.Round(s.Width, 0) + padding + 5;
            this.Width = this.edit.Width + 5;
        }

        // EDITPANEL EDIT catch keys in edit
        private void NodeNameEdit_KeyDown(object sender, KeyEventArgs e)
        {

            Keys keyData = e.KeyCode;

            if (KeyMap.ParseKey("ESCAPE", keyData)) // zrusenie editácie v panely
            {
                this.SaveNodeNamePanel();
                this.Focus();
            }

            if (KeyMap.ParseKey("ENTER", keyData) && !e.Shift) // zvretie panelu a vytvorenie novej editacie
            {
                this.SaveNodeNamePanel();
                this.Focus();
            }

            if (KeyMap.ParseKey("TAB", keyData) && !e.Shift) // zvretie panelu a vytvorenie novej editacie
            {
                this.SaveNodeNamePanel();
                this.Focus();
                this.diagramView.AddNodeAfterNode();
            }

            if (KeyMap.ParseKey("TAB", keyData) && e.Shift) // zvretie panelu a vytvorenie novej editacie
            {
                this.SaveNodeNamePanel();
                this.Focus();
                this.diagramView.AddNodeBelowNode();
            }

            if (KeyMap.ParseKey("ENTER", keyData) || KeyMap.ParseKey("TAB", keyData))
            {
                if (!e.Shift)
                {
                    e.Handled = e.SuppressKeyPress = true;
                }
            }
        }

        public bool IsEditing()
        {
            return this.editing;
        }

        public void ClosePanel()
        {
            this.SaveNodeNamePanel();
        }
    }
}
