using MemBot.Constant;
using MemBot.Entity;
using MemBot.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MemBot.Command;

public class GuessTheWordCommand: ICommand
{
    public GuessTheWordCommand(IUserRepo userRepo, IWordRepo wordRepo)
    {
        _userRepo = userRepo;
        _wordRepo = wordRepo;
    }

    public string Key => Commands.GuessTheWordCommand;
    private readonly IUserRepo _userRepo;
    private readonly IWordRepo _wordRepo;
    
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        var text = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Data : update.Message!.Text;
        var userData = await _userRepo.GetUser(message.Chat.Id);

        var sentMessage = new Message();
        if (userData != null)
        {
            if (text == Key)
            {
                sentMessage = await client.SendTextMessageAsync(message.Chat.Id, "loading...");
                await SendGuessMessage(client, message.Chat.Id, sentMessage.MessageId);
            } else
            {
                var splitText = text.Split(' ');
                sentMessage = splitText[0] switch
                {
                    "true" => await client.EditMessageTextAsync(message.Chat.Id,
                        _userRepo.GetLastMessageId(message.Chat.Id), "✅ You are correct!"),
                    "false" => await client.EditMessageTextAsync(message.Chat.Id,
                        _userRepo.GetLastMessageId(message.Chat.Id), $"❌ Correct answer was <b>{splitText[1]}</b>", parseMode: ParseMode.Html),
                    _ => sentMessage
                };
            }
        }
    }

    private async Task SendGuessMessage(ITelegramBotClient client, long chatId, int messageId)
    {
        var words = _wordRepo.GetUserWords(chatId);
        if (words.Count < 3)
        {
            await client.SendTextMessageAsync(chatId, "⚠ You don't have enough words to play");
            return;
        }
        
        var random = new Random();
        words = Shuffle(words);
        var wordsForGuess = new List<Word>();
        
        for (var i = 0; i < 3; i++)
        {
            wordsForGuess.Add(words[i]);
            words.Remove(words[i]);
        }

        var buttons = new List<InlineKeyboardButton>();
        var buttons2 = new List<List<InlineKeyboardButton>>();
        var correctAnswerIndex = random.Next(3);
        var message = "What word does this definition refer to?\n\n";
        for (var i = 0; i < 3; i++)
        {
            var callbackData = i == correctAnswerIndex ? "true" : "false";
            if (callbackData.Equals("true"))
            {
                var definitions = _wordRepo.GetWordDefinitions(wordsForGuess[i].Id);
                var definition = definitions!.OrderBy(d => Guid.NewGuid()).First();
                message += $"<i>{definition.Definition}</i>";
            }
            var buttonText = $"{wordsForGuess[i].Name}";
            buttons = new List<InlineKeyboardButton>();
            buttons.Add(InlineKeyboardButton.WithCallbackData(buttonText, callbackData + $" {wordsForGuess[correctAnswerIndex].Name}"));
            buttons2.Add(buttons);
        }
        
        var keyboardMarkup = new InlineKeyboardMarkup(buttons2.ToArray());
        var sentMessage = await client.EditMessageTextAsync(chatId, messageId,  message, replyMarkup: keyboardMarkup, parseMode: ParseMode.Html);
        await _userRepo.AddLastMessageId(chatId, sentMessage.MessageId);
    }

    private List<Word> Shuffle(List<Word> list)
    {
        var random = new Random();
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (list[j], list[i]) = (list[i], list[j]);
        }

        return list;
    }

}
