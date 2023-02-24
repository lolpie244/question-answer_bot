using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.UsersAndChats;

public class ChangeChatType : IBotController
{
	[BotStatusInGroup]
	public async Task BotStatusChanged(ITelegramBotClient client, Update update)
	{
		var status = update.MyChatMember!.NewChatMember.Status;

		using (var context = new dbContext()) {
			var chat = context.Chats.Find(update.GetChat()!.Id);
			if (chat != null)
				context.Chats.Remove(chat);
			await context.SaveChangesAsync();
		}

		if (status == ChatMemberStatus.Member)
			await client.SendTextMessageAsync(update.GetChat()!.Id, TEXT.Get("chats.promote_to_admin"));


		if (status == ChatMemberStatus.Administrator || status == ChatMemberStatus.Creator)
			await ChangeGroupType(client, update);
	}

	[Command("/change_group_type"), Scope(ChatType.Group, ChatType.Supergroup), Role(RoleEnum.Admin, true)]
	public async Task ChangeGroupType(ITelegramBotClient client, Update update)
	{
		await client.SendTextMessageAsync(
		    update.GetChat()!.Id, TEXT.Get("chats.select_type"), replyMarkup: new Keyboards(update).ChatType()
		);
	}

	[InlineButtonCallback("new_chat_type:type=.*"), Role(RoleEnum.Admin, true)]
	public async Task ChatStatusChanged(ITelegramBotClient client, Update update)
	{
		var data = helping.Helping.get_data_from_string(update.CallbackQuery!.Data!);
		var message = update.GetMessage()!;
		ChatEnum chat_type = (ChatEnum)int.Parse(data["type"]);
		string text = TEXT.Get($"chats.changing.{chat_type}");
		using (var context = new dbContext()) {
			context.Chats.Add(new db_namespace.Chat { Id = message.Chat.Id, Type = chat_type });
			await context.SaveChangesAsync();
		}
		await client.EditMessageTextAsync(message.Chat.Id, message.MessageId, text);
	}
}
