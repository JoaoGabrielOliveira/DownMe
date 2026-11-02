namespace API.Models.Request;

public record DownloadRequest (string Name, string MagnetLink, int Priority);