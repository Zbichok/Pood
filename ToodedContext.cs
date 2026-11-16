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
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=Tooded_DB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kategooria>().ToTable("Kategooriatabel");
            modelBuilder.Entity<Toode>().ToTable("Toodetabel");
            modelBuilder.Entity<Kasutaja>().ToTable("Kasutajad");
            modelBuilder.Entity<Kasutaja>().Property(k => k.Saldo).HasPrecision(10, 2);
            modelBuilder.Entity<Toode>().Property(t => t.Hind).HasPrecision(10, 2);

            // Sama e-postiga kahte kasutajat ei saa olla (450 = SQL Serveri indeksi pikkuse piir)
            modelBuilder.Entity<Kasutaja>().Property(k => k.Email).HasMaxLength(450);
            modelBuilder.Entity<Kasutaja>().HasIndex(k => k.Email).IsUnique();
        }
    }
}
