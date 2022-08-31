using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = db_namespace.User;

namespace Bot;

public class Keyboards
{
    private Update? update;

    public Keyboards(Update update=null)
    {
        this.update = update;
    }

    public InlineKeyboardMarkup ChatType()
    {
        return new(new[]
        {
            new[]{InlineKeyboardButton.WithCallbackData(TEXT.Get("answer_chat_button"), "answer_group")},
            new[]{InlineKeyboardButton.WithCallbackData(TEXT.Get("archive_chat_button"), "archive_group")},
            new[]{InlineKeyboardButton.WithCallbackData(TEXT.Get("report_chat_button"), "report_group")}
        });
    }

    public InlineKeyboardMarkup Ask()
    {
        var user = update!.GetUser();
        return new(new[]
        {
            new []{InlineKeyboardButton.WithUrl(string.Format(TEXT.Get("user_that_ask_button"), user.FullName()), 
                $"tg://user?id={user.Id}")},
            new []{InlineKeyboardButton.WithCallbackData(TEXT.Get("close_question_button"), "close_question") },
            new []{InlineKeyboardButton.WithCallbackData(TEXT.Get("ban_request_button"), "ban_request") }
        });
    }

    public InlineKeyboardMarkup Reporter(long user_id)
    {
        var context = new dbContext();
        var username = context.Users.Find(user_id).Name;
        return new(new[]
        {
            new[]{InlineKeyboardButton.WithUrl(string.Format(TEXT.Get("user_that_report_button"), username), 
                $"tg://user?id={user_id}")}
        });
    }

    public InlineKeyboardMarkup History(Archive message, User? user = null)
    {
        if (user == null)
        {
            var context = new dbContext();
            user = context.Users.Find(message.UserId);
        }

        Console.WriteLine(message.UserId);
        if(message.IsQuestion)
            return new(new[]
            {
                new[]{InlineKeyboardButton.WithUrl(string.Format(TEXT.Get("user_that_ask_button"), user.Name), 
                    $"tg://user?id={user.Id}")},
            });
        else
            return new(new[]
            {
                new[]{InlineKeyboardButton.WithUrl(string.Format(TEXT.Get("user_that_ans_button"), user.Name), 
                    $"tg://user?id={user.Id}")},
            });
    }

    public ReplyKeyboardMarkup ReplyEnd()
    {
        return new(new[] { new KeyboardButton(TEXT.Get("end_button")) }){ResizeKeyboard = true};
    }

    public InlineKeyboardMarkup Ban(User user, bool ban)
    {
        var user_that_report = update.GetUser();
        InlineKeyboardButton ban_button;
        string ban_str = ban ? "ban" : "unban";
        return new(new[]
        {
            new[]{InlineKeyboardButton.WithUrl(string.Format(TEXT.Get("user_that_report_button"), 
                    user_that_report.FullName()),$"tg://user?id={user_that_report.Id}")},
            new[]{InlineKeyboardButton.WithUrl(string.Format(TEXT.Get("reported_user_button"), 
                user.Name),$"tg://user?id={user.Id}")},
            new[]{InlineKeyboardButton.WithCallbackData(string.Format(TEXT.Get($"{ban_str}_button"), 
                user.Name),$"{ban_str}:user_id={user.Id}")},
        });
    }
}