using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemBot.Entity
{
    public class Word
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Pronunciation { get; set; }
        public List<WordDefinition> Definitions { get; set; }
        public User? User { get; set; }
    }
}