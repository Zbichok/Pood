using System;
using System.Drawing;
using System.IO;

namespace Tooted
{
    /// <summary>
    /// Piltide ja arvete kaustad. Kõik hoitakse kasutaja kaustas AppData\Local\ToodedApp,
    /// sest Program Files kausta (kuhu installer rakenduse paigaldab) tavakasutaja kirjutada ei saa.
    /// </summary>
    public static class Kaustad
    {
        public static readonly string Juur = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ToodedApp");
        public static readonly string Pildid = Path.Combine(Juur, "Images");
        public static readonly string Arved = Path.Combine(Juur, "Arved");

        /// <summary>Loob kaustad, kui neid veel pole, ja kopeerib rakendusega kaasas olevad demo-pildid.</summary>
        public static void Loo()
        {
            Directory.CreateDirectory(Pildid);
            Directory.CreateDirectory(Arved);

            string allikas = Path.Combine(AppContext.BaseDirectory, "Images");
            if (!Directory.Exists(allikas)) return;

            foreach (string fail in Directory.GetFiles(allikas))
            {
                string siht = Path.Combine(Pildid, Path.GetFileName(fail));
                if (!File.Exists(siht))
                    File.Copy(fail, siht);
            }
        }

        /// <summary>Laeb toote pildi (failinimi andmebaasist) kaustast Images. Kui faili pole, tagastab null.</summary>
        public static Image? LaePilt(string? failinimi)
        {
            if (string.IsNullOrEmpty(failinimi)) return null;
            return LaeFailist(Path.Combine(Pildid, failinimi));
        }

        /// <summary>Laeb pildi koopiana, et fail ei jääks lukku.</summary>
        public static Image? LaeFailist(string tee)
        {
            if (!File.Exists(tee)) return null;

            using var voog = new MemoryStream(File.ReadAllBytes(tee));
            using var ajutine = Image.FromStream(voog);
            return new Bitmap(ajutine);
        }

        /// <summary>Kopeerib valitud pildi kausta Images ja tagastab faili nime (see läheb andmebaasi).</summary>
        public static string Salvesta(string allikas)
        {
            string failinimi = Path.GetFileName(allikas);
            string siht = Path.Combine(Pildid, failinimi);

            // Fail on juba Images kaustas (nt valiti sealt) – kopeerida pole vaja
            if (string.Equals(Path.GetFullPath(allikas), Path.GetFullPath(siht),
                    StringComparison.OrdinalIgnoreCase))
                return failinimi;

            // Sama nimega fail on olemas, aga see on teine pilt: ei kirjuta üle,
            // muidu muutuks ka teise toote foto. Teeme unikaalse nime (nimi_1.png jne).
            if (File.Exists(siht))
            {
                string nimi = Path.GetFileNameWithoutExtension(failinimi);
                string laiend = Path.GetExtension(failinimi);
                int n = 1;
                do
                {
                    failinimi = $"{nimi}_{n++}{laiend}";
                    siht = Path.Combine(Pildid, failinimi);
                } while (File.Exists(siht));
            }

            File.Copy(allikas, siht);
            return failinimi;
        }
    }
}
