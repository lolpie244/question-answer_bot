using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using BotSettings;

namespace settings;

public class BotWebhookController : ControllerBase
{
    private TelegramBotClient client;

    [HttpPost]
    public async Task<IActionResult> Post([FromServices] BotService botService, [FromBody] Update update)
    {
        await botService.GetUpdate(update);
        return Ok();
    }
}