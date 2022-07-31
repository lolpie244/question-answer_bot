using BotSettings;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot;

public class Testt
{
    [Command("/start", Priority = 0), Scope(ChatType.Group)]
    public async Task StartCommands(ITelegramBotClient client, Update update)
    {
        var message = update.Message;
        Console.WriteLine(message.Chat.Type);
        await client.SendTextMessageAsync(message.Chat.Id, "Hello user");
    }
}