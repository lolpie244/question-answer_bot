using BotSettings;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot;

public class super_admin
{
    [InlineButtonCallback("close_report")]
    public async Task CloseReport(ITelegramBotClient client, Update update)
    {
        Console.WriteLine("KEEEEEEEEEEEEEEEEEK");
        await client.DeleteMessageAsync(update.GetChat().Id, update.GetMessage().MessageId);
    }
}