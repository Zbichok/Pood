namespace Tooted
{
    public class Kasutaja
    {
        public int Id { get; set; }
        public string Nimi { get; set; }
        public string Email { get; set; }
        public string ParoolHash { get; set; }
        public string Roll { get; set; }

        // Päevane rull ja boonused
        public decimal Saldo { get; set; }
        public DateTime? ViimanePoore { get; set; }
        public int Soodustus { get; set; }                 // % järgmisele ostule
        public bool TasutaKohaletoimetus { get; set; }
        public int? TasutaToodeId { get; set; }
    }
}
