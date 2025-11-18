#nullable disable

namespace Diagram
{
    public partial class PasswordForm : Form //UID8255589531
    {
        public Main main = null;

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox editPassword;

        public bool ok = false;
        public bool cancled = false;

        public PasswordForm(Main main)
        {
            this.main = main;
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordForm));
            buttonOk = new Button();
            buttonCancel = new Button();
            labelPassword = new Label();
            editPassword = new TextBox();
            SuspendLayout();
            // 
            // buttonOk
            // 
            buttonOk.Location = new Point(105, 51);
            buttonOk.Margin = new Padding(5, 4, 5, 4);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(101, 46);
            buttonOk.TabIndex = 4;
            buttonOk.Text = "Ok";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += ButtonOk_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(214, 51);
            buttonCancel.Margin = new Padding(5, 4, 5, 4);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(101, 47);
            buttonCancel.TabIndex = 5;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // labelPassword
            // 
            labelPassword.AutoSize = true;
            labelPassword.Location = new Point(26, 18);
            labelPassword.Margin = new Padding(5, 0, 5, 0);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(70, 19);
            labelPassword.TabIndex = 6;
            labelPassword.Text = "Password:";
            // 
            // editPassword
            // 
            editPassword.Location = new Point(105, 13);
            editPassword.Margin = new Padding(5, 4, 5, 4);
            editPassword.Name = "editPassword";
            editPassword.Size = new Size(377, 26);
            editPassword.TabIndex = 8;
            editPassword.UseSystemPasswordChar = true;
            editPassword.KeyDown += EditPassword_KeyDown;
            // 
            // PasswordForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(495, 108);
            Controls.Add(editPassword);
            Controls.Add(labelPassword);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOk);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 4, 5, 4);
            Name = "PasswordForm";
            Text = "Password";
            Activated += PasswordForm_Activated;
            FormClosed += PasswordForm_FormClosed;
            Load += PasswordForm_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        private void PasswordForm_Load(object sender, EventArgs e)
        {
            this.Clear();
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;

            Program.log.Write("bring focus");
            Media.BringToFront(this);

            this.editPassword.Focus();
        }

        private void PasswordForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            cancled = true;

            if (ok)
            {
                cancled = false;
            }
        }

        public void Clear()
        {
            this.editPassword.Text = "";
            ok = false;
            cancled = false;
        }

        public string GetPassword() {
            return this.editPassword.Text;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            this.ok = true;
            this.cancled = false;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.ok = false;
            this.cancled = true;
            this.Close();
        }

        private void EditPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ButtonOk_Click(sender, e);
            }
        }

        private void PasswordForm_Activated(object sender, EventArgs e)
        {
            this.editPassword.Focus();
        }
    }
}
