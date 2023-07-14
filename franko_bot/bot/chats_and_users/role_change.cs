using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.UsersAndChats;

public class role_change : IBotController
{
	public string? change_role(Update update, RoleEnum role)
	{
		var context = new dbContext();

		var message = update.GetMessage()!.ReplyToMessage!;
		var user = Helping.get_user_from_message(message, context);
		if (message == null || user == null)
			return null;
		var username = user.Name;

		var current_user = update.GetDbUser()!;
		if (current_user.Role <= user.Role || current_user.Role <= role || user.Role == role)
			return null;
		user.Role = role;
		context.SaveChanges();
		return username;
	}
	[Command("/promote_admin")]
	public async Task PromoteToAdmin(ITelegramBotClient client, Update update)
	{
		var username = change_role(update, RoleEnum.Admin);
		if (username != null)
			await client.SendTextMessageAsync(
			    update.GetChat()!.Id, String.Format(TEXT.Get("roles.changing.admin"), username)
			);

	}
	[Command("/promote_moderator")]
	public async Task PromoteToModer(ITelegramBotClient client, Update update)
	{
		var username = change_role(update, RoleEnum.Moderator);
		if (username != null)
			await client.SendTextMessageAsync(update.GetChat()!.Id, String.Format(TEXT.Get("roles.changing.moderator"), username));
	}
	[Command("/remove_admin", "/remove_moderator")]
	public async Task RemoveFromAdmin(ITelegramBotClient client, Update update)
	{
		var username = change_role(update, RoleEnum.User);
		if (username != null)
			await client.SendTextMessageAsync(update.GetChat()!.Id, String.Format(TEXT.Get("role.changing.user"), username));
	}
}
