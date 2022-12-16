
using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MessageType = db_namespace.MessageType;

namespace Bot;

public class question_answer: IBotController
{
    
    [Message(Priority = 1000), Scope(ChatType.Private), Role(RoleEnum.User, true)]
    public async Task Ask(ITelegramBotClient client, Update update)
    {
        var context = new dbContext();

        db_namespace.Chat[] chats = context.Chats.Where(obj => obj.Type == ChatEnum.Answer).ToArray();

        var message = update.Message!;
        var asker_id = message.From!.Id;
        foreach (var chat in chats)
        {
            int new_message_id = (await client.CopyMessageAsync(chat.Id, message.Chat.Id, 
                message.MessageId, replyMarkup: new Keyboards(update).Ask())).Id;
            context.Archive.Add(new Archive
            {
                MessageId = new_message_id, ChatId = chat.Id, UserId = asker_id, RelatedUserId = asker_id
            });
        }

        await context.SaveChangesAsync();
    }

    private async Task<bool> AnswerQuestion(ITelegramBotClient client, Update update)
    {
        var message = update.Message;
        var message_that_replied = message!.ReplyToMessage;
        if(message_that_replied == null || message_that_replied.From!.Id != client.GetMeAsync().Result.Id)
            return false;
        var context = new dbContext();
        var question =
            context.Archive.FirstOrDefault(obj => obj.Type == MessageType.QA && obj.MessageId == message_that_replied.MessageId);
        if (question == null || question.RelatedUserId == null)
            return false;
        
        context.Archive.Add(new Archive(message, MessageType.QA,  question.RelatedUserId));
        await context.SaveChangesAsync();
        await client.CopyMessageAsync(question.RelatedUserId, message.Chat.Id, message.MessageId);
        return true;
    }
    [Message(Priority = 1000), Role(RoleEnum.Moderator, true)]
    public async Task Answer(ITelegramBotClient client, Update update)
    {
        await AnswerQuestion(client, update);
    }

    [Message(Priority = 1000), Role(RoleEnum.Moderator, true), Scope(ChatType.Private)]
    public async Task AnswerInPrivate(ITelegramBotClient client, Update update)
    {
        if (!await AnswerQuestion(client, update))
            await Ask(client, update);
    }

    [Message(Priority = 1000), Role(RoleEnum.Banned), Scope(ChatType.Private)]
    public async Task BannedTryAsk(ITelegramBotClient client, Update update)
    {
        await client.SendTextMessageAsync(update.GetChat().Id, TEXT.Get("banned_cant_ask"));
    }
}