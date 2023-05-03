using MemBot.Constant;
using MemBot.Repository;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MemBot.Command;

public class StartCommand : ICommand
{
    public string Key => Commands.StartCommand;
    private readonly IUserRepo _userRepo;

    public StartCommand(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update?.Message!;
        var userData = await _userRepo.GetUser(message.Chat.Id)!;
         
        if (userData is null)
        {
            await _userRepo.CreateUser(message);
        }

        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Welcome to a MemBot! 👋🏼 \nHere you can add english words and I will help you to memorize it 🧠",
            replyMarkup: KeyboardMarkups.MainMenuKeyboardMarkup
        );
    }
}