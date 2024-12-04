using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace ImageUploadService.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Image>()
                .ToTable("images")
                .Property(i => i.Path)
                .HasColumnType("text");  // Використовуємо текстовий тип для PostgreSQL

            modelBuilder.Entity<Image>()
                .Property(i => i.ThumbnailsJson)
                .HasColumnType("text");  // Використовуємо текстовий тип для PostgreSQL
        }
    }
}
