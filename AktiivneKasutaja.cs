namespace Tooted
{
    public static class AktiivneKasutaja
    {
        public static string Nimi { get; set; }
        public static string Roll { get; set; }
        public static string Email { get; set; }

        public static bool OnAdmin => Roll == "admin";

        public static void LogiValja()
        {
            Nimi = null;
            Roll = null;
            Email = null;
        }
    }
}
