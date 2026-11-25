namespace Tooted;

// UI translations only. Database values and business rules remain unchanged.
public static class Keel
{
    public static int Praegune { get; private set; }
    private static readonly Dictionary<string, string[]> Tekstid = new();
    private static readonly List<Form> Vormid = new();
    public static event Action? Muutus;

    public static string T(string et, string ru, string en)
    {
        var texts = new[] { et, ru, en };
        foreach (var text in texts)
            if (text.Length > 0) Tekstid[text] = texts;
        return texts[Praegune];
    }

    public static string Tolgi(string text)
    {
        if (Tekstid.TryGetValue(text, out var texts)) return texts[Praegune];
        return string.Join("\n", text.Split('\n').Select(line =>
        {
            var clean = line.TrimEnd('\r');
            return Tekstid.TryGetValue(clean, out var parts) ? parts[Praegune] : clean;
        }));
    }

    public static void Vali(int keel)
    {
        if (keel < 0 || keel > 2 || keel == Praegune) return;
        Praegune = keel;
        foreach (var form in Vormid.ToArray())
            if (!form.IsDisposed) Uuenda(form);
        Muutus?.Invoke();
    }

    private static void Uuenda(Control control)
    {
        if ((control is Form || control is Label || control is Button) && !Equals(control.Tag, "product-data"))
            control.Text = Tolgi(control.Text);
        if (control is ComboBox combo && combo.Name == "keeleValik")
            combo.SelectedIndex = Praegune;
        if (control is DataGridView grid)
            foreach (DataGridViewColumn column in grid.Columns)
                column.HeaderText = Tolgi(column.HeaderText);
        foreach (Control child in control.Controls) Uuenda(child);
    }

    public static void Lisa(Form form)
    {
        if (Vormid.Contains(form)) return;
        Vormid.Add(form);
        form.FormClosed += (_, _) => Vormid.Remove(form);
        form.Load += (_, _) =>
        {
            if (form is Form2 || form is Form3 || form is ArveForm ||
                form is FormMain || form is LoginForm || form is RegistreerimisForm)
            {
                var y = form.ClientSize.Height;
                form.ClientSize = new Size(form.ClientSize.Width, y + 42);
                var combo = new ComboBox
                {
                    Name = "keeleValik", DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(form.ClientSize.Width - 188, y + 8),
                    Size = new Size(170, 25), TabIndex = 100
                };
                combo.Items.AddRange(new object[] { "Eesti", "Русский", "English" });
                combo.SelectedIndex = Praegune;
                combo.SelectedIndexChanged += (_, _) => Vali(combo.SelectedIndex);
                form.Controls.Add(combo);
            }
            Uuenda(form);
        };
    }
}
