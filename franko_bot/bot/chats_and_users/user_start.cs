using BotSettings;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.UsersAndChats;

public class UserStart : IBotController
{
	[Scope(ChatType.Private), UpdateUser, Command("/start", "/help")]
	public async Task User(ITelegramBotClient client, Update update)
	{
		var role = update.GetDbUser()!.Role.ToString().ToLower();
		string text = TEXT.Get($"roles.help.{role}", "roles.help.default");
		await client.SendTextMessageAsync(update.GetChat()!.Id, text);
	}
}
