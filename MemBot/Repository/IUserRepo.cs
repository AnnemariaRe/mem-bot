using MemBot.Entity;
using Telegram.Bot.Types;
using User = MemBot.Entity.User;

namespace MemBot.Repository;

public interface IUserRepo
{
    public Task<User> CreateUser(Message message);
    public Task<User?> GetUser(long id);
    public Task AddWord(long id, Word? word);

}