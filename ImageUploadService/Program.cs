using ImageUploadService.Models;
using ImageUploadService.Services;
using Microsoft.EntityFrameworkCore;

public class ThumbnailServiceOptions
{
    public string ThumbnailsDirectory { get; set; }
    public string RabbitMqHostName { get; set; }
    public string RabbitMqUserName { get; set; }
    public string RabbitMqPassword { get; set; }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Налаштування контексту бази даних
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ImageUploadDB")));

        // Додаємо конфігурацію для ThumbnailService
        builder.Services.Configure<ThumbnailServiceOptions>(builder.Configuration.GetSection("ThumbnailService"));

        // Додаємо сервіс для генерації ескізів
        builder.Services.AddSingleton<ThumbnailService>();

        // Додаємо контролери для роботи з API
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Налаштуємо порт для прослуховування
        app.Urls.Add("http://0.0.0.0:5000");

        // Налаштування статичних файлів
        app.UseStaticFiles();

        // Налаштування маршрутизації
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Image}/{action=Index}/{id?}");

        app.Run();
    }
}//тест