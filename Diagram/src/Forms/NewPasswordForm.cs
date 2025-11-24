#nullable disable

namespace Diagram
{
    public partial class NewPasswordForm : Form 
    {
        public Main main = null;

        private System.Windows.Forms.Label labelNewPassword1;
        private System.Windows.Forms.Label labelNewPassword2;
        private System.Windows.Forms.TextBox editNewPassword1;
        private System.Windows.Forms.TextBox editNewPassword2;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;

        public bool cancled = false;
        public bool buttonok = false;

        public NewPasswordForm(Main main)
        {
            this.main = main;
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewPasswordForm));
            labelNewPassword1 = new Label();
            labelNewPassword2 = new Label();
            editNewPassword1 = new TextBox();
            editNewPassword2 = new TextBox();
            buttonOk = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // labelNewPassword1
            // 
            labelNewPassword1.AutoSize = true;
            labelNewPassword1.Location = new Point(16, 37);
            labelNewPassword1.Margin = new Padding(5, 0, 5, 0);
            labelNewPassword1.Name = "labelNewPassword1";
            labelNewPassword1.Size = new Size(101, 19);
            labelNewPassword1.TabIndex = 2;
            labelNewPassword1.Text = "New password:";
            // 
            // labelNewPassword2
            // 
            labelNewPassword2.AutoSize = true;
            labelNewPassword2.Location = new Point(16, 75);
            labelNewPassword2.Margin = new Padding(5, 0, 5, 0);
            labelNewPassword2.Name = "labelNewPassword2";
            labelNewPassword2.Size = new Size(101, 19);
            labelNewPassword2.TabIndex = 3;
            labelNewPassword2.Text = "New password:";
            // 
            // editNewPassword1
            // 
            editNewPassword1.Location = new Point(144, 32);
            editNewPassword1.Margin = new Padding(5, 5, 5, 5);
            editNewPassword1.Name = "editNewPassword1";
            editNewPassword1.Size = new Size(347, 26);
            editNewPassword1.TabIndex = 5;
            editNewPassword1.UseSystemPasswordChar = true;
            // 
            // editNewPassword2
            // 
            editNewPassword2.Location = new Point(144, 70);
            editNewPassword2.Margin = new Padding(5, 5, 5, 5);
            editNewPassword2.Name = "editNewPassword2";
            editNewPassword2.Size = new Size(347, 26);
            editNewPassword2.TabIndex = 6;
            editNewPassword2.UseSystemPasswordChar = true;
            // 
            // buttonOk
            // 
            buttonOk.Location = new Point(144, 108);
            buttonOk.Margin = new Padding(5, 5, 5, 5);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(85, 41);
            buttonOk.TabIndex = 7;
            buttonOk.Text = "Ok";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += ButtonOk_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(237, 108);
            buttonCancel.Margin = new Padding(5, 5, 5, 5);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(96, 41);
            buttonCancel.TabIndex = 8;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // NewPasswordForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(507, 161);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOk);
            Controls.Add(editNewPassword2);
            Controls.Add(editNewPassword1);
            Controls.Add(labelNewPassword2);
            Controls.Add(labelNewPassword1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 5, 5, 5);
            Name = "NewPasswordForm";
            Text = "NewPasswordForm";
            FormClosed += NewPasswordForm_FormClosed;
            Load += NewPasswordForm_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        public void Clear()
        {
            this.editNewPassword1.Text = "";
            this.editNewPassword2.Text = "";
            buttonok = false;
            cancled = false;
        }

        public string GetPassword()
        {
            return this.editNewPassword1.Text;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            if (this.editNewPassword1.Text != this.editNewPassword2.Text)
            {
                MessageBox.Show("The new password does not match!");
                return;
            }

            this.cancled = false;
            buttonok = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.cancled = true;
            this.Close();
        }

        private void NewPasswordForm_Load(object sender, EventArgs e)
        {
            cancled = false;
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            this.ActiveControl = editNewPassword1;
        }

        private void NewPasswordForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            cancled = true;

            if (buttonok)
            {
                cancled = false;
            }
        }

    }
}
