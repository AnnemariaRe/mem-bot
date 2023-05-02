using MemBot.Entity;
using Telegram.Bot.Types;

namespace MemBot.Repository;

public interface IWordRepo
{
    Task<Word?> AddWord(long id, ResponseWord word);
}