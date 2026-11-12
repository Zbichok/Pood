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
            vorm.BackColor = Taust;
            vorm.ForeColor = Tekst;
            vorm.Font = new Font("Segoe UI", 9.5F);
            RakendaKontrollidele(vorm.Controls);
        }

        /// <summary>Vikerkaarevärv (kraad 0-360), pastelne. Kasutab rulli efekt ja menüü nupp.</summary>
        public static Color Vikerkaar(double kraad, double heledus = 1.0)
        {
            double h = (((kraad % 360) + 360) % 360) / 60.0;
            double s = 0.6, v = heledus;
            int i = (int)Math.Floor(h) % 6;
            double f = h - Math.Floor(h);
            double p = v * (1 - s), q = v * (1 - s * f), t = v * (1 - s * (1 - f));
            double r, g, b;
            switch (i)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                default: r = v; g = p; b = q; break;
            }
            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
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
