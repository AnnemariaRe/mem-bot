using System.ComponentModel.DataAnnotations;

namespace MemBot.Entity
{
    public class Word
    {
        [Key] public long Id { get; set; }
        public string Name { get; set; }
        public string? Pronuncation { get; set; }
        public string[]? Definitions { get; set; }
        public string[]? PartsOfSpeech { get; set; }
        public string[]? Synonyms { get; set; }
        public string[]? Antonyms { get; set; }
        public string[]? Phrases { get; set; }
        public string[]? Examples { get; set; }
    }
}