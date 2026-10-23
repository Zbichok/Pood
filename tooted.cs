using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Tooted
{
    public partial class tooted : Form
    {
        private readonly ToodedContext _db;
        private string? valitudPilt;
        public tooted()
        {
            InitializeComponent();
            _db = new ToodedContext();
            LaeKategooriad();
            LaeTooted();

            dataGridTooted.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridTooted.MultiSelect = false;
            dataGridTooted.ReadOnly = true;
            dataGridTooted.AllowUserToAddRows = false;
            picPilt.SizeMode = PictureBoxSizeMode.Zoom;

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            if (!AktiivneKasutaja.OnAdmin)
            {
                btnLisa.Enabled = false;
                btnUuenda.Enabled = false;
                btnKustuta.Enabled = false;
                btnLisaKategooria.Enabled = false;
                btnKustutaKategooria.Enabled = false;
            }
        }

        private void LaeKategooriad()
        {
            cmbKategooria.DataSource = _db.Kategooriatabel.ToList();
            cmbKategooria.DisplayMember = "Kategooria_nimetus";
            cmbKategooria.ValueMember = "Id";
        }

        private void LaeTooted()
        {
            dataGridTooted.DataSource = _db.Toodetabel
                .Include(t => t.Kategooria)
                .Select(t => new
                {
                    t.Id,
                    t.Toodenimetus,
                    t.Kogus,
                    t.Hind,
                    Kategooria = t.Kategooria.Kategooria_nimetus,
                    t.Pilt
                })
                .ToList();
        }

        private void btnOtsi_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Pildid|*.jpg;*.png;*.jpeg";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                valitudPilt = ofd.FileName;
                NaitaPilti(Pildid.LaeFailist(valitudPilt));
            }
        }

        private void NaitaPilti(Image? pilt)
        {
            picPilt.Image?.Dispose();
            picPilt.Image = pilt;
        }

        private void TuhjendaVorm()
        {
            txtNimetus.Clear();
            txtKogus.Clear();
            txtHind.Clear();
            valitudPilt = null;
            NaitaPilti(null);
            dataGridTooted.ClearSelection();
        }

        private bool LoeVorm(out int kogus, out float hind)
        {
            hind = 0;

            if (string.IsNullOrWhiteSpace(txtNimetus.Text))
            {
                MessageBox.Show("Sisesta toote nimetus!");
                kogus = 0;
                return false;
            }

            if (!int.TryParse(txtKogus.Text, out kogus) || kogus < 0)
            {
                MessageBox.Show("Vale kogus!");
                return false;
            }

            if (!float.TryParse(txtHind.Text.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out hind) || hind < 0)
            {
                MessageBox.Show("Vale hind!");
                return false;
            }

            if (cmbKategooria.SelectedValue == null)
            {
                MessageBox.Show("Vali kategooria!");
                return false;
            }

            return true;
        }

        private int? ValitudId()
        {
            if (dataGridTooted.SelectedRows.Count == 0) return null;
            return dataGridTooted.SelectedRows[0].Cells["Id"].Value as int?;
        }

        private void btnLisa_Click(object sender, EventArgs e)
        {
            if (!LoeVorm(out int kogus, out float hind)) return;

            var uus = new Toode
            {
                Toodenimetus = txtNimetus.Text.Trim(),
                Kogus = kogus,
                Hind = hind,
                KategooriaId = (int)cmbKategooria.SelectedValue,
                Pilt = valitudPilt != null ? Pildid.Salvesta(valitudPilt) : ""
            };

            _db.Toodetabel.Add(uus);
            _db.SaveChanges();
            LaeTooted();
            TuhjendaVorm();
        }

        private void btnLisaKategooria_Click(object sender, EventArgs e)
        {
            using Form aken = new Form();
            aken.Text = "Lisa kategooria";
            aken.StartPosition = FormStartPosition.CenterParent;
            aken.Size = new Size(350, 250);
            aken.FormBorderStyle = FormBorderStyle.FixedDialog;
            aken.MaximizeBox = false;
            aken.MinimizeBox = false;

            Label lblNimi = new Label()
            {
                Text = "Kategooria nimi:",
                Location = new Point(15, 20),
                AutoSize = true
            };

            TextBox txtNimi = new TextBox()
            {
                Location = new Point(15, 45),
                Width = 300
            };

            Label lblKirjeldus = new Label()
            {
                Text = "Kirjeldus:",
                Location = new Point(15, 80),
                AutoSize = true
            };

            TextBox txtKirjeldus = new TextBox()
            {
                Location = new Point(15, 105),
                Width = 300
            };

            Button btnSalvesta = new Button()
            {
                Text = "Salvesta",
                Location = new Point(15, 150),
                Width = 140,
                DialogResult = DialogResult.None
            };

            Button btnLoobu = new Button()
            {
                Text = "Loobu",
                Location = new Point(175, 150),
                Width = 140,
                DialogResult = DialogResult.Cancel
            };

            btnSalvesta.Click += (s, args) =>
            {
                string nimi = txtNimi.Text.Trim();
                string kirjeldus = txtKirjeldus.Text.Trim();

                if (string.IsNullOrWhiteSpace(nimi))
                {
                    MessageBox.Show("Sisesta kategooria nimi!");
                    return;
                }

                if (_db.Kategooriatabel.Any(k =>
                    k.Kategooria_nimetus.ToLower() == nimi.ToLower()))
                {
                    MessageBox.Show("Selline kategooria on juba olemas!");
                    return;
                }

                try
                {
                    var kategooria = new Kategooria
                    {
                        Kategooria_nimetus = nimi,
                        Kirjeldus = kirjeldus
                    };

                    _db.Kategooriatabel.Add(kategooria);
                    _db.SaveChanges();

                    LaeKategooriad();

                    cmbKategooria.SelectedValue = kategooria.Id;

                    MessageBox.Show("Kategooria lisatud!");
                    aken.DialogResult = DialogResult.OK;
                    aken.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Viga kategooria lisamisel: " + ex.Message);
                }
            };

            aken.Controls.Add(lblNimi);
            aken.Controls.Add(txtNimi);
            aken.Controls.Add(lblKirjeldus);
            aken.Controls.Add(txtKirjeldus);
            aken.Controls.Add(btnSalvesta);
            aken.Controls.Add(btnLoobu);

            aken.AcceptButton = btnSalvesta;
            aken.CancelButton = btnLoobu;

            aken.ShowDialog(this);
        }

        private void btnKustutaKategooria_Click(object sender, EventArgs e)
        {
            using Form aken = new Form();
            aken.Text = "Kustuta kategooria";
            aken.StartPosition = FormStartPosition.CenterParent;
            aken.Size = new Size(350, 180);
            aken.FormBorderStyle = FormBorderStyle.FixedDialog;
            aken.MaximizeBox = false;
            aken.MinimizeBox = false;

            Label lblNimi = new Label()
            {
                Text = "Sisesta kategooria nimi:",
                Location = new Point(15, 20),
                AutoSize = true
            };

            TextBox txtNimi = new TextBox()
            {
                Location = new Point(15, 50),
                Width = 300
            };

            Button btnKustuta = new Button()
            {
                Text = "Kustuta",
                Location = new Point(15, 90),
                Width = 140
            };

            Button btnLoobu = new Button()
            {
                Text = "Loobu",
                Location = new Point(175, 90),
                Width = 140,
                DialogResult = DialogResult.Cancel
            };

            btnKustuta.Click += (s, args) =>
            {
                string nimi = txtNimi.Text.Trim();

                if (string.IsNullOrWhiteSpace(nimi))
                {
                    MessageBox.Show("Sisesta kategooria nimi!");
                    return;
                }

                var kategooria = _db.Kategooriatabel
                    .FirstOrDefault(k =>
                        k.Kategooria_nimetus.ToLower() == nimi.ToLower());

                if (kategooria == null)
                {
                    MessageBox.Show("Kategooriat ei leitud!");
                    return;
                }

                bool tootedOlemas = _db.Toodetabel
                    .Any(t => t.KategooriaId == kategooria.Id);

                if (tootedOlemas)
                {
                    MessageBox.Show(
                        "Selles kategoorias on tooteid. " +
                        "Kustuta või muuda kõigepealt nende toodete kategooriat."
                    );
                    return;
                }

                var vastus = MessageBox.Show(
                    $"Kas soovid kategooria \"{kategooria.Kategooria_nimetus}\" kustutada?",
                    "Kinnitamine",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (vastus != DialogResult.Yes)
                    return;

                try
                {
                    _db.Kategooriatabel.Remove(kategooria);
                    _db.SaveChanges();

                    LaeKategooriad();

                    MessageBox.Show("Kategooria kustutatud!");

                    aken.DialogResult = DialogResult.OK;
                    aken.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Viga kategooria kustutamisel: " + ex.Message);
                }
            };

            aken.Controls.Add(lblNimi);
            aken.Controls.Add(txtNimi);
            aken.Controls.Add(btnKustuta);
            aken.Controls.Add(btnLoobu);

            aken.AcceptButton = btnKustuta;
            aken.CancelButton = btnLoobu;

            aken.ShowDialog(this);
        }

        private void dataGridViewTooted_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var rida = dataGridTooted.Rows[e.RowIndex];

            txtNimetus.Text = rida.Cells["Toodenimetus"].Value?.ToString();
            txtKogus.Text = rida.Cells["Kogus"].Value?.ToString();
            txtHind.Text = rida.Cells["Hind"].Value?.ToString();

            int id = (int)rida.Cells["Id"].Value;
            var toode = _db.Toodetabel.Find(id);
            if (toode != null)
                cmbKategooria.SelectedValue = toode.KategooriaId;

            valitudPilt = null;
            NaitaPilti(Pildid.Lae(rida.Cells["Pilt"].Value?.ToString()));
        }

        private void btnUuenda_Click(object sender, EventArgs e)
        {
            int? id = ValitudId();
            if (id == null)
            {
                MessageBox.Show("Vali tabelist toode!");
                return;
            }

            if (!LoeVorm(out int kogus, out float hind)) return;

            var toode = _db.Toodetabel.Find(id.Value);
            if (toode == null) return;

            toode.Toodenimetus = txtNimetus.Text.Trim();
            toode.Kogus = kogus;
            toode.Hind = hind;
            toode.KategooriaId = (int)cmbKategooria.SelectedValue;

            if (valitudPilt != null)
                toode.Pilt = Pildid.Salvesta(valitudPilt);

            _db.SaveChanges();
            LaeTooted();
            TuhjendaVorm();
        }

        private void btnKustuta_Click(object sender, EventArgs e)
        {
            int? id = ValitudId();
            if (id == null)
            {
                MessageBox.Show("Vali tabelist toode!");
                return;
            }

            var toode = _db.Toodetabel.Find(id.Value);
            if (toode == null) return;

            if (MessageBox.Show($"Kas kustutada \"{toode.Toodenimetus}\"?", "Kinnitamine",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _db.Toodetabel.Remove(toode);
            _db.SaveChanges();
            LaeTooted();
            TuhjendaVorm();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cmbKategooria_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
