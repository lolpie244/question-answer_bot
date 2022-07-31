using Telegram.Bot.Types;

namespace Bot;

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
}