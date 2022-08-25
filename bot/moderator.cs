using System.ComponentModel;
using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot;

[Role(RoleEnum.Moderator, true)]
public class moderator
{
    [InlineButtonCallback("close_question")]
    public async Task CloseQuestion(ITelegramBotClient client, Update update)
    {
        var context = new db_namespace.dbContext();
        var chats = context.Chats.Where(obj => obj.Type == ChatEnum.Archive).ToArray();

        var asker = context.Archive.First(obj => obj.MessageId == update.GetMessage()!.MessageId
                                                 && obj.ChatId == update.GetChat()!.Id).RelatedUserId;
        if(asker == null)
            return;
        
        var messages = context.Archive.Where(obj => obj.Type == MessageType.QA && obj.RelatedUserId == asker);

        var keyboard = new Keyboards();
        string closed_at = DateTime.Now.ToString();
        
        foreach (var chat in chats)
        {
            await client.SendTextMessageAsync(chat.Id, 
                String.Format(TEXT.Get("new_conversation_in_archive"), closed_at));
            foreach (var message in messages)
                await client.CopyMessageAsync(chat.Id, message.ChatId, message.MessageId,
                    replyMarkup: keyboard.History(message));
        }

        foreach (var message in messages)
        {
            try
            {
                await client.DeleteMessageAsync(message.ChatId, message.MessageId);
            }
            catch
            {
                if (message.IsQuestion)
                    await client.EditMessageTextAsync(message.ChatId, message.MessageId, TEXT.Get("question_removed"));
            }
        }
        context.Archive.RemoveRange(messages);
        await context.SaveChangesAsync();
    }
}