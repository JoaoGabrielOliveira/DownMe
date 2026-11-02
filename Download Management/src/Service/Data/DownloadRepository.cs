using API.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Service.Data;

public class DownloadRepository : DbContext
{
    private DbSet<Download> Downloads {get; set; }

    public DownloadRepository(DbContextOptions<DownloadRepository> options)
        : base(options)
    {
        
    }

    public async Task<Download?> GetDownloadById(ulong id)
    {
        Download? download = await Downloads
        .Where(download => download.Id.Equals(id))
        .FirstOrDefaultAsync();

        return download;
    }

    public async Task<Download?> AddDownload(Download download)
    {
        

        return download;
    }

    public async Task<List<Download>> GetDownloads(int limit = 5, int offset = 0)
    {
        return [.. Downloads
        .Take(limit)
        .Skip(offset)];
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Download>()
            //.ToTable("downloads_history")
            .ToTable("downloads_history")
            .Property(e => e.Id)
            .HasConversion<ulong>();


        
    }
}