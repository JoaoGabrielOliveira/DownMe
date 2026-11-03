using System.Data.Common;
using API.Models;
using API.Models.Data;
using API.Models.Request;
using API.Models.Utils;
using API.Service.Data;
using RabbitMQ.AMQP.Client;

public interface IDownloadService
{
    Task<DownloadStatus?> CheckStatusDownloadQueue(ulong id);
    Task AddDownloadQueue(Download download);
    Task StopDownloadQueue(Download download);
}

public class DownloadService : IDownloadService
{
    private const string EXCHANGE_NAME = "download_orders";

    readonly IMessageService<DownloadMessage> MessageService;
    readonly DownloadRepository DownloadRepository;

    public DownloadService(IMessageService<DownloadMessage> messageService, DownloadRepository downloadRepository)
    {
        MessageService = messageService;
        DownloadRepository = downloadRepository;

        MessageService.ExchangeName = EXCHANGE_NAME;
    }

    public async Task AddDownloadQueue(Download download)
    {   
        await using var transaction = await DownloadRepository.Database.BeginTransactionAsync();
        try
        {
            DownloadRepository.Add(download);
            await DownloadRepository.SaveChangesAsync();
   
            await MessageService.publishMessage(new DownloadMessage(download, DownloadOrderType.Add), DownloadOrderType.Add.Key);
        }
        catch (PublisherException e)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error to send queue", e);
        }

        await transaction.CommitAsync();
    }

    public async Task<DownloadStatus?> CheckStatusDownloadQueue(ulong id)
    {
        Download? download = await DownloadRepository.GetDownloadById(id);
        if (download == null)
        {
            return null;
        }
        return download.Status;
    }

    public async Task StopDownloadQueue(Download download)
    {
        await MessageService.publishMessage(new DownloadMessage(download, DownloadOrderType.Stop), DownloadOrderType.Stop.Key);
    }
}