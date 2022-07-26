using Telegram.Bot;
using Telegram.Bot.Types;

namespace settings;

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
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            await client.SendTextMessageAsync(update.Message.Chat.Id, update.Message.Text);
    }
    
}