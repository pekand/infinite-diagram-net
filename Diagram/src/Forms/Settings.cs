using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

#nullable disable

namespace Diagram.src.Forms
{
    public partial class Settings : Form
    {

        public Diagram diagram;

        public Dictionary<string, Control> ControlsMap { get; } = new Dictionary<string, Control>(StringComparer.OrdinalIgnoreCase);

        public Settings(Diagram diagram)
        {
            this.diagram = diagram;

            InitializeComponent();
        }



        private void Settings_Load(object sender, EventArgs e)
        {
            this.Settings_Resize(sender, e);

            int pos = 21;
            this.AddLabel("lblMaxZoom", "Max zoom:", new Point(21, pos), new Size(100, 20));
            this.AddTextBox("txtMaxZoom", string.Empty, new Point(160, pos), new Size(267, 27));

            pos += 30;
            this.AddLabel("lblMinZoom", "Min zoom:", new Point(21, pos), new Size(100, 20));
            this.AddTextBox("txtMinZoom", string.Empty, new Point(160, pos), new Size(267, 27));

            pos += 30;
            this.AddLabel("lblZoomStep", "Zoom step:", new Point(21, pos), new Size(100, 20));
            this.AddTextBox("txtZoomStep", string.Empty, new Point(160, pos), new Size(267, 27));


            (ControlsMap["txtMaxZoom"] as TextBox).Text = diagram.options.currentMaxZoom.ToString();
            (ControlsMap["txtMinZoom"] as TextBox).Text = diagram.options.currentMinZoom.ToString();
            (ControlsMap["txtZoomStep"] as TextBox).Text = diagram.options.currentZoomStep.ToString();

        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (validate()) {
                diagram.options.currentMaxZoom = Decimal.Parse((ControlsMap["txtMaxZoom"] as TextBox).Text);
                diagram.options.currentMinZoom = Decimal.Parse((ControlsMap["txtMinZoom"] as TextBox).Text);
                diagram.options.currentZoomStep = Decimal.Parse((ControlsMap["txtZoomStep"] as TextBox).Text);
                this.Close();
            }
        }

        private void Settings_Resize(object sender, EventArgs e)
        {
            buttonOk.Left = this.Width - buttonOk.Width - 20;
            panel.Height = this.Height - buttonOk.Height - 50;
            buttonOk.Top = this.Height - buttonOk.Height - 50;
        }

        public Button AddButton(string name, string text, Point location, Size size)
        {
            var b = new Button();
            b.Name = name;
            b.Text = text ?? string.Empty;
            b.Location = location;
            b.Size = size;
            panel.Controls.Add(b);
            ControlsMap[name] = b;
            return b;
        }

        public TextBox AddTextBox(string name, string text, Point location, Size? size)
        {
            var tb = new TextBox();
            tb.Name = name;
            tb.Text = text ?? string.Empty;
            tb.Location = location;
            tb.Size = size.Value;
            tb.TextChanged += this.textBox_TextChanged;
            panel.Controls.Add(tb);
            ControlsMap[name] = tb;
            return tb;
        }

        public Label AddLabel(string name, string text, Point location, Size size)
        {
            var lbl = new Label();
            lbl.Name = name;
            lbl.Text = text ?? string.Empty;
            lbl.AutoSize = false;
            lbl.Location = location;
            lbl.Size = size;
            panel.Controls.Add(lbl);
            ControlsMap[name] = lbl;
            return lbl;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            validate();
        }

        private bool validate()
        {
            TextBox txtMaxZoom = (ControlsMap["txtMaxZoom"] as TextBox);
            TextBox txtMinZoom = (ControlsMap["txtMinZoom"] as TextBox);
            TextBox txtZoomStep = (ControlsMap["txtZoomStep"] as TextBox);

            decimal? txtMaxZoomValue = Converter.StringToDecimal(txtMaxZoom.Text) ?? null;
            decimal? txtMinZoomValue = Converter.StringToDecimal(txtMinZoom.Text) ?? null;
            decimal? txtZoomStepValue = Converter.StringToDecimal(txtZoomStep.Text) ?? null;

            Color errorColor = Converter.ColorFromHexString("#ffd6e0");
            Color defaultColor = Color.White;

            bool valid = true;
            if (txtMaxZoomValue == null || txtMaxZoomValue > diagram.options.maxZoom || txtMaxZoomValue < diagram.options.minZoom) 
            {
                valid = false;
                if (txtMaxZoom.BackColor != errorColor) txtMaxZoom.BackColor = errorColor;
            } else {
                if (txtMaxZoom.BackColor != defaultColor) txtMaxZoom.BackColor = defaultColor;
            }

            if (txtMinZoomValue == null || txtMinZoomValue > diagram.options.maxZoom || txtMinZoomValue < diagram.options.minZoom || txtMinZoomValue > txtMaxZoomValue)
            {
                valid = false;
                if (txtMinZoom.BackColor != errorColor) txtMinZoom.BackColor = errorColor;
            }
            else
            {
                if (txtMinZoom.BackColor != defaultColor) txtMinZoom.BackColor = defaultColor;
            }

            if (txtZoomStepValue == null)
            {
                valid = false;
                if (txtZoomStep.BackColor != errorColor) txtZoomStep.BackColor = errorColor;
            }
            else
            {
                if (txtZoomStep.BackColor != defaultColor) txtZoomStep.BackColor = defaultColor;
            }

            return valid;

        }
    }
}
