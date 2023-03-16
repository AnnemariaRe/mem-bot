using System.Data;
using System.Threading.Tasks;
using MemBot.Action;
using MemBot.Constant;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MemBot.Service
{
    public class HandleUpdateService : IHandleUpdateService
    {
        public async Task Execute(Update? update, TelegramBotClient client)
        {
            if (update is null)
            {
                return;
            }
            
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
                case CommandTypes.Start:
                    await MessageAction.StartChat(client, message);
                    return;
                //"/switch" => MessageAction.SwitchMegaAccount(client, message),
            }
        }

        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, TelegramBotClient client)
        {

        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            return Task.CompletedTask;
        }
    }
}