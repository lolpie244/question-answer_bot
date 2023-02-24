using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.UsersAndChats;

public class StartInChat : IBotController
{
	[Scope(ChatType.Group, ChatType.Supergroup), Command("/start", "/help")]
	public async Task chat_start(ITelegramBotClient client, Update update)
	{
		var chat_id = update.GetChat()!.Id;
		var bot_status_in_chat = (await client.GetChatMemberAsync(chat_id, (await client.GetMeAsync()).Id)).Status;
		if (bot_status_in_chat != ChatMemberStatus.Administrator) {
			await client.SendTextMessageAsync(chat_id, TEXT.Get("other.promote_to_admin"));
			return;
		}

		var context = new db_namespace.dbContext();
		var chat = context.Chats.Find(chat_id);
		if (chat == null) {
			await new ChangeChatType().ChangeGroupType(client, update);
			return;
		}

		string text = TEXT.Get($"chats.help.{chat.Type.ToString().ToLower()}");
		await client.SendTextMessageAsync(chat_id, text);
	}
}
