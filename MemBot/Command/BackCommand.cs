using MemBot.Constant;
using MemBot.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MemBot.Command;

public class BackCommand : ICommand
{
    public BackCommand(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    public string Key => Commands.BackCommand;
    private readonly IUserRepo _userRepo;
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Message!;
        var userData = await _userRepo.GetUser(message.Chat.Id);

        if (userData != null)
        {
            _userRepo.ResetStage(message.Chat.Id);
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose action ⏬",
                replyMarkup: KeyboardMarkups.MainMenuKeyboardMarkup
            );
        }
    }
}