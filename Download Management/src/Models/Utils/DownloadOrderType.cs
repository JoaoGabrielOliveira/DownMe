using System.Text.Json.Serialization;

namespace API.Models.Utils;

public sealed class DownloadOrderType
{
    public static readonly DownloadOrderType Add = new("add", "add");
    public static readonly DownloadOrderType Start = new("start", "updade");
    public static readonly DownloadOrderType Cancel = new("cancel", "update");
    public static readonly DownloadOrderType Stop = new("stop", "update");

    public string Name {get; private set; }
    public string Key {get; private set; }

    [JsonConstructor]
    private DownloadOrderType(string name, string key)
    {
        Name = name;
        Key = key;
    }


}