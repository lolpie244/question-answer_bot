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
            new[]{InlineKeyboardButton.WithCallbackData(TEXT.Get("archive_chat_button"), "archive_group")}
        });
    }

    public InlineKeyboardMarkup Ask()
    {
        var user = update.GetUser();
        return new(new[]
        {
            new []{InlineKeyboardButton.WithUrl(string.Format(TEXT.Get("user_that_ask_button"), user.FullName()), 
                $"tg://user?id={user.Id}")},
            new []{InlineKeyboardButton.WithCallbackData(TEXT.Get("close_question_button"), "close_question") }
        });
    }

    public InlineKeyboardMarkup Reporter(long user_id)
    {
        var context = new db_namespace.dbContext();
        var username = context.Users.Find(user_id).Name;
        return new(new[]
        {
            new[]{InlineKeyboardButton.WithUrl(string.Format(TEXT.Get("user_that_report_button"), username), 
                $"tg://user?id={user_id}")},
            new[]{InlineKeyboardButton.WithCallbackData(TEXT.Get("close_report_button"), "close_report")},
        });
    }

    public InlineKeyboardMarkup History(Archive message)
    {
        var context = new dbContext();
        var user = context.Users.Find(message.UserId);
        if(message.IsQuestion)
            return new(new[]
            {
                new[]{InlineKeyboardButton.WithUrl(string.Format(TEXT.Get("user_that_ask_button"), user.Name), 
                    $"tg://user?id={user.Id}")}
            });
        else
            return new(new[]
            {
                new[]{InlineKeyboardButton.WithUrl(string.Format(TEXT.Get("user_that_ans_button"), user.Name), 
                    $"tg://user?id={user.Id}")}
            });
    }
}