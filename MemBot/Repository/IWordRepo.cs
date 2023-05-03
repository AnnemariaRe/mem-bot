using MemBot.Entity;
using Telegram.Bot.Types;

namespace MemBot.Repository;

public interface IWordRepo
{
    Task<Word?> AddWord(long id, ResponseWord word);
    public Word? GetWord(int id);
    public List<Word> GetUserWords(long id);
    public IEnumerable<WordDefinition>? GetWordDefinitions(int id);
}