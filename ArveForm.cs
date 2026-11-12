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

        // Rulli boonuste jaoks (luuakse koodis, vt LisaBoonusKontrollid)
        private readonly Label lblKokkuvote = new Label();
        private readonly Label lblSaldo = new Label();
        private readonly CheckBox chkSaldo = new CheckBox();
        private readonly Button btnMaksa = new Button();

        public ArveForm()
        {
            InitializeComponent();
            LisaBoonusKontrollid();
            StartPosition = FormStartPosition.CenterScreen;
            Stiil.Rakenda(this);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Admin: ostud on ainult testostud (arve PDF + e-post), andmebaasi ei muudeta.
            if (AktiivneKasutaja.OnAdmin)
            {
                btnMaksa.Text = "Testost";
                chkSaldo.Visible = false;
                lblMarkus.Text = "Admin: testost ei muuda andmebaasi, saldot ega ostukorvi.";
            }

            UuendaOstukorvi();
        }

        private void LisaBoonusKontrollid()
        {
            ClientSize = new Size(670, 480);

            lblKokkuvote.AutoSize = false;
            lblKokkuvote.Location = new Point(20, 322);
            lblKokkuvote.Size = new Size(440, 84);

            lblKokku.Location = new Point(20, 410);
            lblMarkus.Location = new Point(20, 445);

            lblSaldo.AutoSize = true;
            lblSaldo.Location = new Point(480, 190);

            chkSaldo.Text = "Kasuta saldot";
            chkSaldo.Checked = true;
            chkSaldo.AutoSize = true;
            chkSaldo.Location = new Point(480, 215);

            btnMaksa.Text = "Maksa";
            btnMaksa.Location = new Point(480, 245);
            btnMaksa.Size = new Size(160, 35);
            btnMaksa.Click += btnMaksa_Click;

            Controls.Add(lblKokkuvote);
            Controls.Add(lblSaldo);
            Controls.Add(chkSaldo);
            Controls.Add(btnMaksa);
        }

        private void UuendaOstukorvi()
        {
            listOstukorv.Items.Clear();
            foreach (var rida in Ostukorv.Read)
                listOstukorv.Items.Add(rida);

            lblKokkuvote.Text = string.Join(Environment.NewLine, Ostukorv.Kokkuvote());
            lblKokku.Text = $"Kokku: {Ostukorv.Kokku:0.00} €";
            lblSaldo.Text = $"Saldo: {AktiivneKasutaja.Saldo:0.00} €";
        }

        // ------------------------------------------------------------------
        // Maksmine: saldo, boonused kasutatakse ära, ostukorv tühjendatakse
        // ------------------------------------------------------------------
        private void btnMaksa_Click(object sender, EventArgs e)
        {
            if (Ostukorv.Read.Count == 0)
            {
                MessageBox.Show("Ostukorv on tühi.");
                return;
            }

            // 1) Aadress (ei salvestata kuhugi, elab ainult selle ostu tšekis)
            string? aadress = AadressForm.Kysi(this);
            if (aadress == null) return;

            if (AktiivneKasutaja.OnAdmin)
            {
                // Testost: andmebaasi, saldot ega ostukorvi ei muudeta
                if (MessageBox.Show(
                        "Testost: andmebaasi, saldot ega ostukorvi ei muudeta.\n\nJätka?",
                        "Testost", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                var test = Tellimus.Praegusest(aadress, kuller: true);
                TootlemisForm.Naita(this);
                ChekiForm.Naita(this, test, testost: true);
                return;
            }

            decimal kokku = Math.Round((decimal)Ostukorv.Kokku, 2);
            decimal saldoKasutus = chkSaldo.Checked ? Math.Min(AktiivneKasutaja.Saldo, kokku) : 0m;
            decimal makstakse = kokku - saldoKasutus;

            var vastus = MessageBox.Show(
                $"Kokku: {kokku:0.00} €\nSaldost: {saldoKasutus:0.00} €\nMakstakse: {makstakse:0.00} €\n\nKinnita ost?",
                "Maksmine", MessageBoxButtons.YesNo);
            if (vastus != DialogResult.Yes) return;

            // Hetktõmmis enne ostukorvi tühjendamist ja boonuste kasutamist
            var tellimus = Tellimus.Praegusest(aadress, kuller: true);
            tellimus.SaldoKasutus = saldoKasutus;
            tellimus.Makstud = makstakse;

            try
            {
                using var db = new ToodedContext();
                var k = db.Kasutajad.FirstOrDefault(x => x.Email == AktiivneKasutaja.Email);
                if (k == null)
                {
                    MessageBox.Show("Kasutajat ei leitud.");
                    return;
                }

                k.Saldo -= saldoKasutus;
                k.Soodustus = 0;                       // soodustus on kasutatud
                k.TasutaKohaletoimetus = false;        // tasuta kohaletoimetus on kasutatud
                if (Ostukorv.TasutaRida != null)
                    k.TasutaToodeId = null;            // tasuta toode on kasutatud
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Maksmine ebaõnnestus: " + ex.Message);
                return;
            }

            Ostukorv.Tuhjenda();
            AktiivneKasutaja.LaeBoonused();
            UuendaOstukorvi();

            // 2) "Palun oota" 1-2 sekundit, 3) aitäh + tšekk
            TootlemisForm.Naita(this);
            ChekiForm.Naita(this, tellimus, testost: false);
        }

        private void EemaldaValitud()
        {
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
            string failinimi = Path.Combine(Kaust, $"Arve_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            ChekiPdf.Loo(Tellimus.Praegusest(), failinimi);
            return failinimi;
        }

        private void btnGenPDF_Click(object sender, EventArgs e)
        {
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
            SaadaArve();
        }

        private void SaadaArve()
        {
            if (Ostukorv.Read.Count == 0)
            {
                MessageBox.Show("Ostukorv on tühi.");
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
                    mail.Subject = $"Arve nr {t.Number}";
                    mail.Body = "Arve on manuses.";
                    mail.Attachments.Add(new Attachment(pdfPath));

                    using var smtp = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(saatja, appParool),
                        EnableSsl = true
                    };

                    smtp.Send(mail);
                }

                MessageBox.Show(omanik, "Arve saadetud!");
            }
            catch (SmtpException ex)
            {
                MessageBox.Show(omanik, "Saatmine ebaõnnestus. Kontrolli saatja e-maili ja app passwordit.\n\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(omanik, "Viga: " + ex.Message);
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
