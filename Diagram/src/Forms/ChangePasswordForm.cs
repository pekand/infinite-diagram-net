using System.Security;

#nullable disable

namespace Diagram
{
    public class ChangePasswordForm : Form //UID9355910334
    {
        public Main main = null;

        private System.Windows.Forms.Label labelOldPassword;
        private System.Windows.Forms.Label labelNewPassword1;
        private System.Windows.Forms.Label labelNewPassword2;
        private System.Windows.Forms.TextBox editOldPassword;
        private System.Windows.Forms.TextBox editNewPassword1;
        private System.Windows.Forms.TextBox editNewPassword2;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;

        public bool canceled = false;
        public SecureString oldPassword = null;
        public bool buttonok = false;

        public ChangePasswordForm(Main main)
        {
            this.main = main;
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePasswordForm));
            labelOldPassword = new Label();
            labelNewPassword1 = new Label();
            labelNewPassword2 = new Label();
            editOldPassword = new TextBox();
            editNewPassword1 = new TextBox();
            editNewPassword2 = new TextBox();
            buttonOk = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // labelOldPassword
            // 
            labelOldPassword.AutoSize = true;
            labelOldPassword.Location = new Point(27, 28);
            labelOldPassword.Margin = new Padding(5, 0, 5, 0);
            labelOldPassword.Name = "labelOldPassword";
            labelOldPassword.Size = new Size(96, 19);
            labelOldPassword.TabIndex = 0;
            labelOldPassword.Text = "Old password:";
            // 
            // labelNewPassword1
            // 
            labelNewPassword1.AutoSize = true;
            labelNewPassword1.Location = new Point(21, 66);
            labelNewPassword1.Margin = new Padding(5, 0, 5, 0);
            labelNewPassword1.Name = "labelNewPassword1";
            labelNewPassword1.Size = new Size(101, 19);
            labelNewPassword1.TabIndex = 1;
            labelNewPassword1.Text = "New password:";
            // 
            // labelNewPassword2
            // 
            labelNewPassword2.AutoSize = true;
            labelNewPassword2.Location = new Point(21, 104);
            labelNewPassword2.Margin = new Padding(5, 0, 5, 0);
            labelNewPassword2.Name = "labelNewPassword2";
            labelNewPassword2.Size = new Size(101, 19);
            labelNewPassword2.TabIndex = 2;
            labelNewPassword2.Text = "New password:";
            // 
            // editOldPassword
            // 
            editOldPassword.Location = new Point(135, 24);
            editOldPassword.Margin = new Padding(5, 5, 5, 5);
            editOldPassword.Name = "editOldPassword";
            editOldPassword.Size = new Size(331, 26);
            editOldPassword.TabIndex = 3;
            editOldPassword.UseSystemPasswordChar = true;
            // 
            // editNewPassword1
            // 
            editNewPassword1.Location = new Point(135, 62);
            editNewPassword1.Margin = new Padding(5, 5, 5, 5);
            editNewPassword1.Name = "editNewPassword1";
            editNewPassword1.Size = new Size(331, 26);
            editNewPassword1.TabIndex = 4;
            editNewPassword1.UseSystemPasswordChar = true;
            // 
            // editNewPassword2
            // 
            editNewPassword2.Location = new Point(135, 100);
            editNewPassword2.Margin = new Padding(5, 5, 5, 5);
            editNewPassword2.Name = "editNewPassword2";
            editNewPassword2.Size = new Size(329, 26);
            editNewPassword2.TabIndex = 5;
            editNewPassword2.UseSystemPasswordChar = true;
            // 
            // buttonOk
            // 
            buttonOk.Location = new Point(133, 138);
            buttonOk.Margin = new Padding(5, 5, 5, 5);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(85, 41);
            buttonOk.TabIndex = 6;
            buttonOk.Text = "Ok";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += ButtonOk_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(227, 138);
            buttonCancel.Margin = new Padding(5, 5, 5, 5);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(96, 41);
            buttonCancel.TabIndex = 7;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // ChangePasswordForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(483, 187);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOk);
            Controls.Add(editNewPassword2);
            Controls.Add(editNewPassword1);
            Controls.Add(editOldPassword);
            Controls.Add(labelNewPassword2);
            Controls.Add(labelNewPassword1);
            Controls.Add(labelOldPassword);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 5, 5, 5);
            Name = "ChangePasswordForm";
            Text = "ChangePasswordForm";
            FormClosed += ChangePasswordForm_FormClosed;
            Load += ChangePasswordForm_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        public void Clear()
        {
            this.editOldPassword.Text = "";
            this.editNewPassword1.Text = "";
            this.editNewPassword2.Text = "";
            canceled = false;
            buttonok = false;
            this.oldPassword = null;
        }

        public string GetPassword()
        {
            return this.editNewPassword1.Text;
        }


        private void ButtonOk_Click(object sender, EventArgs e)
        {
            if (!Encrypt.CompareSecureString(this.oldPassword, this.editOldPassword.Text))
            {
                MessageBox.Show("Old password is incorrect!");
                return;
            }

            if (this.editNewPassword1.Text != this.editNewPassword2.Text)
            {
                MessageBox.Show("The new password does not match!");
                return;
            }

            if (this.editNewPassword1.Text == "" && MessageBox.Show("Really remove password?", "Confirm remove password", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            this.canceled = false;
            buttonok = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.canceled = true;
            this.Close();
        }

        private void ChangePasswordForm_Load(object sender, EventArgs e)
        {
            canceled = false;
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            this.ActiveControl = editOldPassword;
        }

        private void ChangePasswordForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            canceled = true;

            if (buttonok)
            {
                canceled = false;
            }
        }

    }
}
