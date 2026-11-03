using System.Text.Json.Serialization;
using API.Models.Data;
using API.Models.Utils;

namespace API.Models.Request;

[Serializable]
public class DownloadMessage
{
    [JsonPropertyName("download")]
    public Download Download { get; set; }

    [JsonPropertyName("action")]
    public DownloadOrderType Action { get; set; }

    [JsonConstructor]
    public DownloadMessage(Download download, DownloadOrderType action)
    {
        Download = download;
        Action = action;
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}