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
            AcceptButton = btnRegistreeru;   // Enter vajutab Keel.T("Registreeru", "Регистрация", "Register")
            StartPosition = FormStartPosition.CenterParent;
            Stiil.Rakenda(this);
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
                lblVeateade.Text = Keel.T("Sisesta nimi!", "Введите имя!", "Enter your name!");
                return;
            }

            if (!OnEmail(email))
            {
                lblVeateade.Text = Keel.T("Vale e-maili formaat!", "Неверный формат почты!", "Invalid email format!");
                return;
            }

            if (parool.Length < 6)
            {
                lblVeateade.Text = Keel.T("Parool peab olema vähemalt 6 märki!", "Пароль: не менее 6 символов!", "Password: at least 6 characters!");
                return;
            }

            if (parool != kordus)
            {
                lblVeateade.Text = Keel.T("Paroolid ei kattu!", "Пароли не совпадают!", "Passwords do not match!");
                return;
            }

            try
            {
                using var db = new ToodedContext();

                if (db.Kasutajad.Any(k => k.Email.ToLower() == email))
                {
                    lblVeateade.Text = Keel.T("Selle e-mailiga kasutaja on juba olemas!", "Эта почта уже зарегистрирована!", "This email is already registered!");
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
                lblVeateade.Text = Keel.T("Viga: ", "Ошибка: ", "Error: ") + ex.Message;
                return;
            }

            MessageBox.Show(Keel.T("Konto loodud! Nüüd saad sisse logida.", "Аккаунт создан! Теперь можно войти.", "Account created! You can now sign in."));
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
