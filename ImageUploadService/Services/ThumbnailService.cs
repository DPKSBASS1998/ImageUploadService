using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace ImageUploadService.Services
{
    public class ThumbnailService
    {
        private readonly string _thumbnailsDirectory;

        public ThumbnailService(string thumbnailsDirectory)
        {
            // Вказуємо папку для збереження ескізів всередині Docker volume
            _thumbnailsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "thumbnails");

            if (!Directory.Exists(_thumbnailsDirectory))
            {
                Directory.CreateDirectory(_thumbnailsDirectory);  // Якщо директорії немає, створюємо її
            }
        }

        public async Task<List<string>> GenerateThumbnailsAsync(string originalImagePath, string originalFileName)
        {
            var image = await Task.Run(() => Image.Load(originalImagePath));  // Завантажуємо зображення через ImageSharp

            // Шляхи до ескізів (всі ескізи зберігаються в папці wwwroot/thumbnails)
            var thumbnails = new List<string>
            {
                $"{originalFileName}_small.jpg",
                $"{originalFileName}_medium.jpg",
                $"{originalFileName}_large.jpg"
            };

            // Створення ескізів і збереження їх у відповідні файли
            image.Clone(x => x.Resize(100, 100)).Save(Path.Combine(_thumbnailsDirectory, thumbnails[0]));  // Малий
            image.Clone(x => x.Resize(300, 300)).Save(Path.Combine(_thumbnailsDirectory, thumbnails[1]));  // Середній
            image.Clone(x => x.Resize(600, 600)).Save(Path.Combine(_thumbnailsDirectory, thumbnails[2]));  // Великий

            return thumbnails;
        }
    }
}
