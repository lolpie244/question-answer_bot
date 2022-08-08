using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Chat = Telegram.Bot.Types.Chat;

namespace Bot;

public class settings
{
    [BotStatusInGroup]
    public async Task BotStatusChanged(ITelegramBotClient client, Update update)
    {
        var status = update.MyChatMember!.NewChatMember.Status;
        
        using (var context = new dbContext())
        {
            var chat = context.Chats.Find(update.GetChat().Id);
            if (chat != null)
                context.Chats.Remove(chat);
            await context.SaveChangesAsync();
        }
        
        if (status == ChatMemberStatus.Member)
            await client.SendTextMessageAsync(update.GetChat().Id, TEXT.Get("promote_to_admin"));
        

        if (status == ChatMemberStatus.Administrator || status == ChatMemberStatus.Creator)
            await ChangeGroupType(client, update);
    }
    [Command("/change_group_type"), Scope(ChatType.Group, ChatType.Supergroup), Role(RoleEnum.Moderator, true)]
    public async Task ChangeGroupType(ITelegramBotClient client, Update update)
    {
        await client.SendTextMessageAsync(update.GetChat().Id, TEXT.Get("select_chat_type"), 
            replyMarkup: new Keyboards(update).ChatType());
    }

    [InlineButtonCallback("answer_group", "archive_group")]
    public async Task ChatStatusChanged(ITelegramBotClient client, Update update)
    {
        var data = update.CallbackQuery.Data;
        var message = update.GetMessage();
        string text;
        ChatEnum chat_type;

        if (data == "answer_group")
        {
            text = TEXT.Get("become_chat_answer");
            chat_type = ChatEnum.Answer;
        }
        else
        {
            text = TEXT.Get("become_chat_archive");
            chat_type = ChatEnum.Archive;
        }

        using (var context = new dbContext())
        {
            context.Chats.Add(new db_namespace.Chat { Id = message.Chat.Id, Type = chat_type });
            await context.SaveChangesAsync();
        }
        await client.EditMessageTextAsync(message.Chat.Id, message.MessageId, text);
            
    }
}