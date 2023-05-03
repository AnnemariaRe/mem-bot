using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MemBot.Command;

public interface ICommand
{
    public string Key { get; }
    public Task Execute(Update? update, ITelegramBotClient client);
}