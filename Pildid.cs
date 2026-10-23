using System;
using System.Drawing;
using System.IO;

namespace Tooted
{
    public static class Pildid
    {
        public static string Kaust
        {
            get
            {
                string projektiKaust = Path.GetFullPath(
                    Path.Combine(AppContext.BaseDirectory, @"..\..\..\Images"));

                if (Directory.Exists(projektiKaust))
                    return projektiKaust;

                string kaust = Path.Combine(AppContext.BaseDirectory, "Images");
                Directory.CreateDirectory(kaust);
                return kaust;
            }
        }

        public static Image? Lae(string? failinimi)
        {
            if (string.IsNullOrEmpty(failinimi)) return null;

            string tee = Path.Combine(Kaust, failinimi);
            if (!File.Exists(tee))
                tee = Path.Combine(AppContext.BaseDirectory, "Images", failinimi);

            return LaeFailist(tee);
        }

        public static Image? LaeFailist(string tee)
        {
            if (!File.Exists(tee)) return null;

            return Image.FromStream(new MemoryStream(File.ReadAllBytes(tee)));
        }

        public static string Salvesta(string allikas)
        {
            string failinimi = Path.GetFileName(allikas);
            string siht = Path.Combine(Kaust, failinimi);

            if (!string.Equals(Path.GetFullPath(allikas), Path.GetFullPath(siht),
                    StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(allikas, siht, true);
            }

            return failinimi;
        }
    }
}
