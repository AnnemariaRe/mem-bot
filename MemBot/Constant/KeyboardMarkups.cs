using Telegram.Bot.Types.ReplyMarkups;

namespace MemBot.Constant;

public static class KeyboardMarkups
{
    public static ReplyKeyboardMarkup MainMenuKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "Add new word" },
        new KeyboardButton[] { "Get random word" },
        new KeyboardButton[] { "Show my added words" },
    }){
        ResizeKeyboard = true
    };
}