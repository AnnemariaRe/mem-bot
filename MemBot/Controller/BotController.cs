using MemBot.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MemBot.Controller;

[ApiController]
[Route("/message")]
public class BotController : ControllerBase
{
    public BotController(Bot bot, ICommandService commandService)
    {
        _commandService = commandService;
        _botClient = bot.GetClient().Result;
    }

    private readonly ICommandService _commandService;
    private readonly TelegramBotClient _botClient;

    [HttpPost]
    public async Task<IActionResult> UpdateAsync([FromBody]object update)
    {
        var upd = JsonConvert.DeserializeObject<Update>(update.ToString()!);
    
        if (upd?.Message is null && upd?.CallbackQuery is null && upd?.InlineQuery is null)
        {
            return Ok();
        }

        try
        {
            await _commandService.Execute(upd, _botClient);
        }
        catch (Exception e)
        {
            return Ok();
        }

        return Ok();
    }
}
