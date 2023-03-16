using MemBot.Action;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MemBot.Service;

public class HandleUpdateService : IHandleUpdateService
{
    public async Task Execute(Update? update, TelegramBotClient client)
    {
        var handler = update.Type switch
        {
            UpdateType.Message => BotOnMessageReceived(update.Message!, client),
            UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery!, client),
            _ => UnknownUpdateHandlerAsync(update)
        };
    }

    private async Task BotOnMessageReceived(Message message, TelegramBotClient client)
    {
        switch (message.Text)
        {
            case "/start":
                await MessageAction.StartChat(client, message);
                return;
            //"/switch" => MessageAction.SwitchMegaAccount(client, message),
        };
    }
    
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, TelegramBotClient client)
    {
        
    }
    
    private Task UnknownUpdateHandlerAsync(Update update)
    {
        return Task.CompletedTask;
    }
}