using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace BotSettings;

internal static class Extensions
{
    public static User GetUser(this Update update) =>
        update switch
        {
            { Message.From: { } user } => user,
            { InlineQuery.From: { } user } => user,
            { CallbackQuery.From: { } user } => user,
            { PreCheckoutQuery.From: { } user } => user,
            { ShippingQuery.From: { } user } => user,
            { ChosenInlineResult.From: { } user } => user,
            { PollAnswer.User: { } user } => user,
            { MyChatMember.NewChatMember.User: { } user } => user,
            { ChatMember.NewChatMember.User: { } user } => user,
            { EditedMessage.From: { } user } => user,
            { ChatJoinRequest.From: { } user } => user,
            _ => throw new ArgumentException("Unsupported update type {0}.", update.Type.ToString())
        };
    public static Chat? GetChat(this Update update) =>
        update switch
        {
            { Message.Chat: { } chat } => chat,
            { InlineQuery.ChatType: { } chatType } => new Chat(){Type = chatType},
            { CallbackQuery.Message.Chat: { } chat } => chat,
            { PreCheckoutQuery: { }} => null,
            { ShippingQuery.From: { }  } => null,
            { ChosenInlineResult.From: { }  } => null,
            { PollAnswer.User: { }  } => null,
            { MyChatMember.Chat: { } chat } => chat,
            { ChatMember.Chat: { } chat } => chat,
            { EditedMessage.Chat: { } chat } => chat,
            { ChatJoinRequest.Chat: { } chat } => chat,
            _ => throw new ArgumentException("Unsupported update type {0}.", update.Type.ToString())
        };
    public static Message? GetMessage(this Update update) =>
        update switch
        {
            { Message: { } message } => message,
            { CallbackQuery.Message: { } message } => message,
            { ChosenInlineResult.InlineMessageId: { }  } => null,
            { EditedMessage: { } message } => message,
            _ => throw new ArgumentException("Unsupported update type {0}.", update.Type.ToString())
        };
    public static string FullName(this User user)
    { 
        return user.FirstName + (user.LastName != "" ? $" {user.LastName}" : "");
    }
    public static string? GetStage(this Update update)
    {
        var user_id = update.GetUser().Id;
        string? result = null;
        using (var context = new InMemoryContext())
        {
            var stage = context.Stages.Find(user_id);
            if (stage != null)
                result = stage.StageName;
            context.SaveChangesAsync();
        }

        return result;
    }

    public static void SetStage(this Update update, string? stage_name)
    {
        var user_id = update.GetUser().Id;
        using (var context = new InMemoryContext())
        {
            var stage = context.Stages.Find(user_id);
            if (stage == null)
                context.Stages.Add(new StageModel { user_id = user_id, StageName = stage_name });
            else
                stage.StageName = stage_name;
            context.SaveChanges();

        }
    }
}