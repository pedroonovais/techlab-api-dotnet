using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using library.Model;

namespace data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Moto> Moto { get; set; }
        public DbSet<Patio> Patio { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Sensor> Sensor { get; set; }
        public DbSet<LeituraRfid> LeituraRfid { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Moto>().ToTable("MOTO");
            builder.Entity<Patio>().ToTable("PATIO");
            builder.Entity<Usuario>().ToTable("USUARIO");
            builder.Entity<Sensor>().ToTable("SENSOR");
            builder.Entity<LeituraRfid>().ToTable("LEITURARFID");

            // Corrige tipo BOOLEAN para Oracle (usa NUMBER(1))
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool))
                    {
                        property.SetColumnType("NUMBER(1)");
                    }
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseOracle(
                    "conn",
                    b => b.MigrationsAssembly("api")
                );
            }
        }
    }
}