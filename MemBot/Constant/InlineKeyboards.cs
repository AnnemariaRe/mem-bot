using Telegram.Bot.Types.ReplyMarkups;

namespace MemBot.Constant
{
    public static class InlineKeyboards
    {
        public static InlineKeyboardMarkup SaveAndBackInlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Save the word", CommandTypes.Save)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Back", CommandTypes.Back)
            }
        });

        public static InlineKeyboardMarkup SaveAndTryAgainInlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Save the word", CommandTypes.Save)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Try again", CommandTypes.TryAgain)
            }
        });

        public static InlineKeyboardMarkup SaveInlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Save the word", CommandTypes.Save)
            },
        });

        public static InlineKeyboardMarkup BackInlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Back", CommandTypes.Back)
            }
        });

    }
}