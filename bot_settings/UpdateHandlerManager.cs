using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotSettings;
using BaseCheckDelegate = Func<ITelegramBotClient, Update, Task>;

public abstract class BaseCheckAttribute : Attribute
{
    virtual public bool isFilter { get; }
    [Range(0, 1000)] public int Priority { get; set; } = 100;

    public abstract bool check(Update update);
}

public class UpdateHandlerManagerClass
{
    private Dictionary<BaseCheckAttribute, LinkedList<BaseCheckDelegate>> Handlers = new();
    private Dictionary<BaseCheckDelegate, LinkedList<BaseCheckAttribute>> Filters = new();
    private Dictionary<Type, Object?> class_instances = new();

    private void AddAttributeMethods(Type T, MethodInfo[] all_methods)
    {
        var methods = all_methods.Where(x => x.GetCustomAttributes(T, false).FirstOrDefault() != null);
        foreach (var method in methods)
        {
            BaseCheckAttribute attribute = (BaseCheckAttribute)method.GetCustomAttribute(T);
            
            if (!class_instances.ContainsKey(method.DeclaringType))
                class_instances[method.DeclaringType] = Activator.CreateInstance(method.DeclaringType);
            var method_delegate = (BaseCheckDelegate)Delegate.CreateDelegate(
                typeof(BaseCheckDelegate), class_instances[method.DeclaringType], method);

            if (!attribute.isFilter)
            {
                if (!Handlers.ContainsKey(attribute))
                    Handlers[attribute] = new LinkedList<BaseCheckDelegate>();
                Handlers[attribute].AddLast(method_delegate);
            }
            else
            {
                if (!Filters.ContainsKey(method_delegate))
                    Filters[method_delegate] = new LinkedList<BaseCheckAttribute>();
                Filters[method_delegate].AddLast(attribute);
            }
        }
    }
    public UpdateHandlerManagerClass()
    {
        var methods = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .SelectMany(x => x.GetMethods()).Where(x => x.CustomAttributes.Count() != 0).ToArray();
        
        foreach (var attribute in HandlerAttribute.GetHandleAttributes())
            AddAttributeMethods(attribute, methods);

        foreach (var attribute in FilterAttribute.GetFilterAttributes())
            AddAttributeMethods(attribute, methods);
    }

    public async Task Run(ITelegramBotClient client, Update update)
    {
        var methods = new LinkedList<Tuple<BaseCheckDelegate, int>>();
        foreach (var attribute in Handlers.Keys)
            if (attribute.check(update))
                foreach(var method in Handlers[attribute])
                    methods.AddLast(new Tuple<BaseCheckDelegate, int>(method, attribute.Priority));
        
        int? current_priority = Int32.MaxValue;
        BaseCheckDelegate? current_method = null;
        var blocked_methods = new HashSet<BaseCheckDelegate>();
        foreach (var method_priority in methods)
        {
            var method = method_priority.Item1;
            if(Filters.ContainsKey(method))
                foreach (var filter in Filters[method])
                    if (!filter.check(update))
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



