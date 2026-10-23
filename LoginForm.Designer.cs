namespace Tooted
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblEmail = new Label();
            txtEmail = new TextBox();
            lblParool = new Label();
            txtParool = new TextBox();
            btnLogin = new Button();
            lblVeateade = new Label();
            btnRegistreeru = new Button();
            SuspendLayout();
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(20, 20);
            lblEmail.Name = "lblEmail";
            lblEmail.Text = "E-mail";
            txtEmail.Location = new Point(20, 40);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(240, 23);
            txtEmail.TabIndex = 0;
            lblParool.AutoSize = true;
            lblParool.Location = new Point(20, 75);
            lblParool.Name = "lblParool";
            lblParool.Text = "Parool";
            txtParool.Location = new Point(20, 95);
            txtParool.Name = "txtParool";
            txtParool.PasswordChar = '*';
            txtParool.Size = new Size(240, 23);
            txtParool.TabIndex = 1;
            btnLogin.Location = new Point(20, 130);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(115, 28);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "Logi sisse";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            btnRegistreeru.Location = new Point(145, 130);
            btnRegistreeru.Name = "btnRegistreeru";
            btnRegistreeru.Size = new Size(115, 28);
            btnRegistreeru.TabIndex = 3;
            btnRegistreeru.Text = "Registreeru";
            btnRegistreeru.UseVisualStyleBackColor = true;
            btnRegistreeru.Click += btnRegistreeru_Click;
            lblVeateade.AutoSize = true;
            lblVeateade.ForeColor = Color.Red;
            lblVeateade.Location = new Point(20, 168);
            lblVeateade.Name = "lblVeateade";
            AcceptButton = btnLogin;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(280, 200);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(lblParool);
            Controls.Add(txtParool);
            Controls.Add(btnLogin);
            Controls.Add(btnRegistreeru);
            Controls.Add(lblVeateade);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblParool;
        private TextBox txtParool;
        private Button btnLogin;
        private Label lblVeateade;
        private Button btnRegistreeru;
    }
}
