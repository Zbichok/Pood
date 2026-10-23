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
        public Peavorm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            btnTooted.Enabled = AktiivneKasutaja.OnAdmin;
            Text = $"Peamenüü – {AktiivneKasutaja.Nimi} ({AktiivneKasutaja.Roll})";
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
