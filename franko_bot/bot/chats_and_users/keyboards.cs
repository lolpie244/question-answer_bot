using BotSettings;
using helping;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
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
		return new(new[] {
			new[]{InlineKeyboardButton.WithCallbackData(TEXT.Get("chats.buttons.answer"), "answer_group")},
			new[]{InlineKeyboardButton.WithCallbackData(TEXT.Get("chats.buttons.archive"), "archive_group")},
			new[]{InlineKeyboardButton.WithCallbackData(TEXT.Get("chats.buttons.report"), "report_group")}
		});
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
