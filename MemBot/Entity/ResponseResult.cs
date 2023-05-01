using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace MemBot.Entity;

public class ResponseResult
{
    [JsonProperty("definition")]
    public string Definition { get; set; }
    [JsonProperty("partOfSpeech")]
    public string PartOfSpeech { get; set; }
    [JsonProperty("synonyms")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Synonyms { get; set; }
    [JsonProperty("typeOf")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? TypeOf { get; set; }
    [JsonProperty("partOf")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? PartOf { get; set; }
    [JsonProperty("anonyms")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Anonyms { get; set; }
    [JsonProperty("examples")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Examples { get; set; }
    [JsonProperty("also")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Also { get; set; }
}