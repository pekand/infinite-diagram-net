#nullable disable

namespace Diagram
{
    public class TextForm : Form //UID6772159546
    {
        public Main main = null;



        public delegate void TextFormSaveEventHandler(Node node);
        public event TextFormSaveEventHandler TextFormSave;

        /*************************************************************************************************************************/

        // ATTRIBUTES Diagram
        public Diagram diagram = null;       // diagram ktory je previazany z pohladom

        private SplitContainer SplitContainer1;
        private RichTextBox TextFormTextBox;
        private RichTextBox TextFormNoteTextBox;
        private TextBox TextFormLinkTextBox;
        public Node node;

        public TextForm(Main main)
        {
            this.main = main;
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextForm));
            SplitContainer1 = new SplitContainer();
            TextFormTextBox = new RichTextBox();
            TextFormLinkTextBox = new TextBox();
            TextFormNoteTextBox = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)SplitContainer1).BeginInit();
            SplitContainer1.Panel1.SuspendLayout();
            SplitContainer1.Panel2.SuspendLayout();
            SplitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // SplitContainer1
            // 
            SplitContainer1.Dock = DockStyle.Fill;
            SplitContainer1.Location = new Point(0, 0);
            SplitContainer1.Margin = new Padding(4);
            SplitContainer1.Name = "SplitContainer1";
            SplitContainer1.Orientation = Orientation.Horizontal;
            // 
            // SplitContainer1.Panel1
            // 
            SplitContainer1.Panel1.Controls.Add(TextFormTextBox);
            // 
            // SplitContainer1.Panel2
            // 
            SplitContainer1.Panel2.Controls.Add(TextFormLinkTextBox);
            SplitContainer1.Panel2.Controls.Add(TextFormNoteTextBox);
            SplitContainer1.Size = new Size(524, 756);
            SplitContainer1.SplitterDistance = 103;
            SplitContainer1.SplitterWidth = 6;
            SplitContainer1.TabIndex = 0;
            // 
            // TextFormTextBox
            // 
            TextFormTextBox.DetectUrls = false;
            TextFormTextBox.Dock = DockStyle.Fill;
            TextFormTextBox.Font = new Font("Times New Roman", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TextFormTextBox.Location = new Point(0, 0);
            TextFormTextBox.Margin = new Padding(4);
            TextFormTextBox.Name = "TextFormTextBox";
            TextFormTextBox.Size = new Size(524, 103);
            TextFormTextBox.TabIndex = 0;
            TextFormTextBox.Text = "";
            TextFormTextBox.KeyDown += TextFormTextBox_KeyDown;
            // 
            // TextFormLinkTextBox
            // 
            TextFormLinkTextBox.Dock = DockStyle.Bottom;
            TextFormLinkTextBox.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TextFormLinkTextBox.Location = new Point(0, 608);
            TextFormLinkTextBox.Margin = new Padding(4);
            TextFormLinkTextBox.Name = "TextFormLinkTextBox";
            TextFormLinkTextBox.Size = new Size(524, 39);
            TextFormLinkTextBox.TabIndex = 1;
            TextFormLinkTextBox.TextChanged += TextFormLinkTextBox_TextChanged;
            TextFormLinkTextBox.KeyDown += TextFormLinkTextBox_KeyDown;
            // 
            // TextFormNoteTextBox
            // 
            TextFormNoteTextBox.DetectUrls = false;
            TextFormNoteTextBox.Dock = DockStyle.Top;
            TextFormNoteTextBox.Font = new Font("Courier New", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TextFormNoteTextBox.Location = new Point(0, 0);
            TextFormNoteTextBox.Margin = new Padding(4);
            TextFormNoteTextBox.Name = "TextFormNoteTextBox";
            TextFormNoteTextBox.Size = new Size(524, 589);
            TextFormNoteTextBox.TabIndex = 0;
            TextFormNoteTextBox.Text = "";
            TextFormNoteTextBox.KeyDown += TextFormNoteTextBox_KeyDown;
            // 
            // TextForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 756);
            Controls.Add(SplitContainer1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(4);
            Name = "TextForm";
            Text = "Edit";
            FormClosed += TextForm_FormClosed;
            Load += TextForm_Load;
            Resize += TextForm_Resize;
            SplitContainer1.Panel1.ResumeLayout(false);
            SplitContainer1.Panel2.ResumeLayout(false);
            SplitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SplitContainer1).EndInit();
            SplitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void TextFormTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                String insertText = (string)Clipboard.GetData("Text");
                TextFormTextBox.SelectedText = insertText;

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void TextFormNoteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                String insertText = (string)Clipboard.GetData("Text");
                TextFormNoteTextBox.SelectedText = insertText;

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void TextFormLinkTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                String insertText = (string)Clipboard.GetData("Text");
                TextFormLinkTextBox.SelectedText = insertText;

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }


        public void TextForm_Load(object sender, EventArgs e)
        {
            if (this.node != null)
            {
                this.TextFormTextBox.Text = this.node.name;
                this.TextFormNoteTextBox.Text = this.node.note;
                this.TextFormLinkTextBox.Text = this.node.link;
                this.Left = Screen.PrimaryScreen.Bounds.Width / 2 - this.Width / 2;
                this.Top = Screen.PrimaryScreen.Bounds.Height / 2 - this.Height / 2;
                this.TextFormTextBox.Select();
            }

            if (this.diagram.IsReadOnly())
            {
                this.TextFormTextBox.ReadOnly = true;
                this.TextFormNoteTextBox.ReadOnly = true;
            }
            else
            {
                this.TextFormTextBox.ReadOnly = false;
                this.TextFormNoteTextBox.ReadOnly = false;
            }

            this.ResizePanel();
        }

        public void SetDiagram(Diagram diagram)
        {
            this.diagram = diagram;
        }

        public Diagram GetDiagram()
        {
            return this.diagram;
        }

        public void TextForm_Resize(object sender, EventArgs e)
        {
            this.ResizePanel();
        }

        private void ResizePanel()
        {
            this.TextFormTextBox.Height = this.ClientSize.Height - 100;
            this.TextFormNoteTextBox.Height = SplitContainer1.Panel2.Height - this.TextFormLinkTextBox.Height;
        }

        // Save data and update main form
        public void SaveNode()
        {
            if (!this.diagram.IsReadOnly())
            {
                if (
                    node.name != this.TextFormTextBox.Text ||
                    node.note != this.TextFormNoteTextBox.Text ||
                    node.link != this.TextFormLinkTextBox.Text
                )
                {
                    this.diagram.undoOperations.Add("edit", node, node.position, node.layer);
                    node.name = this.TextFormTextBox.Text;
                    node.note = this.TextFormNoteTextBox.Text;
                    node.link = this.TextFormLinkTextBox.Text;
                    node.Resize();

                    DateTime dt = DateTime.Now;
                    node.timemodify = String.Format("{0:yyyy-M-d HH:mm:ss}", dt);

                    this.TextFormSave?.Invoke(node);
                }
            }
        }

        public void TextForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.SaveNode();
            this.diagram.EditNodeClose(this.node);

            this.diagram.RemoveTextForm(this);
            main.RemoveTextWindow(this);
            this.diagram.CloseDiagram();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                this.SaveNode();
                this.diagram.Save();
                return true;
            }

            if (keyData == Keys.Escape)
            {
                this.SaveNode();
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void TextFormLinkTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
