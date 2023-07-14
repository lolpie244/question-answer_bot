using db_namespace;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotSettings;
[Serializable]
public abstract class FilterAttribute: BaseCheckAttribute
{
    public override bool isFilter { get => true; }
}
[Serializable]

public class ScopeAttribute : FilterAttribute
{
    private ChatType[] Scopes;

    public ScopeAttribute(params ChatType[] scopes)
    {
        Scopes = scopes;
    }

    public override bool check(ITelegramBotClient client, Update update)
    {
        var chat = update.GetChat();
        if(chat != null)
            return Scopes.Contains(chat.Type);
        return true;
    }
}
[Serializable]

public class StageAttribute : FilterAttribute
{
    private string[] Stages;

    public StageAttribute(params string[] stages)
    {
        Stages = stages;
    }

    public override bool check(ITelegramBotClient client, Update update)
    {
        var stage = update.GetStage();
        return Stages.Length == 0 || Stages.Contains(stage);
    }   
}
[Serializable]

public class RoleAttribute : FilterAttribute
{
    private db_namespace.RoleEnum[] Roles { get; set; }
    private bool Higher { get; set; }

    public RoleAttribute(params db_namespace.RoleEnum[] roles)
    {
        Roles = roles;
    }

    public RoleAttribute(RoleEnum role, bool higher)
    {
        Roles = new[] { role };
        Higher = higher;
    }
    public override bool check(ITelegramBotClient client, Update update)
    {
        if (Roles.Length == 0)
            return true;
        
        var user_id = update.GetUser().Id;
        db_namespace.RoleEnum role;
        using (var context = new db_namespace.dbContext())
        {
            var user = context.Users.FirstOrDefault(obj => obj.Id == user_id);
            if (user == null)
                return false;
            role = user.Role;
        }
        if (Higher)
            return Roles[0] <= role;
        return Roles.Contains(role);
    }
}
[Serializable]

public class UpdateUserAttribute : FilterAttribute
{
    public override bool check(ITelegramBotClient client, Update update)
    {
        var user = update.GetUser();
        var full_name = user.FullName();
        using (var context = new db_namespace.dbContext())
        {
            var user_from_db = context.Users.Find(user.Id);
            if (user_from_db == null)
                context.Users.Add(new db_namespace.User { Id = user.Id, Name = full_name});
            else
                user_from_db.Name = full_name;
            context.SaveChanges();
        }

        return true;
    }
    
}
