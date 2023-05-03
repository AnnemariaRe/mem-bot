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
    public AddWordCommand(IUserRepo userRepo, IWordsApiRepo wordsApiRepo, IWordRepo wordRepo)
    {
        _userRepo = userRepo;
        _wordsApiRepo = wordsApiRepo;
        _wordRepo = wordRepo;
        _responseWord = new ResponseWord();
    }

    public string Key => Commands.AddWordCommand;
    private readonly IUserRepo _userRepo;
    private readonly IWordsApiRepo _wordsApiRepo;
    private readonly IWordRepo _wordRepo;
    private ResponseWord? _responseWord;

    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        var text = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Data : update.Message!.Text;
        var userData = await _userRepo.GetUser(message.Chat.Id);

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
                        sentMessage = await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: WordInfoMessageOneDefinition(wordInfo, wordInfo.Results[0]),
                            replyMarkup: InlineKeyboards.SaveAndTryAgainInlineKeyboard,
                            parseMode: ParseMode.Html
                        );
                        await _userRepo.AddLastMessageId(message.Chat.Id, sentMessage.MessageId);
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

                    if (text == CommandTypes.TryAgain)
                    {
                        await _userRepo.DecrementStage(message.Chat.Id);
                        await _userRepo.DecrementStage(message.Chat.Id);
                        var messageId = _userRepo.GetLastMessageId(message.Chat.Id);
                        await client.DeleteMessageAsync(message.Chat.Id, messageId);
                        
                        goto case 0;
                    }

                    if (text == CommandTypes.Save)
                    {
                        if (_userRepo.GetUserWord(message.Chat.Id, _responseWord.Word) != null)
                        {
                            await client.DeleteMessageAsync(message.Chat.Id, _userRepo.GetLastMessageId(message.Chat.Id));
                            await client.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: $"⚠ Word <b>{_responseWord.Word}</b> is already added.",
                                replyMarkup: KeyboardMarkups.MainMenuKeyboardMarkup,
                                parseMode: ParseMode.Html
                            );
                            await _userRepo.ResetStage(message.Chat.Id);
                            break;
                        }
                        
                        await _wordRepo.AddWord(message.Chat.Id, _responseWord!);
                        await client.DeleteMessageAsync(message.Chat.Id, _userRepo.GetLastMessageId(message.Chat.Id));
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: $"✅ Word <b>{_responseWord.Word}</b> is successfully saved!",
                            replyMarkup: KeyboardMarkups.MainMenuKeyboardMarkup,
                            parseMode: ParseMode.Html
                        );

                        await _userRepo.ResetStage(message.Chat.Id);
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
        var message = $"✅ <b>{wordInfo.Word}</b> /{wordInfo.Pronunciation.All}/\n\n" +
                      $"Definition: <i>{wordResult.Definition}</i>\n" +
                      $"Part of speech: <i>{wordResult.PartOfSpeech}</i>\n\n";

        if (wordResult.Synonyms?.Count > 0)
        {
            message += "Synonyms: \n";
            message = wordResult.Synonyms.Aggregate(message, (current, synonym) => current + $"- <i>{synonym}</i>\n");
            message += "\n";
        }
        if (wordResult.Anonyms?.Count > 0)
        {
            message += "Antonyms: \n";
            message = wordResult.Anonyms.Aggregate(message, (current, synonym) => current + $"- <i>{synonym}</i>\n");
            message += "\n";
        }
        if (wordResult.TypeOf?.Count > 0)
        {
            message += "Type of: \n";
            message = wordResult.TypeOf.Aggregate(message, (current, synonym) => current + $"- <i>{synonym}</i>\n");
            message += "\n";
        }
        if (wordResult.PartOf?.Count > 0)
        {
            message += "Part of: \n";
            message = wordResult.PartOf.Aggregate(message, (current, synonym) => current + $"- <i>{synonym}</i>\n");
            message += "\n";
        }
        if (wordResult.Examples?.Count > 0)
        {
            message += "Examples: \n";
            message = wordResult.Examples.Aggregate(message, (current, synonym) => current + $"- <i>{synonym}</i>\n");
            message += "\n";
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

            if (i == definitionCount)
            {
                row = new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData($"Save the word", CommandTypes.Save) };
                buttons.Add(row);
                
                row = new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData($"Try again", CommandTypes.TryAgain) };
                buttons.Add(row);
            }
        }
        return new InlineKeyboardMarkup(buttons);
    }

}