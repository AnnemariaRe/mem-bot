using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace MemBot.Entity;

public class ResponseWord
{
    [JsonProperty("word")]
    public string Word { get; set; }
    [JsonProperty("results")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ResponseResult>? Results { get; set; }
    [JsonProperty("pronunciation")]
    public ResponsePronunciation Pronunciation { get; set; }
}