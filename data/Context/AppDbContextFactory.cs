using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace data.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=techlab;Username=postgres;Password=postgres",
                x => x.MigrationsAssembly("data")
            );
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
