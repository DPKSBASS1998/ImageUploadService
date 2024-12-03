using Microsoft.EntityFrameworkCore;

namespace ImageUploadService.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Вказуємо назву таблиці в базі даних
            modelBuilder.Entity<Image>().ToTable("images");
        }
    }
}
