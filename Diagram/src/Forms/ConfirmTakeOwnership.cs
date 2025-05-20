using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#nullable disable

namespace Diagram
{
    public partial class ConfirmTakeOwnership : Form
    {
        private bool confirmed = false;
        private bool cancled = false;

        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;

        public ConfirmTakeOwnership()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(ConfirmTakeOwnership));
            labelMessage = new Label();
            buttonOk = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // labelMessage
            // 
            labelMessage.AutoSize = true;
            labelMessage.Location = new Point(50, 75);
            labelMessage.Margin = new Padding(5, 0, 5, 0);
            labelMessage.Name = "labelMessage";
            labelMessage.Size = new Size(156, 19);
            labelMessage.TabIndex = 0;
            labelMessage.Text = "Take diagram ownership";
            // 
            // buttonOk
            // 
            buttonOk.Location = new Point(328, 149);
            buttonOk.Margin = new Padding(5, 4, 5, 4);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(101, 34);
            buttonOk.TabIndex = 1;
            buttonOk.Text = "Ok";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += buttonOk_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(437, 149);
            buttonCancel.Margin = new Padding(5, 4, 5, 4);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(101, 34);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // ConfirmTakeOwnership
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(578, 237);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOk);
            Controls.Add(labelMessage);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 4, 5, 4);
            Name = "ConfirmTakeOwnership";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Take ownership";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.confirmed = true;
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.cancled = true;
            this.Close();
        }

        public bool isConfirmed()
        {
            return this.confirmed;
        }

        public bool isCancled()
        {
            return this.cancled;
        }
    }
}
