using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotSettings;

public abstract class HandlerAttribute: BaseCheckAttribute
{
    public abstract UpdateType UpdateType { get; }
    public override bool isFilter { get => false; }
}
public class CommandAttribute : HandlerAttribute
{
    public string[] Commands { get; set; }
    
    public override UpdateType UpdateType
    {
        get => UpdateType.Message;
    }
    public CommandAttribute(params string[] command)
    {
        Commands = command;
    }
    public override bool check(ITelegramBotClient client, Update update)
    {
        if (update.Message.Text == null)
            return false;
        
        var my_name = client.GetMeAsync().Result.Username;
        
        var message = update.Message.Text.TrimStart();
        var message_command = message.Split(' ')[0];
        foreach (var command in Commands)
            if (message_command == command || message_command == $"{command}@{my_name}")
                return true;
        return false;
    }
}

public class TextAttribute : HandlerAttribute
{
    public override UpdateType UpdateType
    {
        get => UpdateType.Message;
    }
    public string[] Texts { get; set; }

    public TextAttribute(params string[] text)
    {
        Texts = text;
    }
    public override bool check(ITelegramBotClient client, Update update)
    {
        if(update.Message.Text == null)
            return false;
        if (Texts.Length == 0)
            return true;
        foreach (var some_text in Texts)
        {
            var regex = new Regex("^" + some_text + "$");
            if (regex.IsMatch(update.Message.Text))
                return true;
        }

        return false;
    }

}

public class InlineButtonCallbackAttribute : HandlerAttribute
{
    public string[]? CallbackData;
    public override UpdateType UpdateType
    {
        get => UpdateType.CallbackQuery;
    }
    public InlineButtonCallbackAttribute(params string[] callbackData)
    {
        CallbackData = callbackData;
    }
    public override bool check(ITelegramBotClient client, Update update)
    {
        foreach (var callback_data in CallbackData)
        {
            var regex = new Regex("^" + callback_data + "$");
            if (regex.IsMatch(update.CallbackQuery.Data))
                return true;
        }

        return false;
    }
}

public class MessageAttribute : HandlerAttribute
{
    public MessageType[] Types;
    public override UpdateType UpdateType
    {
        get => UpdateType.Message;
    }
    public MessageAttribute(params MessageType[] type)
    {
        Types = type;
    }
    public override bool check(ITelegramBotClient client, Update update)
    {
        if (update.Type != UpdateType.Message)
            return false;
        if (Types.Length == 0)
            return true;
        foreach (var type in Types)
            if (type == update.Message.Type)
                return true;
        return false;
    }
}

public class BotStatusInGroupAttribute : HandlerAttribute
{
    public override UpdateType UpdateType
    {
        get => UpdateType.MyChatMember;
    }
    public override bool check(ITelegramBotClient client, Update update)
    {
        return true;
    }
}