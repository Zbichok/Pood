namespace Tooted
{
    /// <summary>Küsib kohaletoimetuse aadressi. Aadressi EI salvestata kuhugi.</summary>
    public sealed class AadressForm : Form
    {
        private readonly TextBox txtTanav = new TextBox { Width = 330, MaxLength = 80 };
        private readonly TextBox txtKorter = new TextBox { Width = 150, MaxLength = 20 };
        private readonly TextBox txtLinn = new TextBox { Width = 330, MaxLength = 50 };

        public string? Aadress { get; private set; }

        private AadressForm()
        {
            Text = "Kohaletoimetuse aadress";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(360, 270);

            int y = 15;
            Controls.Add(new Label { Text = "Tänav ja maja nr:", Location = new Point(15, y), AutoSize = true });
            txtTanav.Location = new Point(15, y + 20);
            Controls.Add(txtTanav);
            y += 55;

            Controls.Add(new Label { Text = "Korter (kui on):", Location = new Point(15, y), AutoSize = true });
            txtKorter.Location = new Point(15, y + 20);
            Controls.Add(txtKorter);
            y += 55;

            Controls.Add(new Label { Text = "Linn:", Location = new Point(15, y), AutoSize = true });
            txtLinn.Location = new Point(15, y + 20);
            Controls.Add(txtLinn);
            y += 55;

            Controls.Add(new Label
            {
                Text = "Aadressi ei salvestata – seda kasutatakse ainult selle ostu jaoks.",
                ForeColor = Color.Gray,
                AutoSize = false,
                Location = new Point(15, y),
                Size = new Size(330, 34)
            });
            y += 40;

            var btnOk = new Button { Text = "Telli", Location = new Point(15, y), Size = new Size(160, 32) };
            var btnLoobu = new Button
            {
                Text = "Loobu",
                Location = new Point(185, y),
                Size = new Size(160, 32),
                DialogResult = DialogResult.Cancel
            };
            Controls.Add(btnOk);
            Controls.Add(btnLoobu);
            ClientSize = new Size(360, y + 47);

            btnOk.Click += (s, e) =>
            {
                string tanav = txtTanav.Text.Trim();
                string linn = txtLinn.Text.Trim();
                string korter = txtKorter.Text.Trim();

                if (tanav.Length < 3)
                {
                    MessageBox.Show(this, "Sisesta tänav ja maja number.");
                    txtTanav.Focus();
                    return;
                }

                if (linn.Length < 2)
                {
                    MessageBox.Show(this, "Sisesta linn.");
                    txtLinn.Focus();
                    return;
                }

                Aadress = korter.Length > 0
                    ? $"{tanav}, korter {korter}, {linn}"
                    : $"{tanav}, {linn}";
                DialogResult = DialogResult.OK;
            };

            AcceptButton = btnOk;
            CancelButton = btnLoobu;
            Stiil.Rakenda(this);
        }

        /// <summary>Tagastab aadressi või null, kui kasutaja loobus.</summary>
        public static string? Kysi(IWin32Window omanik)
        {
            using var f = new AadressForm();
            return f.ShowDialog(omanik) == DialogResult.OK ? f.Aadress : null;
        }
    }

    /// <summary>"Palun oota" aken – tehislik laadimine 1-2 sekundit.</summary>
    public sealed class TootlemisForm : Form
    {
        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();

        private TootlemisForm()
        {
            Text = "Tellimus";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ControlBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(360, 120);

            Controls.Add(new Label
            {
                Text = "Palun oota...",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 12),
                Size = new Size(340, 32)
            });

            Controls.Add(new Label
            {
                Text = "Töötleme tellimust ja otsime kullerit",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 46),
                Size = new Size(340, 22)
            });

            Controls.Add(new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Location = new Point(40, 80),
                Size = new Size(280, 18)
            });

            Stiil.Rakenda(this);

            _timer.Interval = Random.Shared.Next(1000, 2001);
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                Close();
            };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _timer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _timer.Dispose();
            base.Dispose(disposing);
        }

        public static void Naita(IWin32Window omanik)
        {
            using var f = new TootlemisForm();
            f.ShowDialog(omanik);
        }
    }

    /// <summary>"Aitäh ostu eest" + kulleri aeg + tšekk (PDF / e-post).</summary>
    public sealed class ChekiForm : Form
    {
        private readonly Tellimus _t;

        private ChekiForm(Tellimus t, bool testost)
        {
            _t = t;

            Text = testost ? "Testost tehtud" : "Ost tehtud";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(460, 490);

            Controls.Add(new Label
            {
                Text = testost ? "Testost tehtud – aitäh!" : "Aitäh ostu eest!",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                AutoSize = false,
                Location = new Point(20, 15),
                Size = new Size(420, 34)
            });

            Controls.Add(new Label
            {
                Text = t.KullerTekst + (string.IsNullOrWhiteSpace(t.Aadress)
                    ? ""
                    : Environment.NewLine + "Aadress: " + t.Aadress),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                AutoSize = false,
                Location = new Point(20, 52),
                Size = new Size(420, 50)
            });

            Controls.Add(new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10F),
                Text = t.Tekst(),
                Location = new Point(20, 110),
                Size = new Size(420, 310),
                TabStop = false
            });

            var btnPdf = new Button { Text = "Salvesta PDF", Location = new Point(20, 435), Size = new Size(130, 35) };
            var btnPost = new Button { Text = "Saada e-postile", Location = new Point(160, 435), Size = new Size(150, 35) };
            var btnSulge = new Button { Text = "Sulge", Location = new Point(320, 435), Size = new Size(120, 35), DialogResult = DialogResult.OK };

            btnPdf.Click += (s, e) => SalvestaPdf();
            btnPost.Click += (s, e) => ArveForm.SaadaChek(this, _t);

            Controls.Add(btnPdf);
            Controls.Add(btnPost);
            Controls.Add(btnSulge);

            AcceptButton = btnSulge;
            CancelButton = btnSulge;
            Stiil.Rakenda(this);
        }

        private void SalvestaPdf()
        {
            using var dlg = new SaveFileDialog
            {
                Title = "Salvesta arve",
                Filter = "PDF (*.pdf)|*.pdf",
                DefaultExt = "pdf",
                FileName = $"Arve_{_t.Number}.pdf"
            };

            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                ChekiPdf.Loo(_t, dlg.FileName);
                MessageBox.Show(this, $"PDF on salvestatud:\n{dlg.FileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "PDF salvestamine ebaõnnestus: " + ex.Message);
            }
        }

        public static void Naita(IWin32Window omanik, Tellimus t, bool testost)
        {
            using var f = new ChekiForm(t, testost);
            f.ShowDialog(omanik);
        }
    }
}
