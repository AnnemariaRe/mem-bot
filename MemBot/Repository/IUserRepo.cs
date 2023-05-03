using MemBot.Entity;
using Telegram.Bot.Types;
using User = MemBot.Entity.User;

namespace MemBot.Repository;

public interface IUserRepo
{
    public Task<User> CreateUser(Message message);
    public Task<User?> GetUser(long id);
    public Task AddWord(long id, Word? word);
    public Task IncrementStage(long chatId);
    public Task DecrementStage(long chatId);
    public Task ResetStage(long id);
    public Task AddLastMessageId(long chatId, int id);
    public int GetLastMessageId(long chatId);
    public Word? GetUserWord(long chatId, string wordName);
    public IEnumerable<Word>? GetUserWords(long chatId);

}