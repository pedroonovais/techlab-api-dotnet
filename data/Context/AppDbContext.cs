using Microsoft.EntityFrameworkCore;
using library.Model;

namespace data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Moto> Moto { get; set; }
        public DbSet<Patio> Patio { get; set; }
        public DbSet<Perfil> Perfil { get; set; }
        public DbSet<Rastreador> Rastreador { get; set; }
        public DbSet<StatusOperacional> StatusOperacional { get; set; }
        public DbSet<Usuario> Usuario { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Moto>().ToTable("MOTO");
            builder.Entity<Patio>().ToTable("PATIO");
            builder.Entity<Perfil>().ToTable("PERFIL");
            builder.Entity<Rastreador>().ToTable("RASTREADOR");
            builder.Entity<StatusOperacional>().ToTable("STATUSOPERACIONAL");
            builder.Entity<Usuario>().ToTable("USUARIO");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "Host=localhost;Port=5432;Database=techlab;Username=postgres;Password=postgres",
                    b => b.MigrationsAssembly("data")
                );
            }
        }
    }
}
