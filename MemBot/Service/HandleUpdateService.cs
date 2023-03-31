using MemBot.Command;
using MemBot.Constant;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MemBot.Service
{
    public class HandleUpdateService : IHandleUpdateService
    {
        private readonly List<ICommand> _commands;
        private string? _lastCommandKey;

        public HandleUpdateService(IServiceProvider serviceProvider)
        {
            _lastCommandKey = null;
            _commands = serviceProvider.GetServices<ICommand>().ToList();
        }
        
        public async Task Execute(Update? update, TelegramBotClient client)
        {
            var id = update.Type switch
            {
                UpdateType.Message => update.Message.Chat.Id,
                UpdateType.CallbackQuery => update.CallbackQuery.Message.Chat.Id,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            switch (update)
            {
                case { Type: UpdateType.CallbackQuery }:
                {
                    if (update.CallbackQuery?.Data != null)
                    {
                        
                    }
                    break;
                }
            }
            
            var messageText = update.Message?.Text;
            var callbackQuery = update?.CallbackQuery;
            
            if (messageText is null && callbackQuery is null)
                return;
            
            switch (messageText)
            {
                case Commands.StartCommand:
                    await ExecuteCommand(Commands.StartCommand, id, update, client);
                    return;
            }
            
            switch (_lastCommandKey)
            {
                
            }
        }

    private async Task ExecuteCommand(string? key, long? id, Update? update, ITelegramBotClient client)
    {
        _lastCommandKey = key;
        await _commands.First(x => x.Key == key).Execute(update, client);
    }
    }
}