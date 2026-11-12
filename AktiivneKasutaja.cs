namespace Tooted
{
    public static class AktiivneKasutaja
    {
        public static string Nimi { get; set; }
        public static string Roll { get; set; }
        public static string Email { get; set; }

        // Boonused (laetakse andmebaasist, vt LaeBoonused)
        public static decimal Saldo { get; private set; }
        public static int Soodustus { get; private set; }
        public static bool TasutaKohaletoimetus { get; private set; }
        public static Toode? TasutaToode { get; private set; }
        public static DateTime? ViimanePoore { get; private set; }

        public static bool OnAdmin => Roll == "admin";

        /// <summary>Kõik (ka admin) saavad rulli keerata kord päevas, boonus jääb ainult sellele kontole.</summary>
        public static bool VoibKeerata => ViimanePoore?.Date != DateTime.Today;

        public static void LaeBoonused()
        {
            if (string.IsNullOrEmpty(Email)) return;

            using var db = new ToodedContext();
            var k = db.Kasutajad.FirstOrDefault(x => x.Email == Email);
            if (k == null) return;

            Saldo = k.Saldo;
            Soodustus = k.Soodustus;
            TasutaKohaletoimetus = k.TasutaKohaletoimetus;
            ViimanePoore = k.ViimanePoore;
            TasutaToode = k.TasutaToodeId == null
                ? null
                : db.Toodetabel.FirstOrDefault(t => t.Id == k.TasutaToodeId);
        }

        public static void LogiValja()
        {
            Nimi = null;
            Roll = null;
            Email = null;
            Saldo = 0;
            Soodustus = 0;
            TasutaKohaletoimetus = false;
            TasutaToode = null;
            ViimanePoore = null;
        }
    }
}
