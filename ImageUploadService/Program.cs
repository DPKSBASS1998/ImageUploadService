using Microsoft.EntityFrameworkCore;
using ImageUploadService.Models;
using ImageUploadService.Services;

var builder = WebApplication.CreateBuilder(args);

// ���� �� PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ImageUploadDB")));

// ������ ����� ��� ��������� �����
builder.Services.AddSingleton(provider =>
{
    var config = builder.Configuration.GetSection("ThumbnailService");
    string thumbnailsDirectory = config["ThumbnailsDirectory"];
    return new ThumbnailService(thumbnailsDirectory);
});

// ������ ���������� ��� ������ � API
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ����������� ���� ��� ���������������
app.Urls.Add("http://0.0.0.0:5000"); // ������ ���� �� 5000

// ����������� ������� �����
app.UseStaticFiles(); // ��� ������� �� ����� � wwwroot

// ������������ �������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Image}/{action=Index}/{id?}");

app.Run();
