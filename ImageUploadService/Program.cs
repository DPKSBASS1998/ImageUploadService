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

        // ������������ ��������� ���� �����
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ImageUploadDB")));

        // ������ ������������ ��� ThumbnailService
        builder.Services.Configure<ThumbnailServiceOptions>(builder.Configuration.GetSection("ThumbnailService"));

        // ������ ����� ��� ��������� �����
        builder.Services.AddSingleton<ThumbnailService>();

        // ������ ���������� ��� ������ � API
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // ��������� ���� ��� ���������������
        app.Urls.Add("http://0.0.0.0:5000");

        // ������������ ��������� �����
        app.UseStaticFiles();

        // ������������ �������������
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Image}/{action=Index}/{id?}");

        app.Run();
    }
}//����