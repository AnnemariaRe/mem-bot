using Telegram.Bot.Types.ReplyMarkups;

namespace MemBot.Constant;

public static class InlineKeyboards
{
    public static InlineKeyboardMarkup SaveAndBackInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("Save the word", "/save") },
        new[] { InlineKeyboardButton.WithCallbackData("Back", "/back") }
    });
    
    public static InlineKeyboardMarkup SaveAndTryAgainInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("Save the word", "/save") },
        new[] { InlineKeyboardButton.WithCallbackData("Try again", "/try_again") }
    });
    
    public static InlineKeyboardMarkup SaveInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("Save the word", "/save") },
    });
    
    public static InlineKeyboardMarkup BackInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("Back", "/back") }
    });
    
    
}