using System.ComponentModel;
using BotSettings;
using db_namespace;
using helping;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot;

[Role(RoleEnum.Moderator, true)]
public class moderator: IBotController
{
    public long? get_related_user_id_for_message(Message message)
    {
        var context = new db_namespace.dbContext();
        return context.Archive.First(obj => obj.Type == MessageType.QA 
                                            && obj.MessageId == message.MessageId
                                            && obj.ChatId == message.Chat.Id).RelatedUserId;
    }
    [InlineButtonCallback("close_question")]
    public async Task CloseQuestion(ITelegramBotClient client, Update update)
    {
        var context = new dbContext();
        var chats = context.Chats.Where(obj => obj.Type == ChatEnum.Archive).ToArray();

        var asker_id = get_related_user_id_for_message(update.GetMessage()!);
        if(asker_id == null)
            return;
        var asker = await context.Users.FindAsync(asker_id);
        var messages = context.Archive.Where(obj => obj.Type == MessageType.QA && obj.RelatedUserId == asker_id).ToArray();

        var keyboard = new Keyboards();
        string closed_at = DateTime.Now.ToString();
        
        foreach (var chat in chats)
        {
            await client.SendTextMessageAsync(chat.Id, 
                String.Format(TEXT.Get("new_conversation_in_archive"), closed_at));
            foreach (var message in messages)
                try
                {
                    await client.CopyMessageAsync(chat.Id, message.ChatId, message.MessageId,
                        replyMarkup: keyboard.History(message, asker));
                }
                catch (Exception e)
                {
                    await client.SendTextMessageAsync(chat.Id, TEXT.Get("deleted_message"),
                        replyMarkup: keyboard.History(message, asker));
                }
                
        }

        foreach (var message in messages)
        {
            try
            {
                await client.DeleteMessageAsync(message.ChatId, message.MessageId);
            }
            catch
            {
                try
                {
                    if (message.IsQuestion)
                        await client.EditMessageTextAsync(message.ChatId, message.MessageId, TEXT.Get("question_removed"));
                }
                catch{}
            }
        }
        context.Archive.RemoveRange(messages);
        await context.SaveChangesAsync();
        await client.SendTextMessageAsync(asker_id, TEXT.Get("question_closed_to_user"));
        await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, Strings.Format(TEXT.Get("question_closed"), asker.Name));
    }

    [InlineButtonCallback("ban_request:user_id=.*")]
    public async Task BanRequest(ITelegramBotClient client, Update update)
    {
        var context = new dbContext();
        var original_message = update.GetMessage();
        var user_id = long.Parse(Helping.get_data_from_string(update.CallbackQuery.Data)["user_id"]);
        var user = await context.Users.FindAsync(user_id);
        if (user.Role == RoleEnum.Banned)
        {
            await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, TEXT.Get("user_already_banned"));
            return;
        }
        await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, TEXT.Get("ban_request_sent"));
        
        var chats = context.Chats.Where(obj => obj.Type == ChatEnum.Report).ToArray();
        var keyboard = new Keyboards(update).Ban(user, true);
        foreach (var chat in chats)
        {
            await client.SendTextMessageAsync(chat.Id, TEXT.Get("ban_preview"));
            await client.CopyMessageAsync(chat.Id, original_message.Chat.Id, 
                original_message.MessageId, TEXT.Get("ban_preview"), replyMarkup:keyboard);
        }
    }
}