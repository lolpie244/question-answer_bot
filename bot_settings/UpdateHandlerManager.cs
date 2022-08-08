using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotSettings;
using BaseCheckDelegate = Func<ITelegramBotClient, Update, Task>;

public abstract class BaseCheckAttribute : Attribute
{
    virtual public bool isFilter { get; }
    [Range(0, 1000)] public int Priority { get; set; } = 100;

    public abstract bool check(ITelegramBotClient client, Update update);
}

public class UpdateHandlerManagerClass
{
    private Dictionary<BaseCheckAttribute, LinkedList<BaseCheckDelegate>> Handlers = new();
    private Dictionary<BaseCheckDelegate, LinkedList<BaseCheckAttribute>> Filters = new();
    private Dictionary<Type, Object?> class_instances = new();
    private Dictionary<UpdateType, LinkedList<BaseCheckAttribute>> eventAttributes = new();

    public UpdateHandlerManagerClass()
    {
        var methods = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .SelectMany(x => x.GetMethods())
            .Where(x => x.CustomAttributes.Count() != 0 || 
                        x.DeclaringType!.CustomAttributes.Count() != 0).ToArray();

        foreach (var method in methods)
        {
            var attributes = method.GetCustomAttributes().Concat(method.DeclaringType.GetCustomAttributes());
            foreach (var raw_attribute in attributes)
            {
                if (!(raw_attribute is BaseCheckAttribute attribute))
                    continue;
                if (!class_instances.ContainsKey(method.DeclaringType))
                    class_instances[method.DeclaringType] = Activator.CreateInstance(method.DeclaringType);
                var method_delegate = (BaseCheckDelegate)Delegate.CreateDelegate(
                    typeof(BaseCheckDelegate), class_instances[method.DeclaringType], method);

                if (!attribute.isFilter)
                {
                    if (!Handlers.ContainsKey(attribute))
                        Handlers[attribute] = new LinkedList<BaseCheckDelegate>();
                    Handlers[attribute].AddLast(method_delegate);
                    if (attribute is HandlerAttribute handler)
                    {
                        if(!eventAttributes.ContainsKey(handler.UpdateType))
                            eventAttributes[handler.UpdateType] = new LinkedList<BaseCheckAttribute>();
                        eventAttributes[handler.UpdateType].AddLast(attribute);
                    }
                }
                else
                {
                    if (!Filters.ContainsKey(method_delegate))
                        Filters[method_delegate] = new LinkedList<BaseCheckAttribute>();
                    Filters[method_delegate].AddLast(attribute);
                }
            }
        }
    }

    public async Task Run(ITelegramBotClient client, Update update)
    {
        var methods = new LinkedList<Tuple<BaseCheckDelegate, int>>();
        if(!eventAttributes.Keys.Contains(update.Type))
            return;
        foreach (var attribute in eventAttributes[update.Type])
            if (attribute.check(client, update))
                foreach(var method in Handlers[attribute])
                    methods.AddLast(new Tuple<BaseCheckDelegate, int>(method, attribute.Priority));
        
        int? current_priority = Int32.MaxValue;
        BaseCheckDelegate? current_method = null;
        foreach (var method_priority in methods)
        {
            var method = method_priority.Item1;
            if(Filters.ContainsKey(method))
                foreach (var filter in Filters[method])
                    if (!filter.check(client, update))
                        goto Next;
            var priority = method_priority.Item2;
            if (priority < current_priority)
            {
                current_priority = priority;
                current_method = method;
            }
            Next:
            continue;
        }

        if (current_method != null)
            await current_method(client, update);
    }
}

class UpdateHandlerManager
{
    private static UpdateHandlerManagerClass? instance;

    public static UpdateHandlerManagerClass Get()
    {
        if (instance == null)
            instance = new UpdateHandlerManagerClass();
        return instance;
    }
}



