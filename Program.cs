using Microsoft.EntityFrameworkCore;

namespace Tooted
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Kaustad.Loo();

            try
            {
                LooAndmebaasJaKasutajad();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Andmebaasi viga: " + ex.Message +
                    "\n\nKas kaust Migrations on olemas (Add-Migration InitialCreate)?");
                return;
            }

            // Tsükkel: logi sisse -> peamenüü -> "Logi välja" viib tagasi sisselogimisele.
            while (true)
            {
                using (var login = new LoginForm())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                        break;
                }

                Ostukorv.Tuhjenda();

                using var peavorm = new FormMain();
                Application.Run(peavorm);

                if (!peavorm.LogiValja)
                    break;

                // Uus kasutaja alustab tühja ostukorviga
                AktiivneKasutaja.LogiValja();
                Ostukorv.Tuhjenda();
            }
        }

        private static void LooAndmebaasJaKasutajad()
        {
            using var db = new ToodedContext();
            db.Database.Migrate();

            if (!db.Kasutajad.Any())
            {
                db.Kasutajad.Add(new Kasutaja
                {
                    Nimi = "Admin",
                    Email = "admin@admin.ee",
                    ParoolHash = PasswordHelper.Hash("admin123"),
                    Roll = "admin"
                });

                db.Kasutajad.Add(new Kasutaja
                {
                    Nimi = "Kasutaja",
                    Email = "kasutaja@kasutaja.ee",
                    ParoolHash = PasswordHelper.Hash("kasutaja123"),
                    Roll = "kasutaja"
                });

                db.SaveChanges();
            }

            SeedAndmed.Lisa(db);
        }
    }
}
