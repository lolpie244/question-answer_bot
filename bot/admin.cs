using System.ComponentModel;
using BotSettings;
using db_namespace;
using Telegram.Bot;
using Telegram.Bot.Types;
using db_namespace;
using helping;

namespace Bot;

[Role(RoleEnum.Admin, true)]
public class admin
{
    [InlineButtonCallback("ban:user_id=.*", "unban:user_id=.*")]
    public async Task BanUserButton(ITelegramBotClient client, Update update)
    {
        var context = new dbContext();
        var user_id = long.Parse(helping.Helping.get_data_from_string(update.CallbackQuery.Data)["user_id"]);
        var user = context.Users.Find(user_id);
        var current_user = context.Users.Find(update.GetUser().Id);
        if (user.Role >= current_user.Role)
        {
            await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, TEXT.Get("no_permissions"));
            return;
        }

        var ban = user.Role != RoleEnum.Banned;
        if (ban)
            user.Role = RoleEnum.Banned;
        else
            user.Role = RoleEnum.User;
        await context.SaveChangesAsync();
        
        var ban_str = ban ? "ban" : "unban";
        var keyboard = new Keyboards(update).Ban(user, !ban);
        var original_message = update.GetMessage();
        await client.EditMessageReplyMarkupAsync(original_message.Chat.Id, original_message.MessageId, keyboard);
        await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id, string.Format(TEXT.Get($"{ban_str}_text"), user.Name));
        await client.SendTextMessageAsync(user.Id, TEXT.Get($"{ban_str}_to_user_text"));
    }
}