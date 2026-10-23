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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using var db = new ToodedContext();

            string email = txtEmail.Text.Trim().ToLower();
            string parool = txtParool.Text;

            var kasutaja = db.Kasutajad.FirstOrDefault(k => k.Email.ToLower() == email);

            if (kasutaja == null)
            {
                lblVeateade.Text = "Vale email!";
                return;
            }

            if (!PasswordHelper.Verify(parool, kasutaja.ParoolHash))
            {
                lblVeateade.Text = "Vale parool!";
                return;
            }

            AktiivneKasutaja.Nimi = kasutaja.Nimi;
            AktiivneKasutaja.Roll = kasutaja.Roll;

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
