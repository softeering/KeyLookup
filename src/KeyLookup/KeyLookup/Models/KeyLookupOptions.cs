namespace KeyLookup.Models;

public class KeyLookupOptions
{
    public string RootFolder { get; set; }
    public bool EnablePreload { get; set; }
    public TimeSpan NodeHeartBeatInterval { get; set; } = TimeSpan.FromSeconds(30);
}
