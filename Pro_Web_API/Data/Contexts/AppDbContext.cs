using Microsoft.EntityFrameworkCore;
using Pro_Web_API.Core.Entities;

namespace Pro_Web_API.Data.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApiLog> ApiLogs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=ProfenDB;Trusted_Connection=True;MultipleActiveResultSets=true";

            optionsBuilder.UseSqlServer(connectionString);
        }



 protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                 .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd(); 

            modelBuilder.Entity<User>()
                .HasIndex(u => u.email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.user_Name)
                .IsUnique();

            modelBuilder.Entity<Category>()
              .HasKey(u => u.Id);

            modelBuilder.Entity<Category>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd(); 

            modelBuilder.Entity<Category>()
                .HasIndex(u => u.category_name)
                .IsUnique();
        }
    }
}
