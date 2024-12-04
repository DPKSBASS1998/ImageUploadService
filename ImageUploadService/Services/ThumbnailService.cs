using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Connections;

namespace ImageUploadService.Services
{
    public class ThumbnailService
    {
        private readonly string _thumbnailsDirectory;
        private readonly string _rabbitMqHostName;
        private readonly string _rabbitMqUserName;
        private readonly string _rabbitMqPassword;

        public ThumbnailService(IConfiguration configuration)
        {
            _thumbnailsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "thumbnails");
            _rabbitMqHostName = configuration["RabbitMQ:HostName"];
            _rabbitMqUserName = configuration["RabbitMQ:UserName"];
            _rabbitMqPassword = configuration["RabbitMQ:Password"];

            if (!Directory.Exists(_thumbnailsDirectory))
            {
                Directory.CreateDirectory(_thumbnailsDirectory);
            }
        }

        // Метод для відправки повідомлення до черги RabbitMQ
        public void SendMessageToQueue(string message)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitMqHostName,
                    UserName = _rabbitMqUserName,
                    Password = _rabbitMqPassword
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "thumbnail_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "", routingKey: "thumbnail_queue", basicProperties: null, body: body);
                    Console.WriteLine("Message sent successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to queue: {ex.Message}");
            }
        }

        // Метод для обробки отриманих повідомлень з черги RabbitMQ
        public void ProcessImageFromQueue()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqHostName,
                UserName = _rabbitMqUserName,
                Password = _rabbitMqPassword
            };

            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "thumbnail_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += async (model, ea) =>
                    {
                        try
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            var imageRequest = JsonConvert.DeserializeObject<ImageRequest>(message);

                            // Генерація ескізів
                            var thumbnails = await GenerateThumbnailsAsync(imageRequest.ImagePath, imageRequest.FileName);

                            // Логування результату
                            Console.WriteLine("Thumbnails generated: " + string.Join(", ", thumbnails));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing message: {ex.Message}");
                        }
                    };

                    channel.BasicConsume(queue: "thumbnail_queue", autoAck: true, consumer: consumer);
                    Console.WriteLine("Waiting for messages...");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to RabbitMQ: {ex.Message}");
            }
        }

        // Генерація ескізів
        public async Task<List<string>> GenerateThumbnailsAsync(string originalImagePath, string originalFileName)
        {
            try
            {
                var image = await Task.Run(() => Image.Load(originalImagePath));

                var thumbnails = new List<string>
                {
                    $"{originalFileName}_small.jpg",
                    $"{originalFileName}_medium.jpg",
                    $"{originalFileName}_large.jpg"
                };

                image.Clone(x => x.Resize(100, 100)).Save(Path.Combine(_thumbnailsDirectory, thumbnails[0]));
                image.Clone(x => x.Resize(300, 300)).Save(Path.Combine(_thumbnailsDirectory, thumbnails[1]));
                image.Clone(x => x.Resize(600, 600)).Save(Path.Combine(_thumbnailsDirectory, thumbnails[2]));

                return thumbnails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating thumbnails for {originalFileName}: {ex.Message}");
                return new List<string>();
            }
        }
    }

    public class ImageRequest
    {
        public string ImagePath { get; set; }
        public string FileName { get; set; }
    }
}