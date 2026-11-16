using System;
using System.Linq;
using System.Windows.Forms;

namespace Tooted
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Stiil.Rakenda(this);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using var db = new ToodedContext();

            string email = txtEmail.Text.Trim().ToLower();
            string parool = txtParool.Text;

            var kasutaja = db.Kasutajad.FirstOrDefault(k => k.Email.ToLower() == email);

            // Sama teade mõlemal juhul: võõras ei saa teada, kas selline e-post on olemas
            if (kasutaja == null || !PasswordHelper.Verify(parool, kasutaja.ParoolHash))
            {
                lblVeateade.Text = "Vale e-post või parool.";
                txtParool.Clear();
                return;
            }

            AktiivneKasutaja.Nimi = kasutaja.Nimi;
            AktiivneKasutaja.Roll = kasutaja.Roll;
            AktiivneKasutaja.Email = kasutaja.Email;
            AktiivneKasutaja.LaeBoonused();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnRegistreeru_Click(object sender, EventArgs e)
        {
            using var reg = new RegistreerimisForm();

            if (reg.ShowDialog(this) == DialogResult.OK)
            {
                txtEmail.Text = reg.RegistreeritudEmail;
                txtParool.Clear();
                txtParool.Focus();
                lblVeateade.Text = "";
            }
        }
    }
}
