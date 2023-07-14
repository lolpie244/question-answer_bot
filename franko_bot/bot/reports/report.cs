using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using MessageType = db_namespace.MessageType;

namespace Bot.Reports;

[Scope(ChatType.Private)]
public class Report : IBotController
{
	[InlineButtonCallback("report"), Command("/report")]
	public async Task WriteReport(ITelegramBotClient client, Update update)
	{
		update.SetStage("report");
		await client.SendTextMessageAsync(
		    update.GetChat()!.Id, TEXT.Get("reports.buttons.write"), replyMarkup: new Keyboards().ReplyEnd()
		);
	}
	[Text("Завершити"), Stage("report"), UpdateUser]
	public async Task EndReport(ITelegramBotClient client, Update update)
	{
		var user_id = update.GetUser()!.Id;
		update.SetStage(null);
		await client.SendTextMessageAsync(user_id, TEXT.Get("reports.thanks"), replyMarkup: new ReplyKeyboardRemove());

		var context = new dbContext();
		var chats = context.Chats.Where(obj => obj.Type == ChatEnum.Report).ToArray();

		var messages = context.Archive.Where(obj => obj.Type == MessageType.Report && obj.UserId == user_id).ToArray();
		var keyboard = new Keyboards().Reporter(update, user_id);
		foreach (var chat in chats) {
			await client.SendTextMessageAsync(chat.Id, TEXT.Get("reports.preview"), replyMarkup: keyboard);
			foreach (var message in messages)
				await client.CopyMessageAsync(chat.Id, message.ChatId, message.MessageId);
		}
		context.RemoveRange(messages);
		await context.SaveChangesAsync();
	}

	[Message, Stage("report")]
	public async Task NewReportMessage(ITelegramBotClient client, Update update)
	{
		var context = new dbContext();

		context.Archive.Add(new Archive(update.Message!) {
			Type = MessageType.Report
		});
		await context.SaveChangesAsync();
	}
}
