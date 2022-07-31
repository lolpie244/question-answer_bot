using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot;

public abstract class FilterAttribute: BaseCheckAttribute
{
    public override bool isFilter { get => true; }
    public static Type[] GetFilterAttributes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(FilterAttribute))).ToArray();
    }
}

public class ScopeAttribute : FilterAttribute
{
    private ChatType[] Scopes;

    public ScopeAttribute(params ChatType[] scopes)
    {
        Scopes = scopes;
    }

    public override bool check(Update update)
    {
        var chat = update.GetChat();
        if(chat != null)
            return Scopes.Contains(chat.Type);
        return true;
    }
}