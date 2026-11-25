using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace Tooted
{
    public partial class ArveForm : Form
    {
        // ------------------------------------------------------------------
        // Gmail e-maili saatmine.
        // Gmail ei luba tavalist parooli – vaja on "rakenduse parooli" (App password):
        //   1. Google kontol peab olema sisse lülitatud kaheastmeline autentimine.
        //   2. myaccount.google.com -> Turvalisus -> Rakenduste paroolid -> loo uus
        //      (https://myaccount.google.com/apppasswords)
        //   3. Google näitab 16-kohalist parooli – kirjuta see siia konstandi asemele "xxxx".
        //
        // NB! Päris parooliga koodi ei tohi GitHubi push'ida! Enne `git push` pane tagasi "xxxx".
        // Kui siin on veel "xxxx", küsib programm saatja e-maili ja parooli aknas.
        // ------------------------------------------------------------------
        private const string saatjaEmail = "xxxx";
        private const string rakenduseParool = "xxxx";


        // Kontrollid, mis luuakse koodis (vt LisaKontrollid)
        private readonly Label lblKokkuvote = new Label();
        private readonly Button btnMaksa = new Button();

        public ArveForm()
        {
            InitializeComponent();
            LisaKontrollid();
            StartPosition = FormStartPosition.CenterScreen;
            Stiil.Rakenda(this);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Stiil.LisaTagasiNupp(this, new Point(520, 435));

            // Admin: ostud on ainult testostud (arve PDF + e-post), andmebaasi ei muudeta.
            if (AktiivneKasutaja.OnAdmin)
            {
                btnMaksa.Text = Keel.T("Testost", "Тестовая покупка", "Test purchase");
                lblMarkus.Text = Keel.T("Admin: testost ei muuda andmebaasi ega ostukorvi.", "Админ: тестовая покупка не меняет базу и корзину.", "Admin: test purchase does not change the database or cart.");
            }

            UuendaOstukorvi();
            Paiguta();
        }

        private void LisaKontrollid()
        {
            ClientSize = new Size(670, 480);

            lblKokkuvote.AutoSize = false;
            lblKokkuvote.Location = new Point(20, 322);
            lblKokkuvote.Size = new Size(440, 84);

            lblKokku.Location = new Point(20, 410);
            lblMarkus.Location = new Point(20, 445);

            btnMaksa.Text = Keel.T("Maksa", "Оплатить", "Pay");
            btnMaksa.Location = new Point(480, 245);
            btnMaksa.Size = new Size(160, 35);
            btnMaksa.Click += btnMaksa_Click;

            Controls.Add(lblKokkuvote);
            Controls.Add(btnMaksa);
        }

        private void UuendaOstukorvi()
        {
            listOstukorv.Items.Clear();
            foreach (var rida in Ostukorv.Read)
                listOstukorv.Items.Add(rida);

            lblKokkuvote.Text = string.Join(Environment.NewLine, Ostukorv.Kokkuvote());
            lblKokku.Text = Keel.T($"Kokku: {Ostukorv.Kokku:0.00} €", $"Итого: {Ostukorv.Kokku:0.00} €", $"Total: {Ostukorv.Kokku:0.00} €");
        }

        // ------------------------------------------------------------------
        // Maksmine: laoseis väheneb, ostukorv tühjendatakse
        // ------------------------------------------------------------------
        private void btnMaksa_Click(object sender, EventArgs e)
        {
            if (Ostukorv.Read.Count == 0)
            {
                MessageBox.Show(Keel.T("Ostukorv on tühi.", "Корзина пуста.", "The cart is empty."));
                return;
            }

            // 1) Aadress (ei salvestata kuhugi, elab ainult selle ostu tšekis)
            string? aadress = AadressForm.Kysi(this);
            if (aadress == null) return;

            if (AktiivneKasutaja.OnAdmin)
            {
                // Testost: andmebaasi ega ostukorvi ei muudeta
                if (MessageBox.Show(
                        Keel.T("Testost: andmebaasi ega ostukorvi ei muudeta.\n\nJätka?", "Тестовая покупка: база и корзина не изменятся.\n\nПродолжить?", "Test purchase: database and cart will not change.\n\nContinue?"),
                        Keel.T("Testost", "Тестовая покупка", "Test purchase"), MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                var test = Tellimus.Praegusest(aadress, kuller: true);
                TootlemisForm.Naita(this);
                ChekiForm.Naita(this, test, testost: true);
                return;
            }

            decimal kokku = Math.Round(Ostukorv.Kokku, 2);

            var vastus = MessageBox.Show(
                Keel.T($"Kokku: {kokku:0.00} €\n\nKinnita ost?", $"Итого: {kokku:0.00} €\n\nПодтвердить покупку?", $"Total: {kokku:0.00} €\n\nConfirm purchase?"),
                Keel.T("Maksmine", "Оплата", "Payment"), MessageBoxButtons.YesNo);
            if (vastus != DialogResult.Yes) return;

            // Hetktõmmis enne ostukorvi tühjendamist
            var tellimus = Tellimus.Praegusest(aadress, kuller: true);
            tellimus.Makstud = kokku;

            try
            {
                using var db = new ToodedContext();
                // Laoseis: kontrollime ja vähendame (kui keegi jõudis vahepeal ära osta, ei jätka)
                foreach (var rida in Ostukorv.Read)
                {
                    var ladu = db.Toodetabel.Find(rida.Toode.Id);
                    if (ladu == null || ladu.Kogus < rida.Kogus)
                    {
                        MessageBox.Show(Keel.T($"Toodet \"{rida.Toode.Toodenimetus}\" on laos ainult {ladu?.Kogus ?? 0} tk. Muuda ostukorvi.", $"Товара \"{rida.Toode.Toodenimetus}\" осталось {ladu?.Kogus ?? 0} шт. Измените корзину.", $"Only {ladu?.Kogus ?? 0} pcs of \"{rida.Toode.Toodenimetus}\" remain. Update the cart."));
                        return;
                    }
                    ladu.Kogus -= rida.Kogus;
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Keel.T("Maksmine ebaõnnestus: ", "Не удалось оплатить: ", "Payment failed: ") + ex.Message);
                return;
            }

            Ostukorv.Tuhjenda();
            UuendaOstukorvi();

            // 2) "Palun oota" 1-2 sekundit, 3) aitäh + tšekk
            TootlemisForm.Naita(this);
            ChekiForm.Naita(this, tellimus, testost: false);
        }

        private void EemaldaValitud()
        {
            if (listOstukorv.SelectedItem is not OstukorviRida rida) return;

            if (MessageBox.Show(Keel.T($"Eemaldada \"{rida.Toode.Toodenimetus}\" korvist?", $"Удалить \"{rida.Toode.Toodenimetus}\" из корзины?", $"Remove \"{rida.Toode.Toodenimetus}\" from the cart?"),
                    Keel.T("Ostukorv", "Корзина", "Cart"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Ostukorv.Eemalda(rida);
                UuendaOstukorvi();
            }
        }

        private void btnEemalda_Click(object sender, EventArgs e)
        {
            if (listOstukorv.SelectedItem == null)
            {
                MessageBox.Show(Keel.T("Vali rida, mida eemaldada.", "Выберите строку для удаления.", "Select an item to remove."));
                return;
            }
            EemaldaValitud();
        }

        private void listOstukorv_DoubleClick(object sender, EventArgs e)
        {
            EemaldaValitud();
        }

        // ------------------------------------------------------------------
        // PDF
        // ------------------------------------------------------------------
        private string LooPdf()
        {
            string failinimi = Path.Combine(Kaustad.Arved, $"Arve_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            ChekiPdf.Loo(Tellimus.Praegusest(), failinimi);
            return failinimi;
        }

        private void btnGenPDF_Click(object sender, EventArgs e)
        {
            if (Ostukorv.Read.Count == 0)
            {
                MessageBox.Show(Keel.T("Ostukorv on tühi.", "Корзина пуста.", "The cart is empty."));
                return;
            }

            try
            {
                string tee = LooPdf();
                MessageBox.Show(Keel.T($"PDF on loodud:\n{tee}", $"PDF создан:\n{tee}", $"PDF created:\n{tee}"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(Keel.T("PDF loomine ebaõnnestus: ", "Не удалось создать PDF: ", "PDF generation failed: ") + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // E-mail
        // ------------------------------------------------------------------
        private void btnSaadaEmail_Click(object sender, EventArgs e)
        {
            SaadaArve();
        }

        private void SaadaArve()
        {
            if (Ostukorv.Read.Count == 0)
            {
                MessageBox.Show(Keel.T("Ostukorv on tühi.", "Корзина пуста.", "The cart is empty."));
                return;
            }

            SaadaChek(this, Tellimus.Praegusest());
        }

        /// <summary>Saadab tšeki PDF-i e-postiga. PDF tehakse ajutisse kausta ja kustutatakse pärast.</summary>
        internal static void SaadaChek(Form omanik, Tellimus t)
        {
            bool koodisOlemas = saatjaEmail != "xxxx" && rakenduseParool != "xxxx";

            if (!KusiEmailiAndmed(omanik, !koodisOlemas, out string saatja, out string appParool, out string saaja))
                return;

            if (koodisOlemas)
            {
                saatja = saatjaEmail;
                appParool = rakenduseParool;
            }

            string pdfPath = Path.Combine(Path.GetTempPath(), $"Arve_{t.Number}_{Guid.NewGuid():N}.pdf");

            try
            {
                omanik.Cursor = Cursors.WaitCursor;
                ChekiPdf.Loo(t, pdfPath);

                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(saatja);
                    mail.To.Add(saaja);
                    mail.Subject = Keel.T($"Arve nr {t.Number}", $"Счёт № {t.Number}", $"Invoice no. {t.Number}");
                    mail.Body = Keel.T("Arve on manuses.", "Счёт во вложении.", "The invoice is attached.");
                    mail.Attachments.Add(new Attachment(pdfPath));

                    using var smtp = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(saatja, appParool),
                        EnableSsl = true
                    };

                    smtp.Send(mail);
                }

                MessageBox.Show(omanik, Keel.T("Arve saadetud!", "Счёт отправлен!", "Invoice sent!"));
            }
            catch (SmtpException ex)
            {
                MessageBox.Show(omanik, Keel.T("Saatmine ebaõnnestus. Kontrolli saatja e-maili ja app passwordit.\n\n", "Отправка не удалась. Проверьте почту отправителя и пароль приложения.\n\n", "Sending failed. Check the sender email and app password.\n\n") + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(omanik, Keel.T("Viga: ", "Ошибка: ", "Error: ") + ex.Message);
            }
            finally
            {
                omanik.Cursor = Cursors.Default;
                try { File.Delete(pdfPath); } catch { }
            }
        }

        private static bool KusiEmailiAndmed(IWin32Window omanik, bool kysiSaatja, out string saatja, out string appParool, out string saaja)
        {
            saatja = appParool = saaja = "";

            using Form aken = new Form();
            aken.Text = Keel.T("Saada arve e-mailiga", "Отправить счёт по почте", "Email invoice");
            aken.StartPosition = FormStartPosition.CenterParent;
            aken.FormBorderStyle = FormBorderStyle.FixedDialog;
            aken.MaximizeBox = false;
            aken.MinimizeBox = false;

            TextBox txtSaatja = new TextBox { Width = 310 };
            TextBox txtParool = new TextBox { Width = 310, UseSystemPasswordChar = true };
            TextBox txtSaaja = new TextBox { Width = 310 };

            int y = 15;

            if (kysiSaatja)
            {
                aken.Controls.Add(new Label { Text = Keel.T("Saatja e-mail (Gmail):", "Почта отправителя (Gmail):", "Sender email (Gmail):"), Location = new Point(15, y), AutoSize = true });
                txtSaatja.Location = new Point(15, y + 20);
                aken.Controls.Add(txtSaatja);
                y += 55;

                aken.Controls.Add(new Label { Text = Keel.T("App password (rakenduse parool):", "Пароль приложения:", "App password:"), Location = new Point(15, y), AutoSize = true });
                txtParool.Location = new Point(15, y + 20);
                aken.Controls.Add(txtParool);
                y += 55;
            }

            aken.Controls.Add(new Label { Text = Keel.T("Saaja e-mail:", "Почта получателя:", "Recipient email:"), Location = new Point(15, y), AutoSize = true });
            txtSaaja.Location = new Point(15, y + 20);
            aken.Controls.Add(txtSaaja);
            y += 60;

            Button btnSaada = new Button { Text = Keel.T("Saada", "Отправить", "Send"), Location = new Point(15, y), Width = 150 };
            Button btnLoobu = new Button { Text = Keel.T("Loobu", "Отмена", "Cancel"), Location = new Point(175, y), Width = 150, DialogResult = DialogResult.Cancel };
            aken.Controls.Add(btnSaada);
            aken.Controls.Add(btnLoobu);
            aken.ClientSize = new Size(340, y + 50);

            btnSaada.Click += (s, args) =>
            {
                if (!OnEmail(txtSaaja.Text) || (kysiSaatja && !OnEmail(txtSaatja.Text)))
                {
                    MessageBox.Show(Keel.T("Sisesta korrektne e-mail!", "Введите корректную почту!", "Enter a valid email!"));
                    return;
                }

                if (kysiSaatja && string.IsNullOrWhiteSpace(txtParool.Text))
                {
                    MessageBox.Show(Keel.T("Sisesta app password!", "Введите пароль приложения!", "Enter the app password!"));
                    return;
                }

                aken.DialogResult = DialogResult.OK;
            };

            aken.AcceptButton = btnSaada;
            aken.CancelButton = btnLoobu;

            if (aken.ShowDialog(omanik) != DialogResult.OK)
                return false;

            saatja = txtSaatja.Text.Trim();
            appParool = txtParool.Text.Replace(" ", "");
            saaja = txtSaaja.Text.Trim();
            return true;
        }

        private static bool OnEmail(string tekst)
        {
            try
            {
                return new MailAddress(tekst.Trim()).Address == tekst.Trim();
            }
            catch
            {
                return false;
            }
        }
    }
}
