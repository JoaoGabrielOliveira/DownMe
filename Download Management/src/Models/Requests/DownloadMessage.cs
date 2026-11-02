namespace API.Models.Request;

[Serializable]
public record DownloadMessage(string Link, int Priority, List<string> Files)
{
    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}

[Serializable]
public record DownloadMessageStop(ulong Id) : DownloadMessage("", 0, [])
{
    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}