using Newtonsoft.Json;

namespace MemBot.Entity;

public class ResponsePronunciation
{
    [JsonProperty("all")]
    public string All { get; set; }
}