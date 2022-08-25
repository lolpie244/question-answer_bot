using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MessageType = db_namespace.MessageType;

namespace Bot;

[Scope(ChatType.Private)]
public class Report
{
    [InlineButtonCallback("report")]
    public async Task WriteReport(ITelegramBotClient client, Update update)
    {
        update.SetStage("report");
        await client.SendTextMessageAsync(update.GetChat().Id, TEXT.Get("write_report"), replyMarkup:
            new Keyboards().ReplyEnd());
    }
    [Text("Завершити"), Stage("report"), UpdateUser]
    public async Task EndReport(TelegramBotClient client, Update update)
    {
        var user_id = update.GetUser()!.Id;
        update.SetStage(null);

        await client.SendTextMessageAsync(user_id, TEXT.Get("report_thx"));

        var context = new dbContext();
        var superuser = context.Users.Where(obj => obj.Role == RoleEnum.SuperAdmin);
        
        var messages = context.Archive.Where(obj => obj.Type == MessageType.Report && obj.UserId == user_id);
        var keyboard = new Keyboards(update).Reporter(user_id);
        foreach (var user in superuser)
        {
            await client.SendTextMessageAsync(user.Id, TEXT.Get("new_report_created"), replyMarkup: keyboard);
            foreach (var message in messages)
                await client.CopyMessageAsync(user_id, message.ChatId, message.MessageId);
        }
        context.RemoveRange(messages);
        await context.SaveChangesAsync();
    }

    [Message, Stage("report")]
    public async Task NewReportMessage(ITelegramBotClient client, Update update)
    {
        var context = new dbContext();

        context.Archive.Add(new Archive(update.Message!)
        {
            Type = MessageType.Report
        });
        await context.SaveChangesAsync();
    }
}