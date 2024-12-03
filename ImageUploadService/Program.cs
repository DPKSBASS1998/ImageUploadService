using Microsoft.EntityFrameworkCore;
using ImageUploadService.Models;
using ImageUploadService.Services;

var builder = WebApplication.CreateBuilder(args);

// Зміна на PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ImageUploadDB")));

// Додаємо сервіс для генерації ескізів
builder.Services.AddSingleton(provider =>
{
    var config = builder.Configuration.GetSection("ThumbnailService");
    string thumbnailsDirectory = config["ThumbnailsDirectory"];
    return new ThumbnailService(thumbnailsDirectory);
});

// Додаємо контролери для роботи з API
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Налаштовуємо порт для прослуховування
app.Urls.Add("http://0.0.0.0:5000"); // Змінили порт на 5000

// Налаштовуємо статичні файли
app.UseStaticFiles(); // для доступу до файлів в wwwroot

// Налаштування маршрутизації
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Image}/{action=Index}/{id?}");

app.Run();
