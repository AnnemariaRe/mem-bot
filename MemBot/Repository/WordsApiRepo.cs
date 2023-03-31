using System.Net.Http.Headers;
using MemBot.Entity;
using Newtonsoft.Json;

namespace MemBot.Repository;

public class WordsApiRepo
{
    public async Task<ResponseWord?> GetWordInfo(string word)
    {
        var url = "https://wordsapiv1.p.rapidapi.com/words/";
        var parameters = $"{word}";
        
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "wordsapiv1.p.rapidapi.com"); 
        client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "5ba3b0e389msh91ef861df12defap14dfedjsndb0d071492da");
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync(parameters).ConfigureAwait(false);

        var wordResult = new ResponseWord();
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            wordResult = JsonConvert.DeserializeObject<ResponseWord>(jsonString);
        }

        return wordResult;
    }
}