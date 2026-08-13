using AuthService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(e => e.Id);
            modelBuilder.Entity<User>().HasIndex(e => e.Name).IsUnique();
            modelBuilder.Entity<User>().HasIndex(e => e.Email).IsUnique();
        }
    }
}
