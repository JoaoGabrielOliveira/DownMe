using API.Models.Data;
using API.Models.Utils;

namespace API.Models.Request;

[Serializable]
public record DownloadMessage(Download Download, DownloadOrderType Action)
{
    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}