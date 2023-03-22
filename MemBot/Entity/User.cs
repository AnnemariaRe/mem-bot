using System.ComponentModel.DataAnnotations;

namespace MemBot.Entity
{
    public class User
    {
        [Key] public long Id { get; set; }
        public string? Username { get; set; }
        public ICollection<Word>? Words { get; set; }
    }
}