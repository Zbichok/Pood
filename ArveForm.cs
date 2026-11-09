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

        private const string Kaust = "Arved";

        public ArveForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Stiil.Rakenda(this);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Admin ei tohi arvet muuta – ainult vaatamine.
            if (AktiivneKasutaja.OnAdmin)
            {
                btnEemalda.Enabled = false;
                btnGenPDF.Enabled = false;
                btnSaadaEmail.Enabled = false;
                lblMarkus.Text = "Admin saab arvet ainult vaadata, mitte muuta.";
            }

            UuendaOstukorvi();
        }

        private void UuendaOstukorvi()
        {
            listOstukorv.Items.Clear();
            foreach (var rida in Ostukorv.Read)
                listOstukorv.Items.Add(rida);

            lblKokku.Text = $"Kokku: {Ostukorv.Kokku:0.00} €";
        }

        private void EemaldaValitud()
        {
            if (AktiivneKasutaja.OnAdmin) return;
            if (listOstukorv.SelectedItem is not OstukorviRida rida) return;

            if (MessageBox.Show($"Eemaldada \"{rida.Toode.Toodenimetus}\" korvist?",
                    "Ostukorv", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Ostukorv.Eemalda(rida);
                UuendaOstukorvi();
            }
        }

        private void btnEemalda_Click(object sender, EventArgs e)
        {
            if (listOstukorv.SelectedItem == null)
            {
                MessageBox.Show("Vali rida, mida eemaldada.");
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
            Directory.CreateDirectory(Kaust);

            string failinimi = Path.Combine(Kaust, $"Arve_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont("Verdana", 12);

            gfx.DrawString("Arve", new XFont("Verdana", 20), XBrushes.Black,
                new XRect(0, 20, page.Width, 40), XStringFormats.TopCenter);

            gfx.DrawString($"Kuupäev: {DateTime.Now:dd.MM.yyyy HH:mm}", font, XBrushes.Black,
                new XRect(20, 55, page.Width - 40, 20), XStringFormats.TopLeft);

            gfx.DrawString($"Klient: {AktiivneKasutaja.Nimi}", font, XBrushes.Black,
                new XRect(20, 75, page.Width - 40, 20), XStringFormats.TopLeft);

            double y = 110;
            foreach (var item in Ostukorv.Read)
            {
                if (y > page.Height - 80)
                {
                    gfx.Dispose();
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }

                gfx.DrawString(item.ToString(), font, XBrushes.Black,
                    new XRect(20, y, page.Width - 40, 20), XStringFormats.TopLeft);
                y += 30;
            }

            gfx.DrawString($"Kokku: {Ostukorv.Kokku:0.00} €",
                new XFont("Verdana", 14), XBrushes.Black,
                new XRect(20, y + 20, page.Width - 40, 20), XStringFormats.TopLeft);

            gfx.Dispose();
            document.Save(failinimi);
            return failinimi;
        }

        private void btnGenPDF_Click(object sender, EventArgs e)
        {
            if (AktiivneKasutaja.OnAdmin) return;

            if (Ostukorv.Read.Count == 0)
            {
                MessageBox.Show("Ostukorv on tühi.");
                return;
            }

            try
            {
                string tee = LooPdf();
                MessageBox.Show($"PDF on loodud:\n{tee}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("PDF loomine ebaõnnestus: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // E-mail
        // ------------------------------------------------------------------
        private void btnSaadaEmail_Click(object sender, EventArgs e)
        {
            if (AktiivneKasutaja.OnAdmin) return;

            if (Ostukorv.Read.Count == 0)
            {
                MessageBox.Show("Ostukorv on tühi.");
                return;
            }

            bool koodisOlemas = saatjaEmail != "xxxx" && rakenduseParool != "xxxx";

            if (!KusiEmailiAndmed(!koodisOlemas, out string saatja, out string appParool, out string saaja))
                return;

            if (koodisOlemas)
            {
                saatja = saatjaEmail;
                appParool = rakenduseParool;
            }

            try
            {
                Cursor = Cursors.WaitCursor;

                // Saadame alati praeguse ostukorvi PDF-i
                string pdfPath = LooPdf();

                using var mail = new MailMessage();
                mail.From = new MailAddress(saatja);
                mail.To.Add(saaja);
                mail.Subject = "Arve";
                mail.Body = "Arve on manuses.";
                mail.Attachments.Add(new Attachment(pdfPath));

                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(saatja, appParool),
                    EnableSsl = true
                };

                smtp.Send(mail);

                MessageBox.Show("Arve saadetud!");
            }
            catch (SmtpException ex)
            {
                MessageBox.Show("Saatmine ebaõnnestus. Kontrolli saatja e-maili ja app passwordit.\n\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Viga: " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private bool KusiEmailiAndmed(bool kysiSaatja, out string saatja, out string appParool, out string saaja)
        {
            saatja = appParool = saaja = "";

            using Form aken = new Form();
            aken.Text = "Saada arve e-mailiga";
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
                aken.Controls.Add(new Label { Text = "Saatja e-mail (Gmail):", Location = new Point(15, y), AutoSize = true });
                txtSaatja.Location = new Point(15, y + 20);
                aken.Controls.Add(txtSaatja);
                y += 55;

                aken.Controls.Add(new Label { Text = "App password (rakenduse parool):", Location = new Point(15, y), AutoSize = true });
                txtParool.Location = new Point(15, y + 20);
                aken.Controls.Add(txtParool);
                y += 55;
            }

            aken.Controls.Add(new Label { Text = "Saaja e-mail:", Location = new Point(15, y), AutoSize = true });
            txtSaaja.Location = new Point(15, y + 20);
            aken.Controls.Add(txtSaaja);
            y += 60;

            Button btnSaada = new Button { Text = "Saada", Location = new Point(15, y), Width = 150 };
            Button btnLoobu = new Button { Text = "Loobu", Location = new Point(175, y), Width = 150, DialogResult = DialogResult.Cancel };
            aken.Controls.Add(btnSaada);
            aken.Controls.Add(btnLoobu);
            aken.ClientSize = new Size(340, y + 50);

            btnSaada.Click += (s, args) =>
            {
                if (!OnEmail(txtSaaja.Text) || (kysiSaatja && !OnEmail(txtSaatja.Text)))
                {
                    MessageBox.Show("Sisesta korrektne e-mail!");
                    return;
                }

                if (kysiSaatja && string.IsNullOrWhiteSpace(txtParool.Text))
                {
                    MessageBox.Show("Sisesta app password!");
                    return;
                }

                aken.DialogResult = DialogResult.OK;
            };

            aken.AcceptButton = btnSaada;
            aken.CancelButton = btnLoobu;

            if (aken.ShowDialog(this) != DialogResult.OK)
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
