using BotSettings;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot;

[Command("/start", "/help")]
public class start_and_help
{
    [Scope(ChatType.Private), UpdateUser]
    public async Task User(ITelegramBotClient client, Update update)
    {
        await client.SendTextMessageAsync(update.GetChat().Id, TEXT.Get("help_user_message"));
    }
    
    [Scope(ChatType.Group, ChatType.Supergroup)]
    public async Task Chat(ITelegramBotClient client, Update update)
    {
        await client.SendTextMessageAsync(update.GetChat().Id, TEXT.Get("help_chat_message"));
    }
    
}