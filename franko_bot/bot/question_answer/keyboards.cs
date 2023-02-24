using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = db_namespace.User;

namespace Bot.QA;

public class Keyboards
{
	public InlineKeyboardMarkup Ask(Update update)
	{
		var user = update.GetUser();
		return new(new[] {
			new []{
				InlineKeyboardButton.WithUrl(
				    string.Format(TEXT.Get("qa.buttons.user_that_ask"), user.FullName()),
				    $"tg://user?id={user.Id}"
				)
			},
			new []{InlineKeyboardButton.WithCallbackData(TEXT.Get("qa.buttons.close_question"), "close_question") },
			new []{
				InlineKeyboardButton.WithCallbackData(
				    TEXT.Get("ban.buttons.ban_request"),
				    $"ban_request:user_id={user.Id}"
				)
			}
		});
	}


	public InlineKeyboardMarkup History(Archive message, User? user = null)
	{
		if (user == null) {
			var context = new dbContext();
			user = context.Users.Find(message.UserId);
		}

		if (message.IsQuestion)
			return new(new[] {
			new[]{
				InlineKeyboardButton.WithUrl(
				    string.Format(TEXT.Get("qa.buttons.user_that_ask"), user!.Name),
				    $"tg://user?id={user.Id}"
				)
			},
			new []{
				InlineKeyboardButton.WithCallbackData(
				    TEXT.Get("ban.buttons.ban_request"),
				    $"ban_request:user_id={user.Id}"
				)
			}
		});
		else
			return new(new[] {
			new[]{
				InlineKeyboardButton.WithUrl(
				    string.Format(TEXT.Get("qa.buttons.user_that_ans"), user!.Name),
				    $"tg://user?id={user.Id}"
				)
			},
			new []{
				InlineKeyboardButton.WithCallbackData(
				    TEXT.Get("ban.buttons.ban_request"),
				    $"ban_request:user_id={user.Id}"
				)
			}
		});
	}
}
