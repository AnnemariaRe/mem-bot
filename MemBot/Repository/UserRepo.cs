using MemBot.Context;
using MemBot.Entity;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using User = MemBot.Entity.User;

namespace MemBot.Repository;

public class UserRepo : IUserRepo
{
    private readonly ApplicationDbContext _context;

    public UserRepo(ApplicationDbContext context) 
    {
        _context = context;
    }
    
    public async Task<User> CreateUser(Message message)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == message.Chat.Id);
        if (user != null) return user;
        
        var newUser = new User
        {
            Id = message.Chat.Id, 
            Username = message.Chat.Username,
            Words = new List<Word>()
        };

        var result = await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<User?> GetUser(long id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user;
    }

    public async Task AddWord(long id, Word? word)
    {
        var user = GetUser(id).Result;
        if (user is not null && word is not null)
        {
            user.Words?.Add(word);
            await _context.SaveChangesAsync();
        }
    }
}