using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Text;

namespace Tooted
{
    /// <summary>
    /// Ühe ostu "hetktõmmis": read, summad, kuller. Võetakse enne ostukorvi tühjendamist,
    /// et tšekk jääks õigeks. Aadress elab ainult selle objekti sees (mälus) – andmebaasi ei kirjutata.
    /// </summary>
    public class Tellimus
    {
        public int Number { get; init; }
        public DateTime Aeg { get; init; } = DateTime.Now;
        public string Klient { get; init; } = "";
        public string? Aadress { get; init; }
        public List<string> Read { get; init; } = new();
        public List<string> Kokkuvote { get; init; } = new();
        public decimal Kokku { get; init; }
        public int Tukke { get; init; }

        /// <summary>Kulleri hinnanguline saabumisaeg minutites (0 = kohaletoimetuse infot pole).</summary>
        public int KullerMin { get; init; }

        public decimal? Makstud { get; set; }

        public DateTime? KullerSaabub => KullerMin > 0 ? Aeg.AddMinutes(KullerMin) : null;

        /// <summary>
        /// Mida rohkem tükke, seda kõrgem prioriteet ja seda kiirem kohaletoimetus.
        /// 1-4 tk: 45-75 min, 5-9 tk: 30-50 min, 10+ tk: 20-35 min.
        /// </summary>
        public static (int Min, int Max) KullerVahemik(int tukke)
        {
            if (tukke <= 4) return (45, 75);
            if (tukke <= 9) return (30, 50);
            return (20, 35);
        }

        public static Tellimus Praegusest(string? aadress = null, bool kuller = false)
        {
            int tukke = Ostukorv.Tukke;
            int kullerMin = 0;
            if (kuller)
            {
                var (min, max) = KullerVahemik(tukke);
                kullerMin = Random.Shared.Next(min, max + 1);
            }

            return new Tellimus
            {
                Number = Random.Shared.Next(100000, 1000000),
                Aeg = DateTime.Now,
                Klient = AktiivneKasutaja.Nimi ?? "",
                Aadress = aadress,
                Read = Ostukorv.Read.Select(r => r.ToString()).ToList(),
                Kokkuvote = Ostukorv.Kokkuvote(),
                Kokku = Ostukorv.Kokku,
                Tukke = tukke,
                KullerMin = kullerMin
            };
        }

        public string KullerTekst =>
            KullerMin > 0
                ? Keel.T($"Kuller jõuab umbes {KullerMin} min pärast (u kell {KullerSaabub:HH:mm}).", $"Курьер прибудет через {KullerMin} мин (около {KullerSaabub:HH:mm}).", $"Courier arrives in about {KullerMin} min (around {KullerSaabub:HH:mm}).")
                : "";

        /// <summary>Tšeki tekst ekraani jaoks.</summary>
        public string Tekst()
        {
            var sb = new StringBuilder();
            string nl = Environment.NewLine;

            sb.Append(Keel.T($"ARVE nr {Number}", $"СЧЁТ № {Number}", $"INVOICE no. {Number}")).Append(nl);
            sb.Append(Keel.T($"Kuupäev: {Aeg:dd.MM.yyyy HH:mm}", $"Дата: {Aeg:dd.MM.yyyy HH:mm}", $"Date: {Aeg:dd.MM.yyyy HH:mm}")).Append(nl);
            sb.Append(Keel.T($"Klient: {Klient}", $"Клиент: {Klient}", $"Customer: {Klient}")).Append(nl);
            if (!string.IsNullOrWhiteSpace(Aadress))
                sb.Append(Keel.T($"Aadress: {Aadress}", $"Адрес: {Aadress}", $"Address: {Aadress}")).Append(nl);
            sb.Append(new string('-', 44)).Append(nl);

            foreach (var rida in Read)
                sb.Append(rida).Append(nl);

            sb.Append(new string('-', 44)).Append(nl);
            foreach (var rida in Kokkuvote)
                sb.Append(rida).Append(nl);

            sb.Append(Keel.T($"KOKKU: {Kokku:0.00} €", $"ИТОГО: {Kokku:0.00} €", $"TOTAL: {Kokku:0.00} €")).Append(nl);

            if (Makstud != null)
                sb.Append(Keel.T($"Makstud: {Makstud:0.00} €", $"Оплачено: {Makstud:0.00} €", $"Paid: {Makstud:0.00} €")).Append(nl);

            if (KullerMin > 0)
            {
                sb.Append(nl);
                sb.Append(KullerTekst).Append(nl);
            }

            return sb.ToString();
        }
    }

    public static class ChekiPdf
    {
        // Unicode kodeering, et õ, ä, ö, ü ja € trükiksid PDF-is õigesti
        private static XFont Fnt(double suurus) =>
            new XFont("Verdana", suurus, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode));

        public static void Loo(Tellimus t, string tee)
        {
            string? kaust = Path.GetDirectoryName(tee);
            if (!string.IsNullOrEmpty(kaust))
                Directory.CreateDirectory(kaust);

            var document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = Fnt(12);
            XFont suur = Fnt(14);
            double y = 20;

            void UusLeht()
            {
                gfx.Dispose();
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                y = 40;
            }

            void Joonista(string tekst, XFont f, double samm)
            {
                if (y > page.Height - 60)
                    UusLeht();

                gfx.DrawString(tekst, f, XBrushes.Black,
                    new XRect(20, y, page.Width - 40, 20), XStringFormats.TopLeft);
                y += samm;
            }

            gfx.DrawString(Keel.T($"Arve nr {t.Number}", $"Счёт № {t.Number}", $"Invoice no. {t.Number}"), Fnt(20), XBrushes.Black,
                new XRect(0, y, page.Width, 40), XStringFormats.TopCenter);
            y = 60;

            Joonista(Keel.T($"Kuupäev: {t.Aeg:dd.MM.yyyy HH:mm}", $"Дата: {t.Aeg:dd.MM.yyyy HH:mm}", $"Date: {t.Aeg:dd.MM.yyyy HH:mm}"), font, 20);
            Joonista(Keel.T($"Klient: {t.Klient}", $"Клиент: {t.Klient}", $"Customer: {t.Klient}"), font, 20);
            if (!string.IsNullOrWhiteSpace(t.Aadress))
                Joonista(Keel.T($"Aadress: {t.Aadress}", $"Адрес: {t.Aadress}", $"Address: {t.Aadress}"), font, 20);
            if (t.KullerMin > 0)
                Joonista(t.KullerTekst, font, 20);

            y += 15;
            foreach (var rida in t.Read)
                Joonista(rida, font, 30);

            y += 10;
            foreach (var rida in t.Kokkuvote)
                Joonista(rida, font, 22);

            y += 6;
            Joonista(Keel.T($"Kokku: {t.Kokku:0.00} €", $"Итого: {t.Kokku:0.00} €", $"Total: {t.Kokku:0.00} €"), suur, 26);

            if (t.Makstud != null)
                Joonista(Keel.T($"Makstud: {t.Makstud:0.00} €", $"Оплачено: {t.Makstud:0.00} €", $"Paid: {t.Makstud:0.00} €"), font, 22);

            gfx.Dispose();
            document.Save(tee);
        }
    }
}
