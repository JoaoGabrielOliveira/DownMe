using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using API.Models.Data;
using API.Models.Request;
using API.Service.Data;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;

namespace API.Service.Worker;

public class MessageConsumerWorker : BackgroundService
{
    private readonly IRabbitMQConnection _rabbitMQConnection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MessageConsumerWorker> _logger;
    private const string QueueName = "download_orders_update";

    public MessageConsumerWorker(IRabbitMQConnection rabbitMQConnection, IServiceScopeFactory scopeFactory, ILogger<MessageConsumerWorker> logger)
    {
        _rabbitMQConnection = rabbitMQConnection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {                
        IConnection connection = await _rabbitMQConnection.GetConnection();

        var consumer = await connection
        .ConsumerBuilder()
        .Queue(QueueName)
        .InitialCredits(1)
        .MessageHandler(MessageHandler)
        .BuildAndStartAsync();

         while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"Worker actived at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task MessageHandler(IContext context, IMessage message)
    {
        try
        {
            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(new JsonStringEnumConverter<DownloadStatus>()); 

            //TODO Messages with syntax error are being requeue and lock in infinite loop
            DownloadMessage? downloadMessage = JsonSerializer.Deserialize<DownloadMessage>(message.BodyAsString(), options);

            if (downloadMessage == null)
            {
                _logger.LogError("Message could not be deserialized");
                context.Requeue();

                return;
            }

            using (var scope = _scopeFactory.CreateScope())
            {   
                DownloadRepository _downloadRepository = scope.ServiceProvider.GetRequiredService<DownloadRepository>();

                _downloadRepository.Update(downloadMessage.Download);
                await _downloadRepository.SaveChangesAsync();
            }

            context.Accept();
        }
        catch
        {
            context.Requeue();
        }
    }

}