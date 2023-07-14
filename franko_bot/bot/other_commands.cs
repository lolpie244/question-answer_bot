using BotSettings;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot;

public class OtherCommands : IBotController
{
	[Command("/student_council_list")]
	public async Task StudentCouncilList(ITelegramBotClient client, Update update)
	{
		await client.SendTextMessageAsync(update.GetChat()!.Id, TEXT.Get("other.student_council_list"));
	}
}

