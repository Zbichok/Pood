using System;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;

namespace Tooted
{
    public partial class RegistreerimisForm : Form
    {
        public string RegistreeritudEmail { get; private set; } = "";

        public RegistreerimisForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private void btnRegistreeru_Click(object sender, EventArgs e)
        {
            string nimi = txtNimi.Text.Trim();
            string email = txtEmail.Text.Trim().ToLower();
            string parool = txtParool.Text;
            string kordus = txtKordus.Text;

            if (string.IsNullOrWhiteSpace(nimi))
            {
                lblVeateade.Text = "Sisesta nimi!";
                return;
            }

            if (!OnEmail(email))
            {
                lblVeateade.Text = "Vale e-maili formaat!";
                return;
            }

            if (parool.Length < 6)
            {
                lblVeateade.Text = "Parool peab olema vähemalt 6 märki!";
                return;
            }

            if (parool != kordus)
            {
                lblVeateade.Text = "Paroolid ei kattu!";
                return;
            }

            try
            {
                using var db = new ToodedContext();

                if (db.Kasutajad.Any(k => k.Email.ToLower() == email))
                {
                    lblVeateade.Text = "Selle e-mailiga kasutaja on juba olemas!";
                    return;
                }

                db.Kasutajad.Add(new Kasutaja
                {
                    Nimi = nimi,
                    Email = email,
                    ParoolHash = PasswordHelper.Hash(parool),
                    Roll = "kasutaja"
                });
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                lblVeateade.Text = "Viga: " + ex.Message;
                return;
            }

            MessageBox.Show("Konto loodud! Nüüd saad sisse logida.");
            RegistreeritudEmail = email;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnTagasi_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private static bool OnEmail(string tekst)
        {
            try
            {
                return new MailAddress(tekst).Address == tekst;
            }
            catch
            {
                return false;
            }
        }
    }
}
