using MemBot.Constant;
using MemBot.Entity;
using MemBot.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MemBot.Command;

public class AddWordCommand : ICommand
{
    public AddWordCommand(IUserRepo userRepo, IWordsApiRepo wordsApiRepo)
    {
        _userRepo = userRepo;
        _wordsApiRepo = wordsApiRepo;
        lastMessageId = 0;
        _responseWord = new ResponseWord();
    }

    public string Key => Commands.AddWordCommand;
    private readonly IUserRepo _userRepo;
    private readonly IWordsApiRepo _wordsApiRepo;
    private ResponseWord? _responseWord;
    private long lastMessageId;
    
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        var text = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Data : update.Message!.Text;
        var userData = await _userRepo.GetUser(message.Chat.Id)!;

        var sentMessage = new Message();
        if (userData != null)
        {
            switch (userData.ConversationStage)
            {
                case 0:
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write the word name",
                        replyMarkup: new ReplyKeyboardRemove()
                    );

                    await _userRepo.IncrementStage(message.Chat.Id);
                    break;
                case 1:
                    var wordInfo = _wordsApiRepo.GetWordInfo(message.Text!).Result;

                    if (wordInfo is { Results.Count: 1 })
                    {
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: WordInfoMessageOneDefinition(wordInfo, wordInfo.Results[0]),
                            replyMarkup: InlineKeyboards.SaveAndTryAgainInlineKeyboard,
                            parseMode: ParseMode.Html
                        );
                    }
                    else if (wordInfo?.Results is { Count: > 1 })
                    {
                        sentMessage = await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: WordInfoMessageManyDefinitions(wordInfo),
                            replyMarkup: WordDefinitionsInlineKeyboard(wordInfo.Results.Count),
                            parseMode: ParseMode.Html
                        );
                        await _userRepo.AddLastMessageId(message.Chat.Id, sentMessage.MessageId);
                    }
                    else if (wordInfo == null)
                    {
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "❌Couldn't understand that word, please try again"
                        );
                        await _userRepo.DecrementStage(message.Chat.Id);
                    }

                    _responseWord = wordInfo;
                    lastMessageId = message.MessageId;
                    await _userRepo.IncrementStage(message.Chat.Id);
                    break;

                case 2:
                    if (int.TryParse(text, out var i))
                    {
                        var messageId = _userRepo.GetLastMessageId(message.Chat.Id);
                        
                        sentMessage = await client.EditMessageTextAsync(
                            chatId: message.Chat.Id,
                            messageId: messageId,
                            text: WordInfoMessageOneDefinition(_responseWord, _responseWord.Results[i]),
                            replyMarkup: InlineKeyboards.BackInlineKeyboard,
                            parseMode: ParseMode.Html
                        );
                        await _userRepo.AddLastMessageId(message.Chat.Id, sentMessage.MessageId);
                        await _userRepo.IncrementStage(message.Chat.Id);
                    }
                    
                    break;
                case 3:
                    if (text == CommandTypes.Back)
                    {
                        var messageId = _userRepo.GetLastMessageId(message.Chat.Id);
                        
                        sentMessage = await client.EditMessageTextAsync(
                            chatId: message.Chat.Id,
                            messageId: messageId,
                            text: WordInfoMessageManyDefinitions(_responseWord),
                            replyMarkup: WordDefinitionsInlineKeyboard(_responseWord.Results.Count),
                            parseMode: ParseMode.Html
                        );
                        
                        await _userRepo.AddLastMessageId(message.Chat.Id, sentMessage.MessageId);
                        await _userRepo.DecrementStage(message.Chat.Id);
                    }

                    break;
            }
        }
    }

    private string WordInfoMessageOneDefinition(ResponseWord wordInfo, ResponseResult wordResult)
    {
        var message = $"✅<b>{wordInfo.Word}</b> /{wordInfo.Pronunciation.All}/\n\n" +
                      $"Definition: <i>{wordResult.Definition}</i>\n" +
                      $"Part of speech: <i>{wordResult.PartOfSpeech}</i>\n\n";

        if (wordResult.Examples != null)
        {
            message += $"Example with the word <b>{wordInfo.Word}</b>:\n" +
                       $"<i>{wordResult.Examples[0]}</i>";
        }
        return message;
    }
    
    private string WordInfoMessageManyDefinitions(ResponseWord wordInfo)
    {
         var message = $"✅There are some definitions of the word <b>{wordInfo.Word}</b>: \n\n";
         var i = 1;
         foreach (var result in wordInfo.Results!)
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
            var button = InlineKeyboardButton.WithCallbackData($"{i}", $"{i - 1}");
            row.Add(button);
            if (row.Count == 3 || i == definitionCount)
            {
                buttons.Add(row);
                row = new List<InlineKeyboardButton>();
            }
        }
        return new InlineKeyboardMarkup(buttons);
    }

}