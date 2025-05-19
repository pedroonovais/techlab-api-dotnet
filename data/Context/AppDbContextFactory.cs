using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace data.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseOracle(
                "User Id=rm555276;Password=160205;Data Source=//oracle.fiap.com.br:1521/ORCL;",
                x => x.MigrationsAssembly("data")
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
