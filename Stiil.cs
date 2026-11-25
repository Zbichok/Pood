namespace Tooted
{
    /// <summary>Lihtne ühtne välimus: hele taust, ühe fondiga, tasased nupud.</summary>
    public static class Stiil
    {
        private static readonly Color Taust = Color.FromArgb(248, 248, 246);
        private static readonly Color NuppTaust = Color.FromArgb(236, 236, 232);
        private static readonly Color Joon = Color.FromArgb(205, 205, 200);
        private static readonly Color Tekst = Color.FromArgb(40, 40, 40);

        public static void Rakenda(Form vorm)
        {
            Keel.Lisa(vorm);
            vorm.BackColor = Taust;
            vorm.ForeColor = Tekst;
            vorm.Font = new Font("Segoe UI", 9.5F);
            RakendaKontrollidele(vorm.Controls);
        }

        /// <summary>Nupp "Peamenüü": sulgeb vormi ja viib tagasi peamenüüsse (vormid avatakse ShowDialog-iga).</summary>
        public static Button LisaTagasiNupp(Form vorm, Point asukoht)
        {
            var nupp = new Button
            {
                Text = Keel.T("← Peamenüü", "← Главное меню", "← Main menu"),
                Location = asukoht,
                Size = new Size(130, 32),
                Name = "btnTagasi"
            };
            nupp.FlatStyle = FlatStyle.Flat;
            nupp.FlatAppearance.BorderColor = Joon;
            nupp.FlatAppearance.BorderSize = 1;
            nupp.BackColor = NuppTaust;
            nupp.Cursor = Cursors.Hand;
            nupp.Click += (s, e) => vorm.Close();
            vorm.Controls.Add(nupp);
            return nupp;
        }

        public static void RakendaKontrollidele(Control.ControlCollection kontrollid)
        {
            foreach (Control c in kontrollid)
            {
                if (c is Button b && b.BackgroundImage == null)
                {
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderColor = Joon;
                    b.FlatAppearance.BorderSize = 1;
                    b.BackColor = NuppTaust;
                    b.Cursor = Cursors.Hand;
                }
                else if (c is Button img && img.BackgroundImage != null)
                {
                    img.FlatStyle = FlatStyle.Flat;
                    img.FlatAppearance.BorderSize = 0;
                }
                else if (c is Panel p && p.BorderStyle == BorderStyle.FixedSingle)
                {
                    p.BackColor = Color.White;
                }
                else if (c is TextBox || c is ListBox || c is ComboBox)
                {
                    c.BackColor = Color.White;
                }

                if (c.HasChildren)
                    RakendaKontrollidele(c.Controls);
            }
        }
    }
}
