using System.Data.Common;
using API.Models;
using API.Models.Data;
using API.Models.Request;
using API.Models.Utils;
using API.Service.Data;

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
        DownloadMessage message = new(download.Link, 0, []);

        DownloadRepository.Add(download);
        await DownloadRepository.SaveChangesAsync();
        
        await MessageService.publishMessage(message, DownloadOrderType.Start.Exchange);
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
        DownloadMessageStop message = new(download.Id);
        await MessageService.publishMessage(message, DownloadOrderType.Stop.Exchange);
    }
}