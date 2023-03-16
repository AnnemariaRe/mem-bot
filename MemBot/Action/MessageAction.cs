using Telegram.Bot;
using Telegram.Bot.Types;

namespace MemBot.Action;

public class MessageAction : IAction
{
    public static async Task StartChat(TelegramBotClient client, Message message)
    {
        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Welcome to a MemBot! \nHere you can add english words and I will help you to memorize it :)"
        );
    }
}