using MemBot.Context;
using MemBot.Entity;
using Telegram.Bot.Types;

namespace MemBot.Repository;

public class WordRepo : IWordRepo
{
    private readonly ApplicationDbContext _context;
    private readonly IUserRepo _userRepo;

    public WordRepo(ApplicationDbContext context, IUserRepo userRepo)
    {
        _context = context;
        _userRepo = userRepo;
    }

    public async Task<Word?> AddWord(long id, ResponseWord responseWord)
    {
        var definitions = (responseWord.Results ?? Enumerable.Empty<ResponseResult>()).Select(responseResult =>
                new WordDefinition
            {
                Definition = responseResult.Definition,
                PartOfSpeech = responseResult.PartOfSpeech,
                Synonyms = responseResult.Synonyms?.ToArray(),
                Antonyms = responseResult.Anonyms?.ToArray(),
                TypeOf = responseResult.TypeOf?.ToArray(),
                PartOf = responseResult.PartOf?.ToArray(),
                Examples = responseResult.Examples?.ToArray(),
            })
            .ToList();

        var newWord = new Word()
        {
            Name = responseWord.Word,
            Pronunciation = responseWord.Pronunciation.All,
            Definitions = definitions,
            User = await _userRepo.GetUser(id)
        };
        
        await _userRepo.AddWord(id, newWord);
        // _context.Words.Add(newWord);
        // await _context.SaveChangesAsync();
        
        return newWord;
    }
}