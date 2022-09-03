using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot;

public class role_change
{
    public db_namespace.User? change_role(Update update, RoleEnum role)
    {
        var message = update.GetMessage().ReplyToMessage;
        if (message == null)
            return null;
        var context = new dbContext();
        var user = Helping.get_user_from_message(message, context);
        var current_user = update.GetDbUser()!;
        Console.WriteLine($"Is user {user != null}, promote to {role}, current_user_role {current_user.Role}, condition return {current_user.Role <= user.Role || current_user.Role <= role || user.Role == role}");
        if(user == null || current_user.Role <= user.Role || current_user.Role <= role || user.Role == role)
            return null;
        user.Role = role;
        context.SaveChanges();
        return user;
    }
    [Command("/promote_admin")]
    public async Task PromoteToAdmin(ITelegramBotClient client, Update update)
    {
        var user = change_role(update, RoleEnum.Admin);
        if (user != null)
            await client.SendTextMessageAsync(update.GetChat()!.Id, 
                String.Format(TEXT.Get("user_is_admin"), user.Name));

    }
    [Command("/promote_moderator")]
    public async Task PromoteToModer(ITelegramBotClient client, Update update)
    {
        var user = change_role(update, RoleEnum.Moderator);
        if (user != null)
            await client.SendTextMessageAsync(update.GetChat()!.Id, String.Format(TEXT.Get("user_is_moderator")));
    }
    [Command("/remove_admin", "/remove_moderator")]
    public async Task RemoveFromAdmin(ITelegramBotClient client, Update update)
    {
        var user = change_role(update, RoleEnum.User);
        if (user != null)
            await client.SendTextMessageAsync(update.GetChat()!.Id, String.Format(TEXT.Get("user_is_user")));
    }
}