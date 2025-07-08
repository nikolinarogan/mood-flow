using Microsoft.EntityFrameworkCore;
using MoodFlow.Models;

namespace MoodFlow.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<DiaryItem> DiaryItems { get; set; }
        public DbSet<Quote> Quotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<DiaryItem>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(di => di.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiaryItem>()
                .HasIndex(di => new { di.UserId, di.CreatedAt })
                .IsUnique();
        }
    }
}