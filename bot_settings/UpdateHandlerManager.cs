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

public class UpdateHandlerManager
{
    private Dictionary<BaseCheckAttribute, LinkedList<BaseCheckDelegate>> Handlers = new();
    private Dictionary<BaseCheckDelegate, LinkedList<BaseCheckAttribute>> Filters = new();
    private Dictionary<Type, Object?> class_instances = new();
    private Dictionary<UpdateType, List<BaseCheckAttribute>> eventAttributes = new();
    public UpdateHandlerManager()
    {
        Console.WriteLine("Start sync attributes");
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
                BaseCheckDelegate method_delegate;
                try
                {
                    method_delegate = (BaseCheckDelegate)Delegate.CreateDelegate(
                        typeof(BaseCheckDelegate), class_instances[method.DeclaringType], method);
                }
                catch
                {
                    Console.WriteLine($"Method \"{method}\" has wrong signature");
                    continue;
                }
                if (!attribute.isFilter)
                {
                    if (!Handlers.ContainsKey(attribute))
                        Handlers[attribute] = new LinkedList<BaseCheckDelegate>();
                    Handlers[attribute].AddLast(method_delegate);
                    if (attribute is HandlerAttribute handler)
                    {
                        if(!eventAttributes.ContainsKey(handler.UpdateType))
                            eventAttributes[handler.UpdateType] = new List<BaseCheckAttribute>();
                        eventAttributes[handler.UpdateType].Add(attribute);
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

        foreach (var key in eventAttributes.Keys)
            eventAttributes[key].Sort((a, b) => a.Priority.CompareTo(b.Priority));
        Console.WriteLine("Finish sync attributes");
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
        
        foreach (var method_priority in methods)
        {
            var method = method_priority.Item1;
            if(Filters.ContainsKey(method))
                foreach (var filter in Filters[method])
                    if (!filter.check(client, update))
                        goto Next;
            Console.WriteLine($"Run method {method.Method.Name}");
            try
            {
                await method(client, update);
            }
            catch (Exception error)
            {
                Console.WriteLine($"There is error while processing {method.Method.Name}");
                Console.WriteLine(error);
                // Console
                // (exception: error, message: $"There is error while processing {method.Method.Name}");
            }
            return;
            Next:
            continue;
        }
    }
}