using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot;

public class other
{
    [Command("/report")]
    public async Task Report(ITelegramBotClient client, Update update)
    {
        var message = update.Message;
        var text = message.Text.Substring(message.Text.IndexOf(" ") + 1);
        var context = new db_namespace.dbContext();
        var superuser = context.Users.Where(obj => obj.Role == RoleEnum.SuperAdmin);
        var keyboard = new Keyboards(update).Reporter(update.GetUser().Id);
        foreach (var user in superuser)
            await client.SendTextMessageAsync(user.Id, text, replyMarkup: keyboard);
        await client.SendTextMessageAsync(message.Chat.Id, TEXT.Get("report_thx"), replyToMessageId:message.MessageId);

    }
}