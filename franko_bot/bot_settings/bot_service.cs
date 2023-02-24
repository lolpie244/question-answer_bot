using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotSettings;

public class BotService
{
    private readonly ITelegramBotClient client;
    private readonly ILogger<BotService> logger;
    private readonly UpdateHandlerManager updateHandler;

    public BotService(ITelegramBotClient botClient, ILogger<BotService> logger, UpdateHandlerManager updateHandler)
    {
        client = botClient;
        this.logger = logger;
        this.updateHandler = updateHandler;
    }

    public async Task GetUpdate(Update update)
    {
        var start = Stopwatch.StartNew();
        logger.LogInformation("START REQUEST");
        await updateHandler.Run(client, update);
        logger.LogInformation($"END REQUEST IN {start.ElapsedMilliseconds} ms");
    }
}
