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
            Words = new List<Word>(),
            ConversationStage = 0
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
            user.Words ??= new List<Word>();
            user.Words?.Add(word);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task IncrementStage(long chatId)
    {
        var user = GetUser(chatId).Result;
        if (user is not null)
        {
            user.ConversationStage++;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task DecrementStage(long chatId)
    {
        var user = GetUser(chatId).Result;
        if (user is not null && user.ConversationStage != 0)
        {
            user.ConversationStage--;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task ResetStage(long id)
    {
        var user = GetUser(id).Result;
        if (user is not null)
        {
            user.ConversationStage = 0;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task AddLastMessageId(long chatId, int id)
    {
        var user = GetUser(chatId).Result;
        if (user is not null)
        {
            user.LastMessageId = id;
            await _context.SaveChangesAsync();
        }
    }
    
    public int GetLastMessageId(long chatId)
    {
        var user = GetUser(chatId).Result;
        return user?.LastMessageId ?? 0;
    }
    
    public Word? GetUserWord(long chatId, string wordName)
    {
        var user = GetUser(chatId).Result;
        if (user.Words != null && user.Words.Any())
        {
            return user.Words.FirstOrDefault(x => x.Name == wordName);
        }
        return null;
    }
    
    public IEnumerable<Word>? GetUserWords(long chatId)
    {
        var user = GetUser(chatId).Result;
        return user != null ? _context.Words.ToList().Where(word => word.User.Equals(user)).ToList() : null;
    }
}