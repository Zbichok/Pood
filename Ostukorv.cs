namespace Tooted
{
    /// <summary>
    /// Üks ostukorv kogu rakenduse peale – säilib vormide vahel liikudes.
    /// Boonused (soodustus, tasuta toode, tasuta kohaletoimetus) tulevad AktiivneKasutaja käest.
    /// </summary>
    public static class Ostukorv
    {
        public const float KohaletoimetuseHind = 3.00f;

        public static List<OstukorviRida> Read { get; } = new List<OstukorviRida>();

        public static float Vahesumma => Read.Sum(r => r.Summa);

        public static int Tukke => Read.Sum(r => r.Kogus);

        /// <summary>Korvis olev rida, mille üks tükk on boonusena tasuta.</summary>
        public static OstukorviRida? TasutaRida
        {
            get
            {
                var t = AktiivneKasutaja.TasutaToode;
                return t == null ? null : Read.FirstOrDefault(r => r.Toode.Id == t.Id);
            }
        }

        public static float TasutaToodeSumma => TasutaRida?.Toode.Hind ?? 0f;

        public static float SoodustuseSumma =>
            (Vahesumma - TasutaToodeSumma) * AktiivneKasutaja.Soodustus / 100f;

        public static float Kohaletoimetus =>
            Read.Count == 0 || AktiivneKasutaja.TasutaKohaletoimetus ? 0f : KohaletoimetuseHind;

        public static float Kokku => Vahesumma - TasutaToodeSumma - SoodustuseSumma + Kohaletoimetus;

        /// <summary>Read arve all: vahesumma, boonused, kohaletoimetus.</summary>
        public static List<string> Kokkuvote()
        {
            var read = new List<string> { $"Vahesumma: {Vahesumma:0.00} €" };

            var tasuta = TasutaRida;
            if (tasuta != null)
                read.Add($"Tasuta toode ({tasuta.Toode.Toodenimetus}): -{TasutaToodeSumma:0.00} €");

            if (AktiivneKasutaja.Soodustus > 0)
                read.Add($"Soodustus -{AktiivneKasutaja.Soodustus}%: -{SoodustuseSumma:0.00} €");

            read.Add(AktiivneKasutaja.TasutaKohaletoimetus
                ? "Kohaletoimetus: tasuta"
                : $"Kohaletoimetus: {Kohaletoimetus:0.00} €");

            return read;
        }

        public static bool Lisa(Toode toode, int kogus, out string? viga)
        {
            viga = null;

            if (kogus <= 0)
            {
                viga = "Vale kogus.";
                return false;
            }

            var olemas = Read.FirstOrDefault(r => r.Toode.Id == toode.Id);
            int kokkuKorvis = (olemas?.Kogus ?? 0) + kogus;

            if (kokkuKorvis > toode.Kogus)
            {
                viga = $"Laos on ainult {toode.Kogus} tk.";
                return false;
            }

            if (olemas != null)
                olemas.Kogus = kokkuKorvis;
            else
                Read.Add(new OstukorviRida { Toode = toode, Kogus = kogus });

            return true;
        }

        public static void Eemalda(OstukorviRida rida) => Read.Remove(rida);

        public static void Tuhjenda() => Read.Clear();
    }
}
