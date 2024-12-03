using Microsoft.EntityFrameworkCore;

namespace ImageUploadService.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string ThumbnailsJson { get; set; }
    }

}
