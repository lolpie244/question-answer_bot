using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot;

[Role(RoleEnum.SuperAdmin)]
public class super_admin
{
    

    [Command("/remove_admin")]
    public async Task PromoteFromAdmin(ITelegramBotClient client, Update update)
    {
        
    }
}