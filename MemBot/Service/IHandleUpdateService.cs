using Telegram.Bot;
using Telegram.Bot.Types;

namespace MemBot.Service;

public interface IHandleUpdateService
{
    public Task Execute(Update? update, TelegramBotClient client);
}