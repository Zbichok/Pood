namespace Tooted
{
    public class OstukorviRida
    {
        public Toode Toode { get; set; }
        public int Kogus { get; set; }

        public decimal Summa => Toode.Hind * Kogus;

        public override string ToString()
        {
            return Keel.T($"{Toode.Toodenimetus} – {Kogus} tk – {Summa:0.00} €", $"{Toode.Toodenimetus} — {Kogus} шт. — {Summa:0.00} €", $"{Toode.Toodenimetus} — {Kogus} pcs — {Summa:0.00} €");
        }
    }
}
