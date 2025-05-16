using Microsoft.EntityFrameworkCore;
using library.Model;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Post>().ToTable("POSTS");
            builder.Entity<User>().ToTable("USERS");
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