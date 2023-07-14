using Telegram.Bot.Types;
using db_namespace;
using helping;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Reports;

public class Keyboards
{
	public InlineKeyboardMarkup Reporter(Update update, long user_id)
	{
		var context = new dbContext();
		var username = context.Users.Find(user_id)!.Name;
		return new(new[] {
			new[]{
				InlineKeyboardButton.WithUrl(
				    string.Format(TEXT.Get("reports.buttons.user_that_report"), username),
				    $"tg://user?id={user_id}"
				)
			}
		});
	}
	public ReplyKeyboardMarkup ReplyEnd()
	{
		return new(new[] { new KeyboardButton(TEXT.Get("other.buttons.end")) }) {
			ResizeKeyboard = true
		};
	}

}
