using ImageUploadService.Models;
using ImageUploadService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ImageUploadService.ViewModels;

namespace ImageUploadService.Controllers
{
    public class ImageController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;
        private readonly ThumbnailService _thumbnailService;

        public ImageController(ApplicationDbContext context, IWebHostEnvironment environment, ThumbnailService thumbnailService)
        {
            _context = context;
            _environment = environment;
            _thumbnailService = thumbnailService;
        }

        // Метод для завантаження зображення
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Отримуємо шлях до зображення, де воно буде збережене
                var filePath = Path.Combine(_environment.WebRootPath, "images", file.FileName);

                // Зберігаємо оригінальне зображення
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Генерація ескізів
                var thumbnails = await _thumbnailService.GenerateThumbnailsAsync(filePath, file.FileName);

                // Створюємо об'єкт Image для збереження в базі даних
                var image = new Image
                {
                    Path = Path.Combine("images", file.FileName),
                    ThumbnailsJson = JsonConvert.SerializeObject(thumbnails)
                };

                // Додаємо зображення до бази даних
                _context.Images.Add(image);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // Метод для відображення всіх зображень на головній сторінці
        public async Task<IActionResult> Index()
        {
            // Отримуємо список всіх зображень з бази даних
            var images = await _context.Images.ToListAsync();

            // Формуємо модель для відображення зображень і їхніх ескізів
            var model = images.Select(image => new ImageViewModel
            {
                // Виправлений шлях до оригінального зображення
                Path = Url.Content(image.Path.Replace("images\\", "")),  
                Thumbnails = JsonConvert.DeserializeObject<List<string>>(image.ThumbnailsJson)
                    .Select(thumbnail => Url.Content("~/thumbnails/" + thumbnail))  // Шляхи до ескізів
                    .ToList()
            }).ToList();

            return View(model);  // Повертаємо модель для відображення в представлення
        }

    }
}
