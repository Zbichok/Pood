namespace Tooted
{
    partial class RegistreerimisForm
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
            lblNimi = new Label();
            txtNimi = new TextBox();
            lblEmail = new Label();
            txtEmail = new TextBox();
            lblParool = new Label();
            txtParool = new TextBox();
            lblKordus = new Label();
            txtKordus = new TextBox();
            btnRegistreeru = new Button();
            btnTagasi = new Button();
            lblVeateade = new Label();
            SuspendLayout();
            lblNimi.AutoSize = true;
            lblNimi.Location = new Point(30, 20);
            lblNimi.Name = "lblNimi";
            lblNimi.Text = "Nimi";
            txtNimi.Location = new Point(30, 40);
            txtNimi.Name = "txtNimi";
            txtNimi.Size = new Size(240, 23);
            txtNimi.TabIndex = 0;
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(30, 75);
            lblEmail.Name = "lblEmail";
            lblEmail.Text = "E-mail";
            txtEmail.Location = new Point(30, 95);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(240, 23);
            txtEmail.TabIndex = 1;
            lblParool.AutoSize = true;
            lblParool.Location = new Point(30, 130);
            lblParool.Name = "lblParool";
            lblParool.Text = "Parool (vähemalt 6 märki)";
            txtParool.Location = new Point(30, 150);
            txtParool.Name = "txtParool";
            txtParool.PasswordChar = '*';
            txtParool.Size = new Size(240, 23);
            txtParool.TabIndex = 2;
            lblKordus.AutoSize = true;
            lblKordus.Location = new Point(30, 185);
            lblKordus.Name = "lblKordus";
            lblKordus.Text = "Korda parooli";
            txtKordus.Location = new Point(30, 205);
            txtKordus.Name = "txtKordus";
            txtKordus.PasswordChar = '*';
            txtKordus.Size = new Size(240, 23);
            txtKordus.TabIndex = 3;
            btnRegistreeru.Location = new Point(30, 245);
            btnRegistreeru.Name = "btnRegistreeru";
            btnRegistreeru.Size = new Size(240, 35);
            btnRegistreeru.TabIndex = 4;
            btnRegistreeru.Text = "Registreeru";
            btnRegistreeru.UseVisualStyleBackColor = true;
            btnRegistreeru.Click += btnRegistreeru_Click;
            btnTagasi.Location = new Point(30, 288);
            btnTagasi.Name = "btnTagasi";
            btnTagasi.Size = new Size(240, 28);
            btnTagasi.TabIndex = 5;
            btnTagasi.Text = "Tagasi sisselogimisse";
            btnTagasi.UseVisualStyleBackColor = true;
            btnTagasi.Click += btnTagasi_Click;
            lblVeateade.ForeColor = Color.Red;
            lblVeateade.Location = new Point(30, 322);
            lblVeateade.Name = "lblVeateade";
            lblVeateade.Size = new Size(240, 36);
            lblVeateade.Text = "";
            AcceptButton = btnRegistreeru;
            CancelButton = btnTagasi;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(300, 365);
            Controls.Add(lblNimi);
            Controls.Add(txtNimi);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(lblParool);
            Controls.Add(txtParool);
            Controls.Add(lblKordus);
            Controls.Add(txtKordus);
            Controls.Add(btnRegistreeru);
            Controls.Add(btnTagasi);
            Controls.Add(lblVeateade);
            Name = "RegistreerimisForm";
            Text = "Registreerimine";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNimi;
        private TextBox txtNimi;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblParool;
        private TextBox txtParool;
        private Label lblKordus;
        private TextBox txtKordus;
        private Button btnRegistreeru;
        private Button btnTagasi;
        private Label lblVeateade;
    }
}
