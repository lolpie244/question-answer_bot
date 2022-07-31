using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotSettings;

public class BotService
{
    private readonly ITelegramBotClient client;
    private readonly ILogger<BotService> logger;

    public BotService(ITelegramBotClient botClient, ILogger<BotService> logger)
    {
        client = botClient;
        this.logger = logger;
        
    }

    public async Task GetUpdate(Update update)
    {
        logger.LogInformation("IN GET UPDATE");
        await UpdateHandlerManager.Get().Run(client, update);
    }
}