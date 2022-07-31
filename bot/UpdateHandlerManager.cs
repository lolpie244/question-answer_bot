using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot;
using BaseCheckDelegate = Func<ITelegramBotClient, Update, Task>;

public abstract class BaseCheckAttribute : Attribute
{
    virtual public bool isFilter
    {
        get;
    }
    [Range(0, 1000)]
    public int Priority { get; set; } = 100;

    public abstract bool check(Update update);
}

public class UpdateHandlerManagerClass
{
    private Dictionary<BaseCheckAttribute, BaseCheckDelegate> Handlers = new Dictionary<BaseCheckAttribute, BaseCheckDelegate>();
    private Dictionary<BaseCheckAttribute, BaseCheckDelegate> Filters = new Dictionary<BaseCheckAttribute, BaseCheckDelegate>();
    private Dictionary<Type, Object?> class_instances = new Dictionary<Type, object?>();

    private void AddAttributeMethods(Type T)
    {
        var methods = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .SelectMany(x => x.GetMethods()) 
            .Where(x => x.GetCustomAttributes(T, false).FirstOrDefault() != null);
        foreach (var method in methods)
        {
            BaseCheckAttribute attribute = (BaseCheckAttribute)method.GetCustomAttribute(T);
            if (!class_instances.ContainsKey(method.DeclaringType))
                class_instances[method.DeclaringType] = Activator.CreateInstance(method.DeclaringType);
            var method_delegate = (BaseCheckDelegate)Delegate.CreateDelegate(
                typeof(BaseCheckDelegate), class_instances[method.DeclaringType], method);
            if(!attribute.isFilter)
                Handlers.Add(attribute, method_delegate);
            else
                Filters.Add(attribute, method_delegate);
        }
    }
    public UpdateHandlerManagerClass()
    {
        foreach (var attribute in HandlerAttribute.GetHandleAttributes())
            AddAttributeMethods(attribute);

        foreach (var attribute in FilterAttribute.GetFilterAttributes())
            AddAttributeMethods(attribute);
    }

    public async Task Run(ITelegramBotClient client, Update update)
    {
        var blocked_methods = new HashSet<BaseCheckDelegate>();
        foreach (var filter in Filters.Keys)
        {
            var method = Filters[filter];
            if(blocked_methods.Contains(method))
                continue;
            if (!filter.check(update))
                blocked_methods.Add(method);
        }

        int? current_priority = null;
        BaseCheckDelegate? current_method = null;
        foreach (var attribute in Handlers.Keys)
        {
            var method = Handlers[attribute];
            if (!blocked_methods.Contains(method) && attribute.check(update))
                if (current_priority == null || Math.Abs(attribute.Priority) < current_priority)
                {
                    current_method = method;
                    current_priority = attribute.Priority;
                }
        }

        if (current_method != null)
        {
            await current_method(client, update);
        }
    }
}

class UpdateHandlerManager
{
    private static UpdateHandlerManagerClass? instance = null;

    public static UpdateHandlerManagerClass Get()
    {
        if (instance == null)
            instance = new UpdateHandlerManagerClass();
        return instance;
    }
}



