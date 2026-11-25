namespace Tooted;

public partial class Form2
{
    private void Paiguta()
    {
        ClientSize = new Size(1000, 650);
        label1.Location = new Point(20, 18);
        txtNimetus.SetBounds(20, 42, 440, 28);
        label2.Location = new Point(20, 82);
        txtKogus.SetBounds(20, 106, 210, 28);
        label3.Location = new Point(250, 82);
        txtHind.SetBounds(250, 106, 210, 28);
        label4.Location = new Point(20, 146);
        cmbKategooria.SetBounds(20, 170, 440, 28);
        btnLisaKategooria.SetBounds(20, 212, 210, 38);
        btnKustutaKategooria.SetBounds(250, 212, 210, 38);
        picPilt.SetBounds(490, 20, 280, 230);
        btnOtsi.SetBounds(790, 42, 190, 38);
        btnFoto.SetBounds(790, 94, 190, 38);
        btnKustutaFoto.SetBounds(790, 146, 190, 38);
        btnLisa.SetBounds(20, 272, 150, 36);
        btnUuenda.SetBounds(182, 272, 150, 36);
        btnKustuta.SetBounds(344, 272, 150, 36);
        Controls["btnTyhjenda"]?.SetBounds(506, 272, 190, 36);
        dataGridTooted.SetBounds(20, 326, 960, 260);
        dataGridTooted.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        Controls["btnTagasi"]?.SetBounds(20, 604, 190, 34);
        dataGridTooted.DataBindingComplete += (_, _) => TolgiVeerud();
        TolgiVeerud();
    }

    private void TolgiVeerud()
    {
        foreach (DataGridViewColumn c in dataGridTooted.Columns)
            c.HeaderText = c.Name switch
            {
                "Toodenimetus" => Keel.T("Toode nimetus", "Название товара", "Product name"),
                "Kogus" => Keel.T("Kogus", "Количество", "Quantity"),
                "Hind" => Keel.T("Hind", "Цена", "Price"),
                "Kategooria" => Keel.T("Kategooria", "Категория", "Category"),
                "Pilt" => Keel.T("Pilt", "Фото", "Image"),
                _ => c.HeaderText
            };
    }
}

public partial class Form3
{
    private void Paiguta()
    {
        ClientSize = new Size(940, 660);
        lblKorv.Location = new Point(20, 20);
        Controls["btnTagasi"]?.SetBounds(720, 14, 200, 34);
        tabpood.SetBounds(20, 64, 900, 530);
        lblMarkus.AutoSize = false;
        lblMarkus.SetBounds(20, 612, 900, 36);
    }
}

public partial class ArveForm
{
    private void Paiguta()
    {
        ClientSize = new Size(860, 600);
        lblOstukorv.Location = new Point(20, 20);
        listOstukorv.SetBounds(20, 50, 820, 290);
        btnEemalda.SetBounds(20, 356, 230, 36);
        btnGenPDF.SetBounds(265, 356, 230, 36);
        btnSaadaEmail.SetBounds(510, 356, 330, 36);
        lblKokkuvote.SetBounds(20, 414, 520, 70);
        lblKokku.Location = new Point(20, 492);
        btnMaksa.SetBounds(610, 432, 230, 48);
        Controls["btnTagasi"]?.SetBounds(610, 494, 230, 34);
        lblMarkus.AutoSize = false;
        lblMarkus.SetBounds(20, 550, 820, 38);
        Keel.Muutus += UuendaOstukorvi;
        FormClosed += (_, _) => Keel.Muutus -= UuendaOstukorvi;
    }
}
