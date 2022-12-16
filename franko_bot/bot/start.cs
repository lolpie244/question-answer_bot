using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot;

[Command("/start", "/help")]
public class start_and_help: IBotController
{
    [Scope(ChatType.Private), UpdateUser]
    public async Task User(ITelegramBotClient client, Update update)
    {
        await client.SendTextMessageAsync(update.GetChat().Id, TEXT.Get("help_user_message"));
    }
    
    [Scope(ChatType.Group, ChatType.Supergroup)]
    public async Task Chat(ITelegramBotClient client, Update update)
    {
        var chat_id = update.GetChat().Id;
        if ((await client.GetChatMemberAsync(chat_id, (await client.GetMeAsync()).Id)).Status !=
            ChatMemberStatus.Administrator)
        {
            await client.SendTextMessageAsync(chat_id, TEXT.Get("promote_to_admin"));
            return;
        }

        var context = new db_namespace.dbContext();
        var chat = context.Chats.Find(chat_id);
        if (chat == null)
        {
            await new settings().ChangeGroupType(client, update);
            return;
        }

        string text;
        text = chat switch
        {
            { Type: ChatEnum.Answer } => TEXT.Get("help_answer_chat_message"),
            { Type: ChatEnum.Archive } => TEXT.Get("help_chat_message"),
            { Type: ChatEnum.Report } => TEXT.Get("help_chat_message"),
        };
        await client.SendTextMessageAsync(chat_id, text);
            
    }
    
}