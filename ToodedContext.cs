using Microsoft.EntityFrameworkCore;

namespace Tooted
{
    public class ToodedContext : DbContext
    {
        public DbSet<Toode> Toodetabel { get; set; }
        public DbSet<Kategooria> Kategooriatabel { get; set; }
        public DbSet<Kasutaja> Kasutajad { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // SQLite: andmebaas on üks fail kasutaja kaustas, eraldi serverit pole vaja
            Directory.CreateDirectory(Kaustad.Juur);
            optionsBuilder.UseSqlite($"Data Source={Path.Combine(Kaustad.Juur, "tooded.db")}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kategooria>().ToTable("Kategooriatabel");
            modelBuilder.Entity<Toode>().ToTable("Toodetabel");
            modelBuilder.Entity<Kasutaja>().ToTable("Kasutajad");
            modelBuilder.Entity<Toode>().Property(t => t.Hind).HasPrecision(10, 2);

            // Sama e-postiga kahte kasutajat ei saa olla (450 = SQL Serveri indeksi pikkuse piir, SQLite'is pole vaja, aga ei sega)
            modelBuilder.Entity<Kasutaja>().Property(k => k.Email).HasMaxLength(450);
            modelBuilder.Entity<Kasutaja>().HasIndex(k => k.Email).IsUnique();
        }
    }
}
