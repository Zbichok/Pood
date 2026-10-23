using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
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
        private readonly ToodedContext _db = new ToodedContext();
        List<OstukorviRida> ostukorv = new List<OstukorviRida>();

        public Toode SelectedToode => listTooted.SelectedItem as Toode;

        public ArveForm()
        {
            InitializeComponent();
            LaeTooted();
            ArvutaKoguSumma();
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        private void LaeTooted()
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            listTooted.DataSource = _db.Toodetabel
                .Include(t => t.Kategooria)
                .ToList();
            listTooted.DisplayMember = "Toodenimetus";
        }

        private void NaitaToodet(Toode toode)
        {
            toodeNimi.Text = toode.Toodenimetus;
            toodeKirjeldus.Text = $"{toode.Hind:0.00} € | {toode.Kogus} tk | {toode.Kategooria?.Kategooria_nimetus}";

            pictureBox1.Image?.Dispose();
            pictureBox1.Image = Pildid.Lae(toode.Pilt);
        }

        private void listTooted_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedToode != null)
                NaitaToodet(SelectedToode);
        }

        private void btnLisaKorvi_Click(object sender, EventArgs e)
        {
            if (SelectedToode == null)
            {
                MessageBox.Show("Vali toode enne ostukorvi lisamist.");
                return;
            }

            string kogusStr = Microsoft.VisualBasic.Interaction.InputBox(
                "Sisesta kogus", "Kogus", "1");

            if (!int.TryParse(kogusStr, out int kogus) || kogus <= 0)
            {
                MessageBox.Show("Vale kogus.");
                return;
            }

            var olemas = ostukorv.FirstOrDefault(r => r.Toode.Id == SelectedToode.Id);
            int kokkuKorvis = (olemas?.Kogus ?? 0) + kogus;

            if (kokkuKorvis > SelectedToode.Kogus)
            {
                MessageBox.Show($"Laos on ainult {SelectedToode.Kogus} tk.");
                return;
            }

            if (olemas != null)
            {
                olemas.Kogus = kokkuKorvis;
            }
            else
            {
                ostukorv.Add(new OstukorviRida
                {
                    Toode = SelectedToode,
                    Kogus = kogus
                });
            }

            UuendaOstukorvi();
        }

        private void listOstukorv_DoubleClick(object sender, EventArgs e)
        {
            if (listOstukorv.SelectedItem is not OstukorviRida rida) return;

            if (MessageBox.Show($"Eemaldada \"{rida.Toode.Toodenimetus}\" korvist?",
                    "Ostukorv", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ostukorv.Remove(rida);
                UuendaOstukorvi();
            }
        }

        private void UuendaOstukorvi()
        {
            listOstukorv.Items.Clear();
            foreach (var rida in ostukorv)
                listOstukorv.Items.Add(rida);

            ArvutaKoguSumma();
        }

        private void ArvutaKoguSumma()
        {
            float kokku = ostukorv.Sum(r => r.Summa);

            lblKokku.Text = $"Kokku: {kokku:0.00} €";
        }

        private void btnGenPDF_Click(object sender, EventArgs e)
        {
            if (ostukorv.Count == 0)
            {
                MessageBox.Show("Ostukorv on tühi.");
                return;
            }

            Directory.CreateDirectory("Arved");

            string failinimi = $"Arved/Arve_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont("Verdana", 12);

            gfx.DrawString("Arve", new XFont("Verdana", 20), XBrushes.Black,
                new XRect(0, 20, page.Width, 40), XStringFormats.TopCenter);

            gfx.DrawString($"Kuupäev: {DateTime.Now:dd.MM.yyyy HH:mm}", font, XBrushes.Black,
                new XRect(20, 55, page.Width - 40, 20), XStringFormats.TopLeft);

            int y = 90;
            foreach (var item in ostukorv)
            {
                gfx.DrawString(item.ToString(), font, XBrushes.Black,
                    new XRect(20, y, page.Width - 40, 20), XStringFormats.TopLeft);
                y += 30;
            }

            gfx.DrawString($"Kokku: {ostukorv.Sum(r => r.Summa):0.00} €",
                new XFont("Verdana", 14), XBrushes.Black,
                new XRect(20, y + 20, page.Width, 20), XStringFormats.TopLeft);

            document.Save(failinimi);

            MessageBox.Show($"PDF on loodud:\n{failinimi}");
        }

        private void btnSaadaEmail_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists("Arved") || Directory.GetFiles("Arved").Length == 0)
            {
                MessageBox.Show("Loo enne PDF arve.");
                return;
            }

            if (!KusiEmailiAndmed(out string saatja, out string appParool, out string saaja))
                return;

            string pdfPath = Directory.GetFiles("Arved")
                .OrderByDescending(f => f)
                .First();

            try
            {
                Cursor = Cursors.WaitCursor;

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

        private bool KusiEmailiAndmed(out string saatja, out string appParool, out string saaja)
        {
            saatja = appParool = saaja = "";

            using Form aken = new Form();
            aken.Text = "Saada arve e-mailiga";
            aken.StartPosition = FormStartPosition.CenterParent;
            aken.ClientSize = new Size(340, 260);
            aken.FormBorderStyle = FormBorderStyle.FixedDialog;
            aken.MaximizeBox = false;
            aken.MinimizeBox = false;

            Label lblSaatja = new Label { Text = "Saatja e-mail (Gmail):", Location = new Point(15, 15), AutoSize = true };
            TextBox txtSaatja = new TextBox { Location = new Point(15, 35), Width = 310 };

            Label lblParool = new Label { Text = "App password:", Location = new Point(15, 70), AutoSize = true };
            TextBox txtParool = new TextBox { Location = new Point(15, 90), Width = 310, UseSystemPasswordChar = true };

            Label lblSaaja = new Label { Text = "Saaja e-mail:", Location = new Point(15, 125), AutoSize = true };
            TextBox txtSaaja = new TextBox { Location = new Point(15, 145), Width = 310 };

            Button btnSaada = new Button { Text = "Saada", Location = new Point(15, 200), Width = 150 };
            Button btnLoobu = new Button { Text = "Loobu", Location = new Point(175, 200), Width = 150, DialogResult = DialogResult.Cancel };

            btnSaada.Click += (s, args) =>
            {
                if (!OnEmail(txtSaatja.Text) || !OnEmail(txtSaaja.Text))
                {
                    MessageBox.Show("Sisesta korrektne saatja ja saaja e-mail!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtParool.Text))
                {
                    MessageBox.Show("Sisesta app password!");
                    return;
                }

                aken.DialogResult = DialogResult.OK;
            };

            aken.Controls.AddRange(new Control[] { lblSaatja, txtSaatja, lblParool, txtParool, lblSaaja, txtSaaja, btnSaada, btnLoobu });
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
