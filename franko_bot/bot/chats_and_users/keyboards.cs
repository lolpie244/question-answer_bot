using BotSettings;
using helping;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using db_namespace;
using User = db_namespace.User;

namespace Bot.UsersAndChats;

public class Keyboards
{
	private Update? update;

	public Keyboards(Update? update = null)
	{
		this.update = update;
	}

	public InlineKeyboardMarkup ChatType()
	{
		var chat_types = new[] { ChatEnum.Answer, ChatEnum.Report, ChatEnum.Report };
		var buttons = new InlineKeyboardButton[chat_types.Length];
		for (int i = 0; i < chat_types.Length; i++) {
			var type = chat_types[i];
			buttons[i] = InlineKeyboardButton.WithCallbackData(TEXT.Get($"chats.buttons.{type.ToString().ToLower()}"), $"new_chat_type:type={(int)type}");
		}
		return new(buttons);
	}
	public InlineKeyboardMarkup Ban(User user, bool ban)
	{
		var user_that_report = update!.GetUser();
		string ban_str = ban ? "ban" : "unban";
		return new(new[] {
			new[]{
				InlineKeyboardButton.WithUrl(
				    string.Format(TEXT.Get("reports.buttons.user_that_report"), user_that_report.FullName()),
				    $"tg://user?id={user_that_report.Id}"
				)
			},
			new[]{
				InlineKeyboardButton.WithUrl(
				    string.Format(TEXT.Get("reports.buttons.reported_user"), user.Name),
				    $"tg://user?id={user.Id}"
				)
			},
			new[]{
				InlineKeyboardButton.WithCallbackData(
				    string.Format(TEXT.Get($"ban.buttons.{ban_str}"), user.Name),
				    $"{ban_str}:user_id={user.Id}"
				)
			},
		});
	}
}
