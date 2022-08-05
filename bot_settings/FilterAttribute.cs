using db_namespace;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotSettings;

public abstract class FilterAttribute: BaseCheckAttribute
{
    public override bool isFilter { get => true; }
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

public class StageAttribute : FilterAttribute
{
    private string[] Stages;

    public StageAttribute(params string[] stages)
    {
        Stages = stages;
    }

    public override bool check(Update update)
    {
        var stage = update.GetStage();
        return Stages.Length == 0 || Stages.Contains(stage);
    }   
}

public class RoleAttribute : FilterAttribute
{
    private db_namespace.RoleEnum[] Roles;

    public RoleAttribute(params db_namespace.RoleEnum[] roles)
    {
        Roles = roles;
    }

    public override bool check(Update update)
    {
        if (Roles.Length == 0)
            return true;
        
        var user_id = update.GetUser().Id;
        db_namespace.RoleEnum role;
        using (var context = new db_namespace.dbContext())
        {
            var user = context.Users.Find(user_id);
            if (user == null)
                return false;
            role = user.Role;
        }
        return Roles.Contains(role);
    }
}

public class UpdateUserAttribute : FilterAttribute
{
    public override bool check(Update update)
    {
        var user = update.GetUser();
        var full_name = user.FirstName + (user.LastName != "" ? $" {user.LastName}" : "");
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