using Telegram.Bot.Types.ReplyMarkups;

namespace MemBot.Constant
{
    public static class KeyboardMarkups
    {
        public static ReplyKeyboardMarkup MainMenuKeyboardMarkup = new(new[]
        {
            new KeyboardButton[] {Commands.GuessTheWordCommand},
            new KeyboardButton[] {Commands.AddWordCommand, Commands.GetRandomWordCommand},
            new KeyboardButton[] {Commands.ShowWordsCommand},
        })
        {
            ResizeKeyboard = true
        };
    }
}