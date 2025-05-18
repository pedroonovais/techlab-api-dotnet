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


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Moto>().ToTable("MOTO");
            builder.Entity<Patio>().ToTable("PATIO");
            builder.Entity<Usuario>().ToTable("USUARIO");
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