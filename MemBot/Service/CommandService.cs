using Telegram.Bot;
using Telegram.Bot.Types;

namespace MemBot.Service;

public class CommandService : ICommandService
{
    public async Task Execute(Update? update, TelegramBotClient client)
    {
        await client.SendTextMessageAsync(update.Message.Chat.Id, "Hello!");
    }
}