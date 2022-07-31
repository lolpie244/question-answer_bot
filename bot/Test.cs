using BotSettings;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot;

public class SomeClass
{
    [Command("/start")]
    public async Task StartCommand(ITelegramBotClient client, Update update)
    {
        var message = update.Message;
        Console.WriteLine(message.Chat.Type);
        await client.SendTextMessageAsync(message.Chat.Id, "Hi user");
    }
    [Command("/end", "/good_bye"), Scope(ChatType.Supergroup, ChatType.Group)]
    public async Task EndCommand(ITelegramBotClient client, Update update)
    {
        var message = update.Message;
        Console.WriteLine(message.Chat.Type);
        await client.SendTextMessageAsync(message.Chat.Id, "Bye user");
    }
    
    [ContainsText("hello there", "hi there", "general grievous", CaseInsensetive = true, Priority = 0)]
    public async Task Answer(ITelegramBotClient client, Update update)
    {
        var message = update.Message;
        await client.SendTextMessageAsync(message.Chat.Id, "General Kenobi");
    }
    
    [ContainsText]
    public async Task Echo(ITelegramBotClient client, Update update)
    {
        var message = update.Message;
        await client.SendTextMessageAsync(message.Chat.Id, message.Text);
        
    
    }

    [Message(MessageType.Sticker, MessageType.Video, MessageType.Document, Priority = 1000)]
    public async Task NiceGif(ITelegramBotClient client, Update update)
    {
        await client.CopyMessageAsync(update.GetChat().Id, update.GetChat().Id, update.Message.MessageId);
    }
}