using BotSettings;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot;

[Command("/start")]
public class start
{
    [Scope(ChatType.Private), UpdateUser]
    public async Task User(ITelegramBotClient client, Update update)
    {
        await client.SendTextMessageAsync(update.GetChat().Id, "hello user, you registered now");
    }
    
    [Scope(ChatType.Group, ChatType.Supergroup)]
    public async Task Chat(ITelegramBotClient client, Update update)
    {
        await client.SendTextMessageAsync(update.GetChat().Id, "hello chat");
    }
    
}