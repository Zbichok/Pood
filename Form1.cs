using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tooted
{
    public partial class Form1 : Form
    {
        private readonly ToodedContext _db;

        private const int KaartLaius = 160;
        private const int KaartKorgus = 262;
        private const int Veerge = 5;

        public Form1()
        {
            InitializeComponent();
            _db = new ToodedContext();
            LaeKategooriadJaPildid();
            UuendaKorviInfo();
            StartPosition = FormStartPosition.CenterScreen;
            Stiil.Rakenda(this);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Text = "Pood";

            if (AktiivneKasutaja.OnAdmin)
                lblMarkus.Text = "Admin: ostud on testostud (andmeid ei muudeta).";
        }

        private void LaeKategooriadJaPildid()
        {
            tabpood.TabPages.Clear();

            var kategooriad = _db.Kategooriatabel.ToList();

            foreach (var kat in kategooriad)
            {
                var page = new TabPage(kat.Kategooria_nimetus);
                page.AutoScroll = true;

                var tooted = _db.Toodetabel
                                .Where(t => t.KategooriaId == kat.Id)
                                .ToList();

                int x = 10;
                int y = 10;
                int count = 0;

                foreach (var t in tooted)
                {
                    page.Controls.Add(LooKaart(t, x, y));

                    x += KaartLaius + 10;
                    count++;

                    if (count % Veerge == 0)
                    {
                        x = 10;
                        y += KaartKorgus + 10;
                    }
                }

                tabpood.TabPages.Add(page);
            }
        }

        private Panel LooKaart(Toode t, int x, int y)
        {
            Panel p = new Panel
            {
                Width = KaartLaius,
                Height = KaartKorgus,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(x, y)
            };

            PictureBox pb = new PictureBox
            {
                Width = 120,
                Height = 120,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(18, 5),
                Image = Pildid.Lae(t.Pilt)
            };

            Label lblNimi = new Label
            {
                Text = t.Toodenimetus,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(5, 128),
                Size = new Size(KaartLaius - 12, 38)
            };

            Label lblHind = new Label
            {
                Text = $"{t.Hind:0.00} €",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(Font, FontStyle.Bold),
                Location = new Point(5, 167),
                Size = new Size(KaartLaius - 12, 20)
            };

            bool laos = t.Kogus > 0;
            int maxKogus = Math.Max(1, t.Kogus);

            // Koguse valik: [-] [arv] [+] – nuppudega või käsitsi kirjutades
            Button btnMiinus = new Button
            {
                Text = "-",
                Location = new Point(15, 191),
                Size = new Size(30, 27),
                Enabled = laos
            };

            TextBox txtKogus = new TextBox
            {
                Text = "1",
                TextAlign = HorizontalAlignment.Center,
                MaxLength = 4,
                Location = new Point(49, 192),
                Width = 58,
                Enabled = laos
            };

            Button btnPluss = new Button
            {
                Text = "+",
                Location = new Point(111, 191),
                Size = new Size(30, 27),
                Enabled = laos
            };

            Button btn = new Button
            {
                Text = laos ? "Lisa korvi" : "Laost otsas",
                Location = new Point(15, 224),
                Size = new Size(KaartLaius - 32, 27),
                Enabled = laos
            };

            int LoeKogus() => int.TryParse(txtKogus.Text, out int v) ? v : 0;
            void SeaKogus(int v) => txtKogus.Text = Math.Clamp(v, 1, maxKogus).ToString();

            // Ainult numbrid
            txtKogus.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
            // Kui väli kaotab fookuse: tühi / liiga suur / 0 -> korrigeeri
            txtKogus.Leave += (s, e) => SeaKogus(LoeKogus());
            txtKogus.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SeaKogus(LoeKogus());
                    e.SuppressKeyPress = true;
                }
            };

            btnMiinus.Click += (s, e) => SeaKogus(LoeKogus() - 1);
            btnPluss.Click += (s, e) => SeaKogus(LoeKogus() + 1);

            btn.Click += (s, e) =>
            {
                int kogus = LoeKogus();
                if (kogus < 1)
                {
                    SeaKogus(1);
                    MessageBox.Show("Sisesta kogus (vähemalt 1).");
                    return;
                }

                if (!Ostukorv.Lisa(t, kogus, out string? viga))
                {
                    MessageBox.Show(viga);
                    return;
                }

                SeaKogus(1);
                UuendaKorviInfo();
            };

            p.Controls.Add(pb);
            p.Controls.Add(lblNimi);
            p.Controls.Add(lblHind);
            p.Controls.Add(btnMiinus);
            p.Controls.Add(txtKogus);
            p.Controls.Add(btnPluss);
            p.Controls.Add(btn);

            return p;
        }

        private void UuendaKorviInfo()
        {
            lblKorv.Text = $"Ostukorv: {Ostukorv.Tukke} tk – {Ostukorv.Vahesumma:0.00} €";
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
        }
    }
}
