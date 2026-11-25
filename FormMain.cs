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
    public partial class FormMain : Form
    {
        /// <summary>true, kui kasutaja vajutas "Logi välja" (mitte akna X-i).</summary>
        public bool LogiValja { get; private set; }

        public FormMain()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Stiil.Rakenda(this);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            if (!AktiivneKasutaja.OnAdmin)
            {
                // Tavakasutajal Tootehaldus-nuppu ei ole: peidame ja nihutame teised nupud ülespoole
                btnTooted.Visible = false;
                int nihe = btnPood.Top - btnTooted.Top;
                foreach (var nupp in new[] { btnPood, btnArve, btnLogout })
                    nupp.Top -= nihe;
            }
            Text = Keel.T($"Peamenüü – {AktiivneKasutaja.Nimi} ({AktiivneKasutaja.Roll})", $"Главное меню — {AktiivneKasutaja.Nimi} ({AktiivneKasutaja.Roll})", $"Main menu — {AktiivneKasutaja.Nimi} ({AktiivneKasutaja.Roll})");
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogiValja = true;
            Close();
        }

        private void btnTooted_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.ShowDialog();
        }

        private void btnPood_Click(object sender, EventArgs e)
        {
            Form3 poodForm = new Form3();
            poodForm.ShowDialog();
        }

        private void btnArve_Click(object sender, EventArgs e)
        {
            ArveForm arve = new ArveForm();
            arve.ShowDialog();
        }
    }
}
