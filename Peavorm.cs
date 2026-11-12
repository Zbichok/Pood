using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tooted
{
    public partial class Peavorm : Form
    {
        /// <summary>true, kui kasutaja vajutas "Logi välja" (mitte akna X-i).</summary>
        public bool LogiValja { get; private set; }

        public Peavorm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Stiil.Rakenda(this);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            btnTooted.Enabled = AktiivneKasutaja.OnAdmin;
            Text = $"Peamenüü – {AktiivneKasutaja.Nimi} ({AktiivneKasutaja.Roll})";

            KaivitaRullNupuEfekt();
        }

        // ---------- Rulli nupp: vikerkaar, kui täna veel saab keerata ----------
        private System.Windows.Forms.Timer? rullTimer;
        private double rullKraad;
        private bool rullSaadaval;
        private Font? rullPaksFont;
        private Color algTaust, algTekst, algJoon;
        private int algJooneSuurus;
        private Font? algFont;
        private string algTekstSona = "";

        private void KaivitaRullNupuEfekt()
        {
            algTaust = btnRull.BackColor;
            algTekst = btnRull.ForeColor;
            algJoon = btnRull.FlatAppearance.BorderColor;
            algJooneSuurus = btnRull.FlatAppearance.BorderSize;
            algFont = btnRull.Font;
            algTekstSona = btnRull.Text;
            rullPaksFont = new Font(btnRull.Font, FontStyle.Bold);

            rullTimer = new System.Windows.Forms.Timer { Interval = 40 };
            rullTimer.Tick += (s, e) => UuendaRullNuppu();
            rullTimer.Start();
            UuendaRullNuppu();
        }

        private void UuendaRullNuppu()
        {
            if (AktiivneKasutaja.VoibKeerata)
            {
                rullSaadaval = true;
                rullKraad += 6;
                double pulss = 0.5 + 0.5 * Math.Sin(rullKraad / 12.0);

                btnRull.Text = "★ PÄEVANE RULL ★";
                btnRull.Font = rullPaksFont;
                btnRull.ForeColor = Color.Black;
                btnRull.BackColor = Stiil.Vikerkaar(rullKraad);
                btnRull.FlatAppearance.BorderColor = Stiil.Vikerkaar(rullKraad + 180, 0.9);
                btnRull.FlatAppearance.BorderSize = 2 + (int)Math.Round(2 * pulss);
            }
            else if (rullSaadaval || btnRull.Text == algTekstSona)
            {
                // Täna juba keeratud: tavaline, vaikne nupp
                rullSaadaval = false;
                btnRull.Text = "Päevane rull (homme)";
                btnRull.Font = algFont;
                btnRull.ForeColor = algTekst;
                btnRull.BackColor = algTaust;
                btnRull.FlatAppearance.BorderColor = algJoon;
                btnRull.FlatAppearance.BorderSize = algJooneSuurus;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            rullTimer?.Stop();
            rullTimer?.Dispose();
            rullPaksFont?.Dispose();
            base.OnFormClosed(e);
        }
        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogiValja = true;
            Close();
        }

        private void btnRull_Click(object sender, EventArgs e)
        {
            using var rull = new RullForm();
            rull.ShowDialog();
        }

        private void btnTooted_Click(object sender, EventArgs e)
        {
            tooted f = new tooted();
            f.ShowDialog();
        }

        private void btnPood_Click(object sender, EventArgs e)
        {
            Form1 poodForm = new Form1();
            poodForm.ShowDialog();
        }

        private void btnArve_Click(object sender, EventArgs e)
        {
            ArveForm arve = new ArveForm();
            arve.ShowDialog();
        }

        private void github_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/Zbichok?tab=repositories",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Brauserit ei saanud avada: " + ex.Message);
            }
        }
    }
}
