namespace Tooted
{
    /// <summary>
    /// Üks ostukorv kogu rakenduse peale – säilib vormide vahel liikudes.
    /// </summary>
    public static class Ostukorv
    {
        public const decimal KohaletoimetuseHind = 3.00m;

        public static List<OstukorviRida> Read { get; } = new List<OstukorviRida>();

        public static decimal Vahesumma => Read.Sum(r => r.Summa);

        public static int Tukke => Read.Sum(r => r.Kogus);

        public static decimal Kohaletoimetus => Read.Count == 0 ? 0m : KohaletoimetuseHind;

        public static decimal Kokku => Vahesumma + Kohaletoimetus;

        /// <summary>Read arve all: vahesumma ja kohaletoimetus.</summary>
        public static List<string> Kokkuvote()
        {
            return new List<string>
            {
                Keel.T($"Vahesumma: {Vahesumma:0.00} €", $"Стоимость товаров: {Vahesumma:0.00} €", $"Subtotal: {Vahesumma:0.00} €"),
                Keel.T($"Kohaletoimetus: {Kohaletoimetus:0.00} €", $"Доставка: {Kohaletoimetus:0.00} €", $"Delivery: {Kohaletoimetus:0.00} €")
            };
        }

        public static bool Lisa(Toode toode, int kogus, out string? viga)
        {
            viga = null;

            if (kogus <= 0)
            {
                viga = Keel.T("Vale kogus.", "Неверное количество.", "Invalid quantity.");
                return false;
            }

            var olemas = Read.FirstOrDefault(r => r.Toode.Id == toode.Id);
            int kokkuKorvis = (olemas?.Kogus ?? 0) + kogus;

            if (kokkuKorvis > toode.Kogus)
            {
                viga = Keel.T($"Laos on ainult {toode.Kogus} tk.", $"В наличии только {toode.Kogus} шт.", $"Only {toode.Kogus} pcs in stock.");
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
