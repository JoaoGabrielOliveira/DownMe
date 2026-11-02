using System.Data.Common;
using System.Net.Mime;
using API.Models;
using API.Models.Data;
using API.Models.Request;
using API.Service.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
//[Consumes(MediaTypeNames.Application.Json)]
[Route("/downloads")]
public class DownloadsController : ControllerBase
{
    public IDownloadService DownloadService;
    public DownloadRepository DownloadRepository;
    public DownloadsController(IDownloadService downloadService, DownloadRepository downloadRepository)
    {
        DownloadService = downloadService;
        DownloadRepository = downloadRepository;
    }

    [HttpPost]
    public async Task<IResult> AddDownload([FromBody] DownloadRequest donwloadRequest)
    {
        Download download = new Download()
        {
            Name = donwloadRequest.Name,
            Link = donwloadRequest.MagnetLink,
            Status = DownloadStatus.WAITING
        };

        await DownloadService.AddDownloadQueue(download);

        return Results.Created();
    }

    [HttpPost("{id}/stop")]
    public async Task<IResult> StopDownload(ulong id)
    {
        var download = await DownloadRepository.GetDownloadById(id);
        
        if (download == null)
        {
            return Results.NotFound();
        }

        if (download.Status.Equals(DownloadStatus.STOPED))
        {
            return Results.BadRequest($"{id} is already stopped!");
        }

        await DownloadService.StopDownloadQueue(download);

        return Results.Accepted();
    }


    [HttpGet("{id}/status")]
    public async Task<IResult> GetDownloadStatus(ulong id)
    {
        try
        {
            var status = await DownloadService.CheckStatusDownloadQueue(id);

            if (status == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(Enum.GetName(status));
        } catch (InvalidOperationException e)
        {
            return Results.InternalServerError(e);
        } catch(DbException e)
        {
            return Results.InternalServerError(e.Data);
        }
    }
}