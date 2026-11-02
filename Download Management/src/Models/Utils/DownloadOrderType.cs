namespace API.Models.Utils;

public sealed class DownloadOrderType
{
    public static readonly DownloadOrderType Start = new("start", "start");
    public static readonly DownloadOrderType Cancel = new("cancel", "cancel");
    public static readonly DownloadOrderType Stop = new("stop", "stop");

    public string Name {get; private set; }
    public string Exchange {get; private set; }

    private DownloadOrderType(string name, string exchange)
    {
        Name = name;
        Exchange = exchange;
    }


}