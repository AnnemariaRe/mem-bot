using Telegram.Bot;
using Telegram.Bot.Types;

namespace MemBot.Service;

public interface ICommandService
{
    public Task Execute(Update? update, TelegramBotClient client);
}