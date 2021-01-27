using System;
using System.Windows.Forms;

namespace Diagram
{
    public class SearchPanel : Panel //UID1308094022
    {
        public DiagramView diagramView = null;

        public int minimalSize = 100;
        public int maximalSize = 400;

        public delegate void SearchPanelChangedEventHandler(string action, string search);
        public event SearchPanelChangedEventHandler SearchpanelStateChanged;

        public string oldText = "";

        private System.Windows.Forms.TextBox textBoxSearch;

        public SearchPanel(DiagramView diagramView)
        {
            this.diagramView = diagramView;

            InitializeComponent();
            InitializeSearchPanelComponent();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            //
            // textBoxSearch
            //
            this.textBoxSearch.Font = new System.Drawing.Font(
                "Microsoft Sans Serif",
                16F, 
                System.Drawing.FontStyle.Regular, 
                System.Drawing.GraphicsUnit.Point, 
                ((byte)(0))
            );
            this.textBoxSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxSearch.Location = new System.Drawing.Point(4, 4);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(100, 13);
            this.textBoxSearch.TabIndex = 0;
            this.textBoxSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBoxSearch_KeyUp);
            this.textBoxSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxSearch_KeyDown);
            //
            // SearchPanel
            //
            this.Controls.Add(this.textBoxSearch);
            this.VisibleChanged += new System.EventHandler(this.Panel_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void InitializeSearchPanelComponent()
        {

            this.textBoxSearch.Size = new System.Drawing.Size(this.minimalSize, 15);
            this.Size = new System.Drawing.Size(this.minimalSize + 4, 15);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));

            Form parentForm = (this.diagramView as Form);
            parentForm.Resize += new System.EventHandler(this.Parent_Resize);

            System.Drawing.Size size = TextRenderer.MeasureText("Text", textBoxSearch.Font);
            textBoxSearch.Height = size.Height;
            this.Height = size.Height + 5;

            this.CenterPanel();
        }

        private void TextBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            string action = "";
            string currentText = textBoxSearch.Text;

            if (oldText != currentText && currentText != "")
            {
                action = "search";
                oldText = currentText;
            }

            if (e.KeyCode == Keys.Down)
            {
                action = "searchNext";
            }

            if (e.KeyCode == Keys.Up)
            {
                action = "searchPrev";
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                this.Hide();
                action = "close";
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
                action = "cancel";
            }

            this.SearchpanelStateChanged?.Invoke(action, currentText);

            this.CenterPanel();
        }

        public void SearchNext() //UID3222624449
        {
            string currentText = textBoxSearch.Text;

            this.SearchpanelStateChanged?.Invoke("searchNext", currentText);
        }

        private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            // remove ding sound
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        public void CenterPanel()
        {
            Form parentForm = (this.diagramView as Form);
            this.Top = parentForm.Height - 100;
            this.Left = parentForm.Width / 2 - this.Width / 2;

            maximalSize = parentForm.Width - 100;

            int newWidth = 0;
            System.Drawing.Size size = TextRenderer.MeasureText(textBoxSearch.Text, textBoxSearch.Font);
            newWidth = size.Width + 5;

            if (minimalSize <= newWidth && newWidth <= maximalSize)
            {
                this.Width = newWidth + 2;
                textBoxSearch.Width = newWidth;
            }
        }

        private void Parent_Resize(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.CenterPanel();
            }
        }

        private void Panel_VisibleChanged(object sender, EventArgs e)
        {
            this.CenterPanel();
        }

        public void ShowPanel()
        {
            this.Show();
            this.CenterPanel();
            this.textBoxSearch.Focus();
            this.textBoxSearch.SelectAll();
        }

        public void Highlight(bool state)
        {
            if (state)
            {
                this.BackColor = System.Drawing.Color.FromArgb(255, 178, 178);
                this.textBoxSearch.BackColor = System.Drawing.Color.FromArgb(255, 178, 178);
            }
            else
            {
                this.BackColor = System.Drawing.Color.FromArgb(128, 128, 204);
                this.textBoxSearch.BackColor = System.Drawing.Color.FromArgb(128, 128, 204);
            }
        }

        public void HidePanel()
        {
            this.Hide();
        }
    }
}
