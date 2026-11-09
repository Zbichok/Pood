namespace Tooted
{
    /// <summary>
    /// Üks ostukorv kogu rakenduse peale – säilib vormide vahel liikudes.
    /// </summary>
    public static class Ostukorv
    {
        public static List<OstukorviRida> Read { get; } = new List<OstukorviRida>();

        public static float Kokku => Read.Sum(r => r.Summa);

        public static int Tukke => Read.Sum(r => r.Kogus);

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
