namespace Tooted
{
    /// <summary>
    /// Demo-kategooriad ja tooted. Lisatakse ainult siis, kui sellist kategooriat/toodet veel pole.
    /// Pildid peavad olema kaustas Images (nt cocacola.png, fanta.png). Kui faili pole, jääb pilt tühjaks.
    /// </summary>
    public static class SeedAndmed
    {
        private record SeedToode(string Nimi, int Kogus, decimal Hind, string Pilt);
        private record SeedKategooria(string Nimi, string Kirjeldus, SeedToode[] Tooted);

        private static readonly SeedKategooria[] Andmed =
        {
            new("Joogid", "Karastus- ja mahlajoogid", new[]
            {
                new SeedToode("Coca-Cola 0,5L", 40, 1.49m, "cocacola.png"),
                new SeedToode("Fanta 0,5L", 35, 1.39m, "fanta.png"),
                new SeedToode("Sprite 0,5L", 30, 1.39m, "sprite.png"),
                new SeedToode("Õunamahl 1L", 25, 1.99m, "Ounamahl.png"),
                new SeedToode("Mineraalvesi 1,5L", 50, 0.89m, "mineraalvesi.png"),
            }),
            new("Maiustused", "Šokolaad ja kommid", new[]
            {
                new SeedToode("Piimašokolaad 100g", 60, 1.79m, "piimasokolaad.png"),
                new SeedToode("Tumešokolaad 100g", 45, 2.19m, "tumesokolaad.png"),
                new SeedToode("Kommipakk 250g", 30, 3.49m, "kommipakk.png"),
                new SeedToode("Marsipan 150g", 20, 2.99m, "marsipan.png"),
            }),
            new("Puuviljad", "Värsked puu- ja juurviljad", new[]
            {
                new SeedToode("Õunad 1kg", 50, 1.69m, "ounad.png"),
                new SeedToode("Banaanid 1kg", 40, 1.59m, "banaanid.png"),
                new SeedToode("Apelsinid 1kg", 35, 2.29m, "apelsinid.png"),
                new SeedToode("Viinamarjad 500g", 25, 2.99m, "viinamarjad.png"),
            }),
            new("Piimatooted", "Piim, juust ja jogurt", new[]
            {
                new SeedToode("Piim 2,5% 1L", 45, 1.09m, "piim.png"),
                new SeedToode("Gouda juust 200g", 30, 3.29m, "juust.png"),
                new SeedToode("Maasikajogurt 150g", 40, 0.79m, "jogurt.png"),
                new SeedToode("Või 200g", 35, 2.49m, "voi.png"),
            }),
            new("Pagaritooted", "Värske leib ja saiakesed", new[]
            {
                new SeedToode("Rukkileib", 30, 1.59m, "rukkileib.png"),
                new SeedToode("Sai", 40, 0.99m, "sai.png"),
                new SeedToode("Croissant", 25, 1.29m, "croissant.png"),
                new SeedToode("Kaneelisnekk", 20, 1.49m, "kaneelisnekk.png"),
            }),
        };

        public static void Lisa(ToodedContext db)
        {
            foreach (var k in Andmed)
            {
                var kategooria = db.Kategooriatabel
                    .FirstOrDefault(x => x.Kategooria_nimetus.ToLower() == k.Nimi.ToLower());

                if (kategooria == null)
                {
                    kategooria = new Kategooria
                    {
                        Kategooria_nimetus = k.Nimi,
                        Kirjeldus = k.Kirjeldus
                    };
                    db.Kategooriatabel.Add(kategooria);
                    db.SaveChanges();
                }

                foreach (var t in k.Tooted)
                {
                    var olemas = db.Toodetabel.FirstOrDefault(x =>
                        x.KategooriaId == kategooria.Id &&
                        x.Toodenimetus.ToLower() == t.Nimi.ToLower());

                    if (olemas != null)
                    {
                        // Parandame vanad "seed_*.png" nimed (ja tühjad pildid) uute vastu.
                        // Admini enda valitud pilte ei puututa.
                        if (string.IsNullOrEmpty(olemas.Pilt) ||
                            olemas.Pilt.StartsWith("seed_", StringComparison.OrdinalIgnoreCase))
                        {
                            olemas.Pilt = t.Pilt;
                        }
                        continue;
                    }

                    db.Toodetabel.Add(new Toode
                    {
                        Toodenimetus = t.Nimi,
                        Kogus = t.Kogus,
                        Hind = t.Hind,
                        Pilt = t.Pilt,
                        KategooriaId = kategooria.Id
                    });
                }

                db.SaveChanges();
            }
        }
    }
}
