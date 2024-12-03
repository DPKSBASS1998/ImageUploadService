using Microsoft.EntityFrameworkCore;

namespace ImageUploadService.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Image> Images { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
