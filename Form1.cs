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
        private const int KaartKorgus = 245;
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
                lblMarkus.Text = "Admin ei saa ostukorvi muuta (ainult vaatamine).";
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
                Width = 140,
                Height = 140,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(8, 5),
                Image = Pildid.Lae(t.Pilt)
            };

            Label lblNimi = new Label
            {
                Text = t.Toodenimetus,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(5, 150),
                Size = new Size(KaartLaius - 12, 40)
            };

            Label lblHind = new Label
            {
                Text = $"{t.Hind:0.00} €",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(Font, FontStyle.Bold),
                Location = new Point(5, 192),
                Size = new Size(KaartLaius - 12, 20)
            };

            Button btn = new Button
            {
                Text = "Lisa korvi",
                Location = new Point(15, 213),
                Size = new Size(KaartLaius - 32, 27),
                Enabled = !AktiivneKasutaja.OnAdmin && t.Kogus > 0
            };

            if (t.Kogus <= 0)
                btn.Text = "Laost otsas";

            btn.Click += (s, e) =>
            {
                if (!Ostukorv.Lisa(t, 1, out string? viga))
                {
                    MessageBox.Show(viga);
                    return;
                }

                UuendaKorviInfo();
            };

            p.Controls.Add(pb);
            p.Controls.Add(lblNimi);
            p.Controls.Add(lblHind);
            p.Controls.Add(btn);

            return p;
        }

        private void UuendaKorviInfo()
        {
            lblKorv.Text = $"Ostukorv: {Ostukorv.Tukke} tk – {Ostukorv.Kokku:0.00} €";
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
        }
    }
}
