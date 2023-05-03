using MemBot.Constant;
using MemBot.Context;
using MemBot.Entity;
using MemBot.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MemBot.Command;

public class ShowUserWordsCommand : ICommand
{
    public ShowUserWordsCommand(IUserRepo userRepo, IWordRepo wordRepo)
    {
        _userRepo = userRepo;
        _wordRepo = wordRepo;
    }

    public string Key => Commands.ShowWordsCommand;
    private readonly IUserRepo _userRepo;
    private readonly IWordRepo _wordRepo;
    private int? wordId = 0;
    private int? defIndex = 0;
    
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        var text = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Data : update.Message!.Text;
        var userData = await _userRepo.GetUser(message.Chat.Id);
        var splitText = text.Split(' ');

        if (userData != null)
        {
            var word = new Word();
            Message? sentMessage;
            sentMessage = new Message();
            if (text == Commands.ShowWordsCommand)
            {
                sentMessage = await client.SendTextMessageAsync(message.Chat.Id, "loading...");
                await ShowUserWords(client, message.Chat.Id, sentMessage.MessageId);
            } else switch (splitText[0])
            {
                case ">>":
                    var offset1 = int.Parse(splitText[1]);
                    await ShowUserWords(client, message.Chat.Id, _userRepo.GetLastMessageId(message.Chat.Id), offset1);
                    break;
                case "<<":
                    var offset2 = int.Parse(splitText[1]);
                    await ShowUserWords(client, message.Chat.Id, _userRepo.GetLastMessageId(message.Chat.Id), offset2);
                    break;
                case "word":
                    wordId = int.Parse(splitText[1]);
                    word = _wordRepo.GetWord(wordId.Value);
                    sentMessage = await client.EditMessageTextAsync(
                        chatId: message.Chat.Id,
                        messageId: _userRepo.GetLastMessageId(message.Chat.Id),
                        text: WordInfoMessageManyDefinitions(word),
                        replyMarkup: WordDefinitionsInlineKeyboard(word.Definitions.Count),
                        parseMode: ParseMode.Html
                    );
                    await _userRepo.AddLastMessageId(message.Chat.Id, sentMessage.MessageId);
                    break;
                case "def":
                    defIndex = int.Parse(splitText[1]);
                    word = _wordRepo.GetWord(wordId.Value);
                    await ShowWordDefinition(client, message.Chat.Id, word, defIndex.Value,
                        _userRepo.GetLastMessageId(message.Chat.Id));

                    break;
            } 

        }
        
    }

    private async Task ShowUserWords(ITelegramBotClient client, long id, int messageId, int offset = 0)
    {
        const int pageSize = 5;
        var userWords = _userRepo.GetUserWords(id)?.OrderByDescending(w => w.Name).ToList();
        
        if (userWords != null && userWords.Any())
        {
            var buttons = new List<InlineKeyboardButton[]>();

            for (var i = offset; i < Math.Min(offset + pageSize, userWords.Count); i++)
            {
                var word = userWords[i];
                var wordButtons = new List<InlineKeyboardButton>();
                wordButtons.Add(InlineKeyboardButton.WithCallbackData(word.Name, $"word {word.Id}"));
                buttons.Add(wordButtons.ToArray());
            }

            if (offset > 0 && offset + pageSize < userWords.Count)
            {
                buttons.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData("<<", $"<< {offset - pageSize}"),
                    InlineKeyboardButton.WithCallbackData(">>", $">> {offset + pageSize}")
                });
            }
            else
            {
                if (offset > 0)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<<", $"<< {offset - pageSize}") });
                }
                if (offset + pageSize < userWords.Count)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData(">>", $">> {offset + pageSize}") });
                }
            }

            var keyboardMarkup = new InlineKeyboardMarkup(buttons.ToArray());
            var sentMessage = await client.EditMessageTextAsync(id, messageId,  "Your words:", replyMarkup: keyboardMarkup);
            await _userRepo.AddLastMessageId(id, sentMessage.MessageId);
        }
        else await client.EditMessageTextAsync(id, _userRepo.GetLastMessageId(id),"0️⃣ You have no words added yet.");
    }
    
    private string WordInfoMessageManyDefinitions(Word word)
    {
        var message = $"✅There are some definitions of the word <b>{word.Name}</b>: \n\n";
        var i = 1;
        foreach (var result in _wordRepo.GetWordDefinitions(word.Id)!)
        {
            message += $"{i}. {result.Definition}\n";
            i++;
        }
        message += "\nClick for more information.";

        return message;
    }

    private InlineKeyboardMarkup WordDefinitionsInlineKeyboard(int definitionCount)
    {
        var buttons = new List<List<InlineKeyboardButton>>();
        var row = new List<InlineKeyboardButton>();
        for (var i = 1; i <= definitionCount; i++)
        {
            var button = InlineKeyboardButton.WithCallbackData($"{i}", $"def {i - 1}");
            row.Add(button);
            if (row.Count == 3 || i == definitionCount)
            {
                buttons.Add(row);
                row = new List<InlineKeyboardButton>();
            }
            if (i == definitionCount)
            {
                row = new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData($"<<", "<< 0") };
                buttons.Add(row);
            }
        }
        return new InlineKeyboardMarkup(buttons);
    }
    
    private async Task ShowWordDefinition(ITelegramBotClient client, long chatId, Word word, int definitionIndex, int messageId)
    {
        var definition = _wordRepo.GetWordDefinitions(word.Id)!.ToList()[definitionIndex];
        var buttons = new List<InlineKeyboardButton[]>();

        var message = $"Definition: <i>{definition.Definition}</i>\n" +
                      $"Part of speech: <i>{definition.PartOfSpeech}</i>\n\n";

        if (definition.Synonyms?.Length > 0)
        {
            message += "Synonyms: \n";
            message = definition.Synonyms.Aggregate(message, (current, synonym) => current + $"- <i>{synonym}</i>\n");
            message += "\n";
        }
        if (definition.Antonyms?.Length > 0)
        {
            message += "Antonyms: \n";
            message = definition.Antonyms.Aggregate(message, (current, synonym) => current + $"- <i>{synonym}</i>\n");
            message += "\n";
        }
        if (definition.TypeOf?.Length > 0)
        {
            message += "Type of: \n";
            message = definition.TypeOf.Aggregate(message, (current, synonym) => current + $"- <i>{synonym}</i>\n");
            message += "\n";
        }
        if (definition.PartOf?.Length > 0)
        {
            message += "Part of: \n";
            message = definition.PartOf.Aggregate(message, (current, synonym) => current + $"- <i>{synonym}</i>\n");
            message += "\n";
        }
        if (definition.Examples?.Length > 0)
        {
            message += "Examples: \n";
            message = definition.Examples.Aggregate(message, (current, synonym) => current + $"- <i>{synonym}</i>\n");
            message += "\n";
        }

        if (word.Definitions.Count > definitionIndex + 1)
        {
            buttons.Add(new[]
            { InlineKeyboardButton.WithCallbackData("Next Definition >>", $"def {definitionIndex + 1}") });
        }

        buttons.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData("Back", $"word {word.Id}")
        });
        var inlineKeyboard = new InlineKeyboardMarkup(buttons.ToArray());

        var sentMessage = await client.EditMessageTextAsync(chatId, messageId, message, replyMarkup: inlineKeyboard, parseMode: ParseMode.Html);
        await _userRepo.AddLastMessageId(chatId, sentMessage.MessageId);
    }
}