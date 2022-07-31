using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot;

public abstract class HandlerAttribute: BaseCheckAttribute
{
    public override bool isFilter { get => false; }

    public static Type[] GetHandleAttributes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(HandlerAttribute))).ToArray();
    }
}
public class CommandAttribute : HandlerAttribute
{
    public string[] Commands { get; set; }

    public CommandAttribute(params string[] command)
    {
        Commands = command;
    }
    public override bool check(Update update)
    {
        if (update.Type != UpdateType.Message || update.Message.Text == null)
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
        if(update.Type != UpdateType.Message || update.Message.Text == null)
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
    public string? CallbackData;

    public InlineButtonCallbackAttribute(string callbackData = "")
    {
        CallbackData = callbackData;
    }
    public override bool check(Update update)
    {
        return update.Type == UpdateType.CallbackQuery && update.CallbackQuery.Data == CallbackData;
    }
}

public class MessageAttribute : HandlerAttribute
{
    public MessageType[] Types;

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