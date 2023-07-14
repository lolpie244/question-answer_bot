using BotSettings;
using Telegram.Bot.Types.Enums;
using db_namespace;
using helping;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Types;
using MessageType = db_namespace.MessageType;

namespace Bot.QA;

public class CloseQuestion : IBotController
{
	private async Task<string> close_user_questions(ITelegramBotClient client, long asker_id)
	{
		var context = new dbContext();
		var chats = context.Chats.Where(obj => obj.Type == ChatEnum.Archive).ToArray();

		var asker = await context.Users.FindAsync(asker_id);
		var messages = context.Archive.Where(obj => obj.Type == MessageType.QA && obj.RelatedUserId == asker_id).ToArray();

		var keyboard = new Keyboards();
		string closed_at = DateTime.Now.ToString();

		foreach (var chat in chats) {
			await client.SendTextMessageAsync(
			    chat.Id, String.Format(TEXT.Get("chats.new_conversation_in_archive"), closed_at)
			);
			foreach (var message in messages)
				try {
					await client.CopyMessageAsync(chat.Id, message.ChatId, message.MessageId, replyMarkup: keyboard.History(message, asker));
				} catch (Exception) {
					await client.SendTextMessageAsync(
					    chat.Id, TEXT.Get("other.deleted_message"), replyMarkup: keyboard.History(message, asker)
					);
				}
		}

		foreach (var message in messages) {
			try {
				await client.DeleteMessageAsync(message.ChatId, message.MessageId);
			} catch {
				try {
					if (message.IsQuestion)
						await client.EditMessageTextAsync(message.ChatId, message.MessageId, TEXT.Get("qa.question_removed"));
				} catch { }
			}
		}
		context.Archive.RemoveRange(messages);
		await context.SaveChangesAsync();
		await client.SendTextMessageAsync(asker_id!, TEXT.Get("qa.question_closed_to_user"));
		return asker!.Name!;
	}

	[InlineButtonCallback("close_question"), Role(RoleEnum.Moderator, true)]
	public async Task moderator_close_question(ITelegramBotClient client, Update update)
	{
		var message = update.GetMessage()!;
		var context = new db_namespace.dbContext();
		var asker_id = context.Archive.First(
		                   obj => obj.Type == MessageType.QA &&
		                   obj.MessageId == message.MessageId &&
		                   obj.ChatId == message.Chat.Id
		               ).RelatedUserId;

		if (asker_id == null)
			return;
		var name = await close_user_questions(client, (long)asker_id);
		await client.AnswerCallbackQueryAsync(update.CallbackQuery!.Id, Strings.Format(TEXT.Get("qa.question_closed"), name));
	}

	[Command("/end"), Role(RoleEnum.User, true), Scope(ChatType.Private)]
	public async Task user_close_question(ITelegramBotClient client, Update update)
	{
		await close_user_questions(client, update.GetUser().Id!);
	}
}
