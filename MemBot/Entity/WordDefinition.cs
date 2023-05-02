using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemBot.Entity;

public class WordDefinition
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
    public int Id { get; set; }
    public string Definition { get; set; }
    public string? PartOfSpeech { get; set; }
    public string[]? Synonyms { get; set; }
    public string[]? Antonyms { get; set; }
    public string[]? TypeOf { get; set; }
    public string[]? PartOf { get; set; }
    public string[]? Examples { get; set; }
}