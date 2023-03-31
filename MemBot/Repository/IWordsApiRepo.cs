using MemBot.Entity;

namespace MemBot.Repository;

public interface IWordsApiRepo
{
    public Task<ResponseWord?> GetWordInfo(string word);
}