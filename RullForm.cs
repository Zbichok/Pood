using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Tooted
{
    /// <summary>
    /// Päevane rull (nagu CS:GO kast): kord päevas saab keerata ja võita boonuse.
    /// Võitja valitakse kaalutud juhuslikkusega ENNE animatsiooni ja salvestatakse kohe andmebaasi,
    /// animatsioon peatub lihtsalt võidu peal. Mida haruldasem preemia, seda suurem efekt.
    /// </summary>
    public class RullForm : Form
    {
        private enum PreemiaTyyp { Soodustus, Saldo, Kohaletoimetus, TasutaToode, Konto }
        private enum Haruldus { Tavaline, Haruldane, Eepiline, Legendaarne, Risk }
        private enum Faas { Ootel, Keerab, Efekt }

        private sealed record Preemia(string Nimi, string Alamtekst, PreemiaTyyp Tyyp,
                                      decimal Vaartus, int Kaal, Haruldus Aste)
        {
            public Color Varv => Aste switch
            {
                Haruldus.Tavaline => Color.FromArgb(150, 160, 185),
                Haruldus.Haruldane => Color.FromArgb(60, 130, 245),
                Haruldus.Eepiline => Color.FromArgb(175, 80, 235),
                Haruldus.Legendaarne => Color.FromArgb(255, 200, 40),
                _ => Color.Crimson
            };
        }

        private static string HarulduseNimi(Haruldus h) => h switch
        {
            Haruldus.Tavaline => "TAVALINE",
            Haruldus.Haruldane => "HARULDANE",
            Haruldus.Eepiline => "EEPILINE",
            Haruldus.Legendaarne => "LEGENDAARNE",
            _ => "RISK"
        };

        // Kaal = võimalus: mida suurem arv, seda sagedamini tuleb. Kokku 106.
        private static readonly Preemia[] Preemiad =
        {
            new("-5% soodustus",   "järgmisele ostule", PreemiaTyyp.Soodustus,      5,  25, Haruldus.Tavaline),
            new("-10% soodustus",  "järgmisele ostule", PreemiaTyyp.Soodustus,      10, 15, Haruldus.Haruldane),
            new("-20% soodustus",  "järgmisele ostule", PreemiaTyyp.Soodustus,      20, 4,  Haruldus.Eepiline),
            new("+1,00 € saldo",   "kontole",           PreemiaTyyp.Saldo,          1,  25, Haruldus.Tavaline),
            new("+5,00 € saldo",   "kontole",           PreemiaTyyp.Saldo,          5,  12, Haruldus.Haruldane),
            new("+20,00 € saldo",  "kontole",           PreemiaTyyp.Saldo,          20, 2,  Haruldus.Legendaarne),
            new("Tasuta kohaletoimetus", "järgmisele ostule", PreemiaTyyp.Kohaletoimetus, 0, 14, Haruldus.Haruldane),
            new("Tasuta toode",    "juhuslik toode poest", PreemiaTyyp.TasutaToode, 0,  6,  Haruldus.Eepiline),
            new("Konto kustutamine", "hüvasti!",        PreemiaTyyp.Konto,          0,  3,  Haruldus.Risk),
        };

        private static readonly Color[] Kuld = { Color.Gold, Color.Orange, Color.Yellow, Color.White };
        private static readonly Color[] Lilla = { Color.MediumPurple, Color.Violet, Color.HotPink, Color.White };
        private static readonly Color[] Sinine = { Color.DodgerBlue, Color.LightSkyBlue, Color.White };
        private static readonly Color[] Punane = { Color.Crimson, Color.OrangeRed, Color.DarkRed, Color.Black };

        private const int SpinMs = 6000;
        private const int VoiduIndeks = 52;     // mitmendale plaadile rull peatub
        private const int PlaatideArv = 64;

        private readonly Lent lent = new Lent();
        private readonly Button btnKeera = new Button();
        private readonly Label lblInfo = new Label();
        private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 15 };
        private readonly Stopwatch kell = new Stopwatch();
        private readonly Stopwatch efektKell = new Stopwatch();
        private readonly Random rnd = new Random();

        private Faas faas = Faas.Ootel;
        private double sihtNihe;
        private Preemia? voit;
        private string voiduTekst = "";
        private int efektMs;
        private Color taust0;

        private bool Keerab => faas != Faas.Ootel;

        public RullForm()
        {
            Text = "Päevane rull";
            ClientSize = new Size(660, 320);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            var lblPealkiri = new Label
            {
                Text = "Päevane rull",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 12),
                AutoSize = true
            };

            lent.Location = new Point(10, 55);
            lent.Size = new Size(640, 170);

            lblInfo.Location = new Point(20, 238);
            lblInfo.Size = new Size(620, 24);
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblInfo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            btnKeera.Text = "Keeruta!";
            btnKeera.Location = new Point(230, 268);
            btnKeera.Size = new Size(200, 40);
            btnKeera.Click += btnKeera_Click;

            Controls.Add(lblPealkiri);
            Controls.Add(lent);
            Controls.Add(lblInfo);
            Controls.Add(btnKeera);

            Stiil.Rakenda(this);
            taust0 = BackColor;

            // Ootel olekus näitame preemiate valikut
            var ootel = new List<Preemia>();
            for (int i = 0; i < 20; i++) ootel.Add(Preemiad[i % Preemiad.Length]);
            lent.Plaadid = ootel;

            AktiivneKasutaja.LaeBoonused();
            UuendaOlek();

            timer.Tick += Tick;
        }

        private void UuendaOlek()
        {
            lblInfo.ForeColor = ForeColor;

            if (!AktiivneKasutaja.VoibKeerata)
            {
                btnKeera.Enabled = false;
                lblInfo.Text = "Täna oled juba keeranud. Tule homme tagasi!";
            }
            else
            {
                btnKeera.Enabled = true;
                lblInfo.Text = "Üks keeramine päevas. Õnn kaasa!";
            }
        }

        private Preemia ValiVoit()
        {
            int summa = Preemiad.Sum(p => p.Kaal);
            int r = rnd.Next(summa);
            foreach (var p in Preemiad)
            {
                if (r < p.Kaal) return p;
                r -= p.Kaal;
            }
            return Preemiad[0];
        }

        private void btnKeera_Click(object? sender, EventArgs e)
        {
            if (Keerab) return;

            try
            {
                using var db = new ToodedContext();
                var k = db.Kasutajad.FirstOrDefault(x => x.Email == AktiivneKasutaja.Email);
                if (k == null)
                {
                    MessageBox.Show("Kasutajat ei leitud.");
                    return;
                }

                // Kontroll ka andmebaasist (mitte ainult mälust)
                if (k.ViimanePoore?.Date == DateTime.Today)
                {
                    MessageBox.Show("Täna oled juba keeranud. Tule homme tagasi!");
                    AktiivneKasutaja.LaeBoonused();
                    UuendaOlek();
                    return;
                }

                voit = ValiVoit();
                k.ViimanePoore = DateTime.Now;
                voiduTekst = RakendaPreemia(db, k, voit);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Viga: " + ex.Message);
                return;
            }

            AktiivneKasutaja.LaeBoonused();

            // Plaatide lint: algul sama, mis ootel, lõpus sama võitja
            var plaadid = new List<Preemia>(lent.Plaadid.Take(6));
            while (plaadid.Count < PlaatideArv) plaadid.Add(ValiVoit());
            plaadid[VoiduIndeks] = voit;
            lent.Plaadid = plaadid;
            lent.Puhasta();
            lent.Nihe = 0;

            double jitter = (rnd.NextDouble() - 0.5) * Lent.PlaadiLaius * 0.7;
            sihtNihe = VoiduIndeks * Lent.PlaadiLaius + Lent.PlaadiLaius / 2.0 - lent.Width / 2.0 + jitter;

            faas = Faas.Keerab;
            btnKeera.Enabled = false;
            lblInfo.ForeColor = ForeColor;
            lblInfo.Text = "Keerutab...";
            kell.Restart();
            timer.Start();
        }

        /// <summary>Kirjutab boonuse kasutaja külge. Tagastab teksti, mida näidata.</summary>
        private string RakendaPreemia(ToodedContext db, Kasutaja k, Preemia p)
        {
            switch (p.Tyyp)
            {
                case PreemiaTyyp.Soodustus:
                    k.Soodustus = Math.Max(k.Soodustus, (int)p.Vaartus);
                    return $"Järgmisel ostul on sul {k.Soodustus}% soodustus!";

                case PreemiaTyyp.Saldo:
                    k.Saldo += p.Vaartus;
                    return $"Saldole lisati {p.Vaartus:0.00} €. Uus saldo: {k.Saldo:0.00} €.";

                case PreemiaTyyp.Kohaletoimetus:
                    k.TasutaKohaletoimetus = true;
                    return "Järgmise ostu kohaletoimetus on tasuta!";

                case PreemiaTyyp.TasutaToode:
                    var tooted = db.Toodetabel.Where(t => t.Kogus > 0).ToList();
                    if (tooted.Count == 0)
                    {
                        k.Saldo += 2m;
                        return "Poes pole tooteid, nii et saad hoopis 2,00 € saldosse.";
                    }
                    var toode = tooted[rnd.Next(tooted.Count)];
                    k.TasutaToodeId = toode.Id;
                    return $"Võitsid tasuta toote: {toode.Toodenimetus}!\nLisa see ostukorvi ja see on tasuta.";

                default:
                    return "";   // Konto: päris midagi ei kustutata, programm lihtsalt sulgub
            }
        }

        // ---------- Animatsioon ----------
        private void Tick(object? sender, EventArgs e)
        {
            if (faas == Faas.Keerab)
            {
                double t = Math.Min(1.0, kell.ElapsedMilliseconds / (double)SpinMs);
                double sujuv = 1 - Math.Pow(1 - t, 4);      // aeglustub lõpu poole
                lent.Nihe = sihtNihe * sujuv;
                lent.Invalidate();

                if (t >= 1.0) AlustaEfekt();
            }
            else if (faas == Faas.Efekt)
            {
                UuendaEfekt();
            }
        }

        private static Color Sega(Color a, Color b, double k) => Color.FromArgb(
            (int)(a.R + (b.R - a.R) * k),
            (int)(a.G + (b.G - a.G) * k),
            (int)(a.B + (b.B - a.B) * k));

        private void AlustaEfekt()
        {
            if (voit == null) { Lopp(); return; }

            faas = Faas.Efekt;
            efektKell.Restart();

            lent.VoiduIndeks = VoiduIndeks;
            lent.SailVarv = voit.Varv;
            lent.PlakatiVarv = voit.Varv;
            lent.ValguseVarv = voit.Varv;

            lblInfo.ForeColor = voit.Varv;
            lblInfo.Text = $"{HarulduseNimi(voit.Aste)}: {voit.Nimi}";

            switch (voit.Aste)
            {
                case Haruldus.Tavaline:
                    efektMs = 800;
                    lent.ValguseAlfa = 70;
                    break;
                case Haruldus.Haruldane:
                    efektMs = 1600;
                    lent.Plakat = "HARULDANE";
                    lent.PlakatNahtav = true;
                    lent.ValguseAlfa = 90;
                    lent.Pursk(45, Sinine, false);
                    break;
                case Haruldus.Eepiline:
                    efektMs = 2600;
                    lent.Plakat = "EEPILINE!";
                    lent.PlakatNahtav = true;
                    lent.ValguseAlfa = 130;
                    lent.Pursk(110, Lilla, false);
                    break;
                case Haruldus.Legendaarne:
                    efektMs = 4200;
                    lent.Plakat = "LEGENDAARNE!!!";
                    lent.PlakatNahtav = true;
                    lent.ValguseAlfa = 170;
                    lent.Raputus = 4;
                    lent.Pursk(160, Kuld, false);
                    break;
                default:
                    efektMs = 3200;
                    lent.Plakat = "KONTO KUSTUTATUD";
                    lent.PlakatNahtav = true;
                    lent.ValguseAlfa = 150;
                    lent.Raputus = 8;
                    lent.Pursk(60, Punane, false);
                    break;
            }
        }

        private void UuendaEfekt()
        {
            if (voit == null) { Lopp(); return; }

            double t = efektKell.ElapsedMilliseconds;
            double k = Math.Min(1.0, t / efektMs);

            lent.Pulss = (float)(0.5 + 0.5 * Math.Sin(t / 90.0));
            lent.Samm(0.015f);

            switch (voit.Aste)
            {
                case Haruldus.Legendaarne:
                    lent.ValguseVarv = Stiil.Vikerkaar(t / 3.0);
                    lent.PlakatiVarv = Stiil.Vikerkaar(t / 2.0 + 90);
                    lent.ValguseAlfa = (int)(170 * (1 - k));
                    lent.Raputus = t < 800 ? 4 : 0;
                    if (rnd.Next(100) < 60) lent.Pursk(7, Kuld, true);
                    break;
                case Haruldus.Eepiline:
                    lent.ValguseAlfa = (int)(130 * (1 - k));
                    if (rnd.Next(100) < 25) lent.Pursk(4, Lilla, true);
                    break;
                case Haruldus.Risk:
                    lent.ValguseAlfa = ((int)(t / 120) % 2 == 0) ? 150 : 15;
                    lent.Raputus = k < 0.8 ? 8 : 0;
                    lent.PlakatNahtav = ((int)(t / 250) % 2) == 0;
                    if (rnd.Next(100) < 40) lent.Pursk(4, Punane, true);
                    break;
                case Haruldus.Haruldane:
                    lent.ValguseAlfa = (int)(90 * (1 - k));
                    break;
                default:
                    lent.ValguseAlfa = (int)(70 * (1 - k));
                    break;
            }

            // Kogu aken värvub korraks preemia värvi
            double tugevus = voit.Aste == Haruldus.Tavaline ? 0.08 : 0.28;
            BackColor = Sega(taust0, voit.Varv, tugevus * (1 - k));

            lent.Invalidate();
            if (t >= efektMs) Lopp();
        }

        private void Lopp()
        {
            timer.Stop();
            faas = Faas.Ootel;
            BackColor = taust0;
            lent.Puhasta();
            UuendaOlek();

            if (voit == null) return;

            lent.VoiduIndeks = VoiduIndeks;      // võidu plaadile jääb sära
            lent.SailVarv = voit.Varv;
            lent.Pulss = 0.6f;
            lent.Invalidate();

            if (voit.Tyyp == PreemiaTyyp.Konto)
            {
                MessageBox.Show(this, "Sinu konto on kustutatud!", "Rull",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();     // konto jääb andmebaasi alles, programm lihtsalt sulgub
                return;
            }

            lblInfo.ForeColor = voit.Varv;
            lblInfo.Text = $"{HarulduseNimi(voit.Aste)}: {voit.Nimi}";
            MessageBox.Show(this,
                $"{HarulduseNimi(voit.Aste)}\n{voit.Nimi}\n\n{voiduTekst}",
                "Palju õnne!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (Keerab) e.Cancel = true;      // võit on juba salvestatud, oota lõpuni
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            base.OnFormClosed(e);
        }

        /// <summary>Plaatide lint + efektid: sära, konfetti, vilkumine, värin, plakat.</summary>
        private sealed class Lent : Control
        {
            public const int PlaadiLaius = 140;

            private sealed class Osake
            {
                public float X, Y, Vx, Vy, Suurus, Elu;
                public Color V;
            }

            private readonly List<Osake> osad = new List<Osake>();
            private readonly Random rnd = new Random();

            public List<Preemia> Plaadid { get; set; } = new List<Preemia>();
            public double Nihe { get; set; }

            public int VoiduIndeks = -1;          // -1 = ei tõsta esile
            public Color SailVarv = Color.White;
            public float Pulss;                   // 0..1
            public Color ValguseVarv = Color.White;
            public int ValguseAlfa;               // 0..255
            public int Raputus;                   // pikslites
            public string Plakat = "";
            public Color PlakatiVarv = Color.White;
            public bool PlakatNahtav;

            public Lent()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.UserPaint, true);
                BackColor = Color.FromArgb(30, 30, 36);
            }

            public void Puhasta()
            {
                osad.Clear();
                VoiduIndeks = -1;
                ValguseAlfa = 0;
                Raputus = 0;
                Pulss = 0;
                Plakat = "";
                PlakatNahtav = false;
                Invalidate();
            }

            /// <summary>Konfetti: ylevalt = langeb ülevalt, muidu purskab keskelt.</summary>
            public void Pursk(int arv, Color[] varvid, bool ylevalt)
            {
                for (int i = 0; i < arv; i++)
                {
                    var o = new Osake
                    {
                        X = ylevalt ? (float)(rnd.NextDouble() * Width) : Width / 2f + (float)((rnd.NextDouble() - 0.5) * 60),
                        Y = ylevalt ? -10f : Height / 2f,
                        Vx = (float)((rnd.NextDouble() - 0.5) * (ylevalt ? 120 : 520)),
                        Vy = ylevalt ? (float)(rnd.NextDouble() * 80) : -(float)(120 + rnd.NextDouble() * 320),
                        Suurus = 4 + rnd.Next(6),
                        Elu = 1.2f + (float)rnd.NextDouble() * 1.2f,
                        V = varvid[rnd.Next(varvid.Length)]
                    };
                    osad.Add(o);
                }
            }

            public void Samm(float dt)
            {
                for (int i = osad.Count - 1; i >= 0; i--)
                {
                    var o = osad[i];
                    o.X += o.Vx * dt;
                    o.Y += o.Vy * dt;
                    o.Vy += 420f * dt;      // raskusjõud
                    o.Elu -= dt;
                    if (o.Elu <= 0 || o.Y > Height + 20) osad.RemoveAt(i);
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.Clear(BackColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using var fNimi = new Font("Segoe UI", 12F, FontStyle.Bold);
                using var fAlam = new Font("Segoe UI", 9F);
                using var fSilt = new Font("Segoe UI", 8F, FontStyle.Bold);
                using var fmt = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                var algus = g.Save();
                if (Raputus > 0)
                    g.TranslateTransform(rnd.Next(-Raputus, Raputus + 1), rnd.Next(-Raputus, Raputus + 1));

                int esimene = Math.Max(0, (int)(Nihe / PlaadiLaius));
                for (int i = esimene; i < Plaadid.Count; i++)
                {
                    float x = (float)(i * PlaadiLaius - Nihe);
                    if (x > Width) break;
                    if (x + PlaadiLaius < 0) continue;

                    var p = Plaadid[i];
                    var rect = new RectangleF(x + 4, 10, PlaadiLaius - 8, Height - 20);

                    using (var grad = new LinearGradientBrush(rect, Color.FromArgb(110, p.Varv), Color.FromArgb(48, 48, 58), 90f))
                        g.FillRectangle(grad, rect);

                    using (var riba = new SolidBrush(p.Varv))
                    {
                        g.FillRectangle(riba, rect.X, rect.Bottom - 8, rect.Width, 8);
                        g.DrawString(HarulduseNimi(p.Aste), fSilt, riba,
                            new RectangleF(rect.X, rect.Y + 4, rect.Width, 16), fmt);
                    }

                    g.DrawString(p.Nimi, fNimi, Brushes.White,
                        new RectangleF(rect.X + 4, rect.Y + 24, rect.Width - 8, rect.Height - 90), fmt);
                    g.DrawString(p.Alamtekst, fAlam, Brushes.Silver,
                        new RectangleF(rect.X + 4, rect.Bottom - 56, rect.Width - 8, 40), fmt);
                }

                // Võitja plaadi sära (pulseerib)
                if (VoiduIndeks >= 0)
                {
                    float vx = (float)(VoiduIndeks * PlaadiLaius - Nihe) + 4;
                    for (int k = 1; k <= 4; k++)
                    {
                        int alfa = Math.Clamp((int)((80 + 150 * Pulss) / k), 0, 255);
                        using var pen = new Pen(Color.FromArgb(alfa, SailVarv), 2 * k);
                        g.DrawRectangle(pen, vx - k * 2, 10 - k * 2, PlaadiLaius - 8 + k * 4, Height - 20 + k * 4);
                    }
                }

                using (var pen = new Pen(Color.Gold, 3))
                    g.DrawLine(pen, Width / 2, 0, Width / 2, Height);

                // Konfetti
                foreach (var o in osad)
                {
                    int alfa = (int)(255 * Math.Min(1f, o.Elu));
                    using var b = new SolidBrush(Color.FromArgb(Math.Clamp(alfa, 0, 255), o.V));
                    g.FillRectangle(b, o.X, o.Y, o.Suurus, o.Suurus * 0.6f);
                }

                g.Restore(algus);

                // Vilkuv värviline kiht
                if (ValguseAlfa > 0)
                {
                    using var b = new SolidBrush(Color.FromArgb(Math.Clamp(ValguseAlfa, 0, 255), ValguseVarv));
                    g.FillRectangle(b, ClientRectangle);
                }

                // Suur tekst keskel
                if (PlakatNahtav && Plakat.Length > 0)
                {
                    using var f = new Font("Segoe UI", 26F, FontStyle.Bold);
                    using var vari = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
                    using var tekst = new SolidBrush(PlakatiVarv);
                    g.DrawString(Plakat, f, vari, new RectangleF(2, 2, Width, Height), fmt);
                    g.DrawString(Plakat, f, tekst, new RectangleF(0, 0, Width, Height), fmt);
                }
            }
        }
    }
}
