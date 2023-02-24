using BotSettings;
using db_namespace;
using Telegram.Bot;
using Telegram.Bot.Types;
using helping;

namespace Bot.UsersAndChats;

public class BanUser : IBotController
{
	[Role(RoleEnum.Admin, true), InlineButtonCallback("ban:user_id=.*", "unban:user_id=.*")]
	public async Task BanUserButton(ITelegramBotClient client, Update update)
	{
		var context = new dbContext();
		var user_id = long.Parse(helping.Helping.get_data_from_string(update.CallbackQuery!.Data!)["user_id"]);
		var user = context.Users.Find(user_id)!;
		var current_user = update.GetDbUser()!;
		if (user.Role >= current_user.Role) {
			await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, TEXT.Get("roles.no_permission"));
			return;
		}

		var ban = user.Role != RoleEnum.Banned;
		if (ban)
			user.Role = RoleEnum.Banned;
		else
			user.Role = RoleEnum.User;
		await context.SaveChangesAsync();

		var ban_str = ban ? "ban" : "unban";
		var keyboard = new Keyboards(update).Ban(user, !ban);
		var original_message = update.GetMessage()!;
		await client.EditMessageReplyMarkupAsync(original_message.Chat.Id, original_message.MessageId, keyboard);
		await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, string.Format(TEXT.Get($"ban.{ban_str}"), user.Name));
		await client.SendTextMessageAsync(user.Id, TEXT.Get($"ban.{ban_str}_to_user"));
	}

	[Role(RoleEnum.Moderator, true), InlineButtonCallback("ban_request:user_id=.*")]
	public async Task BanRequest(ITelegramBotClient client, Update update)
	{
		var context = new dbContext();
		var original_message = update.GetMessage()!;
		var user_id = long.Parse(Helping.get_data_from_string(update.CallbackQuery!.Data!)["user_id"]);
		var user = await context.Users.FindAsync(user_id);
		if (user!.Role == RoleEnum.Banned) {
			await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, TEXT.Get("ban.user_already_banned"));
			return;
		}
		await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, TEXT.Get("ban.request_sent"));

		var chats = context.Chats.Where(obj => obj.Type == ChatEnum.Report).ToArray();
		var keyboard = new Keyboards(update).Ban(user, true);
		foreach (var chat in chats) {
			await client.SendTextMessageAsync(chat.Id, TEXT.Get("ban.request_preview"));
			await client.CopyMessageAsync(chat.Id, original_message.Chat.Id, original_message.MessageId, replyMarkup: keyboard);
		}
	}
}
