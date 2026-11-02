using System.ComponentModel.DataAnnotations.Schema;
using API.Models.Request;

namespace API.Models.Data;


[Table("downloads_history")]
public sealed class Download
{
    [Column("id")]
    public ulong? Id {get; set;}
    public required string Name {get; set; }

    public required string Link { get; set; }

    [Column("status")]
    public DownloadStatus Status {get; set; }

    [Column("started_at")]
    public DateTime StartedAt {get; set; }

    [Column("last_time_accessed")]
    public DateTime LastTimeAccessed {get; set; }

    [Column("finished_at")]
    public DateTime FinishedAt {get; set; }
}

public enum DownloadStatus
{
    WAITING = 0,
    STARTED = 1,
    STOPED = 2,
    FINISHED = 3
}