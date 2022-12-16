using System.ComponentModel.DataAnnotations;
using System.Reflection;
using db_namespace;
using PostSharp.Aspects;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotSettings;
using BaseCheckDelegate = Func<ITelegramBotClient, Update, Task>;

// Base atribute for Handlers and Filters 
[Serializable]
public abstract class BaseCheckAttribute : Attribute
{
    virtual public bool isFilter { get; }
    [Range(0, 1000)] public int Priority { get; set; } = 100;

    public abstract bool check(ITelegramBotClient client, Update update);
}

// Interface, Which is implemented by classes with handlers
public interface IBotController
{

}
// Main data for using handlers
[Serializable]
public class HandlersData
{
    public Dictionary<BaseCheckAttribute, LinkedList<MethodInfo>> Handlers = new();
    public Dictionary<MethodInfo, LinkedList<BaseCheckAttribute>> Filters = new();
    public Dictionary<UpdateType, List<BaseCheckAttribute>> eventAttributes = new();
}

// Aspect, that fetch all methods with telegram handlers or filters.
// Because of aspect, it runs on compile time, so runtime perfomance will be good
[Serializable]
public class Aspect : LocationInterceptionAspect
{

    public MethodInfo[] methods;
    public Dictionary<BaseCheckAttribute, LinkedList<MethodInfo>> Handlers = new();
    public Dictionary<MethodInfo, LinkedList<BaseCheckAttribute>> Filters = new();
    public Dictionary<UpdateType, List<BaseCheckAttribute>> eventAttributes = new();
    public Aspect() : base()
    {
        methods = GetMethods();
        SetData();
        var context = new InMemoryContext();
        var model = context.Stages.FirstOrDefault();
    }
    public MethodInfo[] GetMethods()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes()).Where(x => x.GetInterfaces().Contains(typeof(IBotController)))
            .SelectMany(x => x.GetMethods())
            .Where(x => x.CustomAttributes.Count() != 0 || 
                        x.DeclaringType!.CustomAttributes.Count() != 0).ToArray();
    }
    
    public void SetData()
    {
        foreach (var method in methods)
        {
            var attributes = method.GetCustomAttributes().Concat(method.DeclaringType.GetCustomAttributes());
            foreach (var raw_attribute in attributes)
            {
                if (!(raw_attribute is BaseCheckAttribute attribute))
                    continue;
                
                if (!attribute.isFilter)
                {
                    if (!Handlers.ContainsKey(attribute))
                        Handlers[attribute] = new LinkedList<MethodInfo>();
                    Handlers[attribute].AddLast(method);
                    if (attribute is HandlerAttribute handler)
                    {
                        if(!eventAttributes.ContainsKey(handler.UpdateType))
                            eventAttributes[handler.UpdateType] = new List<BaseCheckAttribute>();
                        eventAttributes[handler.UpdateType].Add(attribute);
                    }
                }
                else
                {
                    if (!Filters.ContainsKey(method))
                        Filters[method] = new LinkedList<BaseCheckAttribute>();
                    Filters[method].AddLast(attribute);
                }
            }
        }
        
        foreach (var key in eventAttributes.Keys)
            eventAttributes[key].Sort((a, b) => a.Priority.CompareTo(b.Priority));
        Console.WriteLine("Finish sync attributes");

    }
    public override void OnGetValue(LocationInterceptionArgs args) 
    {
        args.Value = new HandlersData
        {
            Handlers = Handlers,
            Filters = Filters,
            eventAttributes = eventAttributes
        };
    }
    
}

public class UpdateHandlerManager
{
    public HandlersData handlersData { get; } = new HandlersData();
    [Aspect]
    public HandlersData originalHanldersData { get; }
    public Dictionary<MethodInfo, BaseCheckDelegate> delegates = new();

    public UpdateHandlerManager()
    {
        handlersData = originalHanldersData;
    }
    
    public async Task Run(ITelegramBotClient client, Update update)
    {
        if(!handlersData.eventAttributes.Keys.Contains(update.Type))
            return;
        foreach (var attribute in handlersData.eventAttributes[update.Type])
            if (attribute.check(client, update))
                foreach (var method in handlersData.Handlers[attribute])
                {
                    if(handlersData.Filters.ContainsKey(method))
                        foreach (var filter in handlersData.Filters[method])
                            if (!filter.check(client, update))
                                goto Next;
                    try
                    {
                        if(!delegates.ContainsKey(method))
                            try
                            {
                                delegates[method] = (BaseCheckDelegate)Delegate.CreateDelegate(
                                    typeof(BaseCheckDelegate), Activator.CreateInstance(method.DeclaringType), method);
                            }
                            catch
                            {
                                Console.WriteLine($"Method \"{method}\" has wrong signature");
                                continue;
                            }
                        Console.WriteLine($"Run method {method.Name}");
                        await delegates[method](client, update);
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine($"There is error while processing {method.Name}");
                        Console.WriteLine(error);
                    }
                    return;
                    Next:
                    continue;
                }
    
    }
}
