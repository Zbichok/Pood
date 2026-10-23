using Microsoft.EntityFrameworkCore;

namespace Tooted
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                LooAndmebaasJaKasutajad();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Andmebaasi viga: " + ex.Message +
                    "\n\nKas tegid Add-Migration LisaKasutajad ja Update-Database?");
                return;
            }

            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return;
            }

            Application.Run(new Peavorm());
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

            if (!db.Kasutajad.Any(k => k.Email.ToLower() == "timoha@pro.ee"))
            {
                db.Kasutajad.Add(new Kasutaja
                {
                    Nimi = "Timoha",
                    Email = "Timoha@pro.ee",
                    ParoolHash = PasswordHelper.Hash("123pro"),
                    Roll = "admin"
                });

                db.SaveChanges();
            }
        }
    }
}
