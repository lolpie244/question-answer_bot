
using BotSettings;
using db_namespace;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot;

public class question_answer
{
    [Message(Priority = 1000), Scope(ChatType.Private)]
    public async Task Ask(ITelegramBotClient client, Update update)
    {
        await client.CopyMessageAsync(update.GetChat().Id, update.GetChat().Id, update.Message.MessageId);
    }
}