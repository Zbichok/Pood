using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Tooted
{
    public partial class Form2 : Form
    {
        private readonly ToodedContext _db;
        private string? valitudPilt;
        public Form2()
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
            Stiil.Rakenda(this);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Stiil.LisaTagasiNupp(this, new Point(12, 515));

            var btnTyhjenda = new Button { Name = "btnTyhjenda", Text = Keel.T("Tühjenda väljad", "Очистить поля", "Clear fields"), Location = new Point(12, 218), Size = new Size(97, 55) };
            btnTyhjenda.Click += (s, e) => TuhjendaVorm();
            Controls.Add(btnTyhjenda);
            Stiil.RakendaKontrollidele(Controls);

            if (!AktiivneKasutaja.OnAdmin)
            {
                btnLisa.Enabled = false;
                btnUuenda.Enabled = false;
                btnKustuta.Enabled = false;
                btnLisaKategooria.Enabled = false;
                btnKustutaKategooria.Enabled = false;
                btnFoto.Enabled = false;
                btnKustutaFoto.Enabled = false;
            }

            UuendaFotoNupud(null);
            Paiguta();
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
                NaitaPilti(Kaustad.LaeFailist(valitudPilt));
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
            UuendaFotoNupud(null);
        }

        private bool LoeVorm(out int kogus, out decimal hind)
        {
            hind = 0;

            if (string.IsNullOrWhiteSpace(txtNimetus.Text))
            {
                MessageBox.Show(Keel.T("Sisesta toote nimetus!", "Введите название товара!", "Enter a product name!"));
                kogus = 0;
                return false;
            }

            if (!int.TryParse(txtKogus.Text, out kogus) || kogus < 0)
            {
                MessageBox.Show(Keel.T("Vale kogus!", "Неверное количество!", "Invalid quantity!"));
                return false;
            }

            if (!decimal.TryParse(txtHind.Text.Replace(',', '.'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out hind) || hind < 0)
            {
                MessageBox.Show(Keel.T("Vale hind!", "Неверная цена!", "Invalid price!"));
                return false;
            }

            hind = Math.Round(hind, 2);

            if (cmbKategooria.SelectedValue == null)
            {
                MessageBox.Show(Keel.T("Vali kategooria!", "Выберите категорию!", "Select a category!"));
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
            if (!LoeVorm(out int kogus, out decimal hind)) return;

            var uus = new Toode
            {
                Toodenimetus = txtNimetus.Text.Trim(),
                Kogus = kogus,
                Hind = hind,
                KategooriaId = (int)cmbKategooria.SelectedValue,
                Pilt = valitudPilt != null ? Kaustad.Salvesta(valitudPilt) : ""
            };

            _db.Toodetabel.Add(uus);
            _db.SaveChanges();
            LaeTooted();
            TuhjendaVorm();
        }

        private void btnLisaKategooria_Click(object sender, EventArgs e)
        {
            using Form aken = new Form();
            aken.Text = Keel.T("Lisa kategooria", "Добавить категорию", "Add category");
            aken.StartPosition = FormStartPosition.CenterParent;
            aken.Size = new Size(350, 250);
            aken.FormBorderStyle = FormBorderStyle.FixedDialog;
            aken.MaximizeBox = false;
            aken.MinimizeBox = false;

            Label lblNimi = new Label()
            {
                Text = Keel.T("Kategooria nimi:", "Название категории:", "Category name:"),
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
                Text = Keel.T("Kirjeldus:", "Описание:", "Description:"),
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
                Text = Keel.T("Salvesta", "Сохранить", "Save"),
                Location = new Point(15, 150),
                Width = 140,
                DialogResult = DialogResult.None
            };

            Button btnLoobu = new Button()
            {
                Text = Keel.T("Loobu", "Отмена", "Cancel"),
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
                    MessageBox.Show(Keel.T("Sisesta kategooria nimi!", "Введите название категории!", "Enter a category name!"));
                    return;
                }

                if (_db.Kategooriatabel.Any(k =>
                    k.Kategooria_nimetus.ToLower() == nimi.ToLower()))
                {
                    MessageBox.Show(Keel.T("Selline kategooria on juba olemas!", "Такая категория уже существует!", "This category already exists!"));
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

                    MessageBox.Show(Keel.T("Kategooria lisatud!", "Категория добавлена!", "Category added!"));
                    aken.DialogResult = DialogResult.OK;
                    aken.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Keel.T("Viga kategooria lisamisel: ", "Ошибка добавления категории: ", "Error adding category: ") + ex.Message);
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
            aken.Text = Keel.T("Kustuta kategooria", "Удалить категорию", "Delete category");
            aken.StartPosition = FormStartPosition.CenterParent;
            aken.Size = new Size(350, 180);
            aken.FormBorderStyle = FormBorderStyle.FixedDialog;
            aken.MaximizeBox = false;
            aken.MinimizeBox = false;

            Label lblNimi = new Label()
            {
                Text = Keel.T("Sisesta kategooria nimi:", "Введите название категории:", "Enter category name:"),
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
                Text = Keel.T("Kustuta", "Удалить", "Delete"),
                Location = new Point(15, 90),
                Width = 140
            };

            Button btnLoobu = new Button()
            {
                Text = Keel.T("Loobu", "Отмена", "Cancel"),
                Location = new Point(175, 90),
                Width = 140,
                DialogResult = DialogResult.Cancel
            };

            btnKustuta.Click += (s, args) =>
            {
                string nimi = txtNimi.Text.Trim();

                if (string.IsNullOrWhiteSpace(nimi))
                {
                    MessageBox.Show(Keel.T("Sisesta kategooria nimi!", "Введите название категории!", "Enter a category name!"));
                    return;
                }

                var kategooria = _db.Kategooriatabel
                    .FirstOrDefault(k =>
                        k.Kategooria_nimetus.ToLower() == nimi.ToLower());

                if (kategooria == null)
                {
                    MessageBox.Show(Keel.T("Kategooriat ei leitud!", "Категория не найдена!", "Category not found!"));
                    return;
                }

                bool tootedOlemas = _db.Toodetabel
                    .Any(t => t.KategooriaId == kategooria.Id);

                if (tootedOlemas)
                {
                    MessageBox.Show(
                        Keel.T("Selles kategoorias on tooteid. ", "В этой категории есть товары. ", "This category contains products. ") +
                        Keel.T("Kustuta või muuda kõigepealt nende toodete kategooriat.", "Сначала удалите товары или измените их категорию.", "First delete the products or change their category.")
                    );
                    return;
                }

                var vastus = MessageBox.Show(
                    Keel.T($"Kas soovid kategooria \"{kategooria.Kategooria_nimetus}\" kustutada?", $"Удалить категорию \"{kategooria.Kategooria_nimetus}\"?", $"Delete category \"{kategooria.Kategooria_nimetus}\"?"),
                    Keel.T("Kinnitamine", "Подтверждение", "Confirmation"),
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

                    MessageBox.Show(Keel.T("Kategooria kustutatud!", "Категория удалена!", "Category deleted!"));

                    aken.DialogResult = DialogResult.OK;
                    aken.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Keel.T("Viga kategooria kustutamisel: ", "Ошибка удаления категории: ", "Error deleting category: ") + ex.Message);
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
            string? pilt = rida.Cells["Pilt"].Value?.ToString();
            NaitaPilti(Kaustad.LaePilt(pilt));
            UuendaFotoNupud(pilt);
        }

        private void btnUuenda_Click(object sender, EventArgs e)
        {
            int? id = ValitudId();
            if (id == null)
            {
                MessageBox.Show(Keel.T("Vali tabelist toode!", "Выберите товар в таблице!", "Select a product in the table!"));
                return;
            }

            if (!LoeVorm(out int kogus, out decimal hind)) return;

            var toode = _db.Toodetabel.Find(id.Value);
            if (toode == null) return;

            toode.Toodenimetus = txtNimetus.Text.Trim();
            toode.Kogus = kogus;
            toode.Hind = hind;
            toode.KategooriaId = (int)cmbKategooria.SelectedValue;

            if (valitudPilt != null)
                toode.Pilt = Kaustad.Salvesta(valitudPilt);

            _db.SaveChanges();
            LaeTooted();
            TuhjendaVorm();
        }

        private void btnKustuta_Click(object sender, EventArgs e)
        {
            int? id = ValitudId();
            if (id == null)
            {
                MessageBox.Show(Keel.T("Vali tabelist toode!", "Выберите товар в таблице!", "Select a product in the table!"));
                return;
            }

            var toode = _db.Toodetabel.Find(id.Value);
            if (toode == null) return;

            if (MessageBox.Show(Keel.T($"Kas kustutada \"{toode.Toodenimetus}\"?", $"Удалить \"{toode.Toodenimetus}\"?", $"Delete \"{toode.Toodenimetus}\"?"), Keel.T("Kinnitamine", "Подтверждение", "Confirmation"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _db.Toodetabel.Remove(toode);
            _db.SaveChanges();
            LaeTooted();
            TuhjendaVorm();
        }

        // ---------- Foto lisamine / vahetamine / kustutamine valitud tootel ----------

        /// <summary>Nupu tekst: "Lisa foto", kui fotot pole, muidu "Muuda foto".</summary>
        private void UuendaFotoNupud(string? pilt)
        {
            bool onFoto = !string.IsNullOrEmpty(pilt);
            btnFoto.Text = onFoto ? Keel.T("Muuda foto", "Изменить фото", "Change photo") : Keel.T("Lisa foto", "Добавить фото", "Add photo");
            btnKustutaFoto.Visible = onFoto;
        }

        /// <summary>Pärast tabeli uuesti laadimist valib sama toote rea tagasi.</summary>
        private void ValiRida(int id)
        {
            foreach (DataGridViewRow r in dataGridTooted.Rows)
            {
                if (r.Cells["Id"].Value is int rid && rid == id)
                {
                    dataGridTooted.ClearSelection();
                    r.Selected = true;
                    dataGridTooted.CurrentCell = r.Cells["Toodenimetus"];
                    return;
                }
            }
        }

        private void btnFoto_Click(object? sender, EventArgs e)
        {
            int? id = ValitudId();
            if (id == null)
            {
                MessageBox.Show(Keel.T("Vali enne tabelist toode!", "Сначала выберите товар в таблице!", "Select a product in the table first!"));
                return;
            }

            var toode = _db.Toodetabel.Find(id.Value);
            if (toode == null) return;

            using var ofd = new OpenFileDialog();
            ofd.Title = Keel.T($"Vali foto: {toode.Toodenimetus}", $"Выберите фото: {toode.Toodenimetus}", $"Choose photo: {toode.Toodenimetus}");
            ofd.Filter = "Pildid|*.jpg;*.jpeg;*.png";

            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                // Kontroll, et fail on tõesti pilt (muidu poes ei laeks)
                using (var test = Kaustad.LaeFailist(ofd.FileName))
                {
                    if (test == null)
                    {
                        MessageBox.Show(Keel.T("Faili ei saanud pildina avada.", "Не удалось открыть файл как изображение.", "Could not open the file as an image."));
                        return;
                    }
                }

                toode.Pilt = Kaustad.Salvesta(ofd.FileName);
                _db.SaveChanges();           // salvestub kohe, Keel.T("Uuenda", "Обновить", "Update") vajutama ei pea
            }
            catch (Exception ex)
            {
                MessageBox.Show(Keel.T("Foto salvestamine ebaõnnestus: ", "Не удалось сохранить фото: ", "Failed to save photo: ") + ex.Message);
                return;
            }

            valitudPilt = null;
            LaeTooted();
            ValiRida(toode.Id);
            NaitaPilti(Kaustad.LaePilt(toode.Pilt));
            UuendaFotoNupud(toode.Pilt);
        }

        private void btnKustutaFoto_Click(object? sender, EventArgs e)
        {
            int? id = ValitudId();
            if (id == null)
            {
                MessageBox.Show(Keel.T("Vali enne tabelist toode!", "Сначала выберите товар в таблице!", "Select a product in the table first!"));
                return;
            }

            var toode = _db.Toodetabel.Find(id.Value);
            if (toode == null || string.IsNullOrEmpty(toode.Pilt)) return;

            if (MessageBox.Show(Keel.T($"Kas eemaldada toote \"{toode.Toodenimetus}\" foto?", $"Удалить фото товара \"{toode.Toodenimetus}\"?", $"Remove the photo of \"{toode.Toodenimetus}\"?"), Keel.T("Kinnitamine", "Подтверждение", "Confirmation"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            // Failile endale ei puututa (seda võib kasutada mõni teine toode), eemaldame ainult seose.
            toode.Pilt = "";
            _db.SaveChanges();

            valitudPilt = null;
            LaeTooted();
            ValiRida(toode.Id);
            NaitaPilti(null);
            UuendaFotoNupud(null);
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _db.Dispose();   // suleb andmebaasiühenduse
            base.OnFormClosed(e);
        }
    }
}
