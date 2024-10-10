using System;
using System.Windows.Forms;

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
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TextFormTextBox = new System.Windows.Forms.RichTextBox();
            this.TextFormLinkTextBox = new System.Windows.Forms.TextBox();
            this.TextFormNoteTextBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SplitContainer1
            // 
            this.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.SplitContainer1.Name = "SplitContainer1";
            this.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer1.Panel1
            // 
            this.SplitContainer1.Panel1.Controls.Add(this.TextFormTextBox);
            // 
            // SplitContainer1.Panel2
            // 
            this.SplitContainer1.Panel2.Controls.Add(this.TextFormLinkTextBox);
            this.SplitContainer1.Panel2.Controls.Add(this.TextFormNoteTextBox);
            this.SplitContainer1.Size = new System.Drawing.Size(393, 517);
            this.SplitContainer1.SplitterDistance = 71;
            this.SplitContainer1.TabIndex = 0;
            // 
            // TextFormTextBox
            // 
            this.TextFormTextBox.DetectUrls = false;
            this.TextFormTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextFormTextBox.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.TextFormTextBox.Location = new System.Drawing.Point(0, 0);
            this.TextFormTextBox.Name = "TextFormTextBox";
            this.TextFormTextBox.Size = new System.Drawing.Size(393, 71);
            this.TextFormTextBox.TabIndex = 0;
            this.TextFormTextBox.Text = "";
            this.TextFormTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextFormTextBox_KeyDown);
            // 
            // TextFormLinkTextBox
            // 
            this.TextFormLinkTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TextFormLinkTextBox.Location = new System.Drawing.Point(0, 422);
            this.TextFormLinkTextBox.Name = "TextFormLinkTextBox";
            this.TextFormLinkTextBox.Size = new System.Drawing.Size(393, 20);
            this.TextFormLinkTextBox.TabIndex = 1;
            this.TextFormLinkTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextFormLinkTextBox_KeyDown);
            // 
            // TextFormNoteTextBox
            // 
            this.TextFormNoteTextBox.DetectUrls = false;
            this.TextFormNoteTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.TextFormNoteTextBox.Font = new System.Drawing.Font("Courier New", 12F);
            this.TextFormNoteTextBox.Location = new System.Drawing.Point(0, 0);
            this.TextFormNoteTextBox.Name = "TextFormNoteTextBox";
            this.TextFormNoteTextBox.Size = new System.Drawing.Size(393, 404);
            this.TextFormNoteTextBox.TabIndex = 0;
            this.TextFormNoteTextBox.Text = "";
            this.TextFormNoteTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextFormNoteTextBox_KeyDown);
            // 
            // TextForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 517);
            this.Controls.Add(this.SplitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "TextForm";
            this.Text = "Edit";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TextForm_FormClosed);
            this.Load += new System.EventHandler(this.TextForm_Load);
            this.Resize += new System.EventHandler(this.TextForm_Resize);
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            this.SplitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void TextFormTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                String insertText = (string)Clipboard.GetData("Text");
                TextFormTextBox.SelectedText = insertText;

                e.Handled = true;
            }
        }

        private void TextFormNoteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                String insertText = (string)Clipboard.GetData("Text");
                TextFormNoteTextBox.SelectedText = insertText;

                e.Handled = true;
            }
        }

        private void TextFormLinkTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                String insertText = (string)Clipboard.GetData("Text");
                TextFormLinkTextBox.SelectedText = insertText;

                e.Handled = true;
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
            if (!this.diagram.options.readOnly)
            {
                if (
                    node.name != this.TextFormTextBox.Text ||
                    node.note != this.TextFormNoteTextBox.Text ||
                    node.link != this.TextFormLinkTextBox.Text
                ) {
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

        
    }
}
