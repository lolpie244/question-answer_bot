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
    public override bool check(Update update)
    {
        if (update.Message.Text == null)
            return false;

        var message = update.Message.Text.TrimStart();
        var message_command = message.Split(' ')[0];
        foreach (var command in Commands)
            if (message_command == command)
                return true;
        return false;
    }
}

public class ContainsTextAttribute : HandlerAttribute
{
    public override UpdateType UpdateType
    {
        get => UpdateType.Message;
    }
    public string[] Texts { get; set; }
    public bool AtStart { get; set; }
    public bool AtEnd { get; set; }
    public bool CaseInsensetive { get; set; }
    public bool Trim { get; set; }

    public ContainsTextAttribute(params string[] text)
    {
        Texts = text;
    }
    public override bool check(Update update)
    {
        if(update.Message.Text == null)
            return false;
        if (Texts.Length == 0)
            return true;
        
        var messageText = update.Message.Text;
        if (this.CaseInsensetive)
            messageText = messageText.ToLower();
        if (this.Trim)
            messageText = messageText.Trim();

        foreach (var text in Texts)
        {
            if (AtStart && messageText.StartsWith(text) ||
                AtEnd && messageText.EndsWith(text) ||
                !AtStart && !AtEnd && messageText.Contains(text))
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
    public override bool check(Update update)
    {
        return  CallbackData.Contains(update.CallbackQuery.Data);
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
    public override bool check(Update update)
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
    public override bool check(Update update)
    {
        return true;
    }
}