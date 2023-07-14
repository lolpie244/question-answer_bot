using System.Text.Json;
using db_namespace;
using settings;
using Telegram.Bot.Types;

namespace helping;

public class Helping
{
	public static string get_connection_string(IConfiguration config)
	{
		var db_data = config.GetSection("db").Get<DbConfig>();
		return $"Host={db_data.host};Port={db_data.port};Database={db_data.database};" +
		       $"Username={db_data.user};Password={db_data.password}";
	}
	public static Dictionary<string, string> get_data_from_string(string input, char seperator = ';')
	{
		var result = new Dictionary<string, string>();

		input = input.Remove(0, input.IndexOf(":") + 1);
		var data = input.Split(seperator);
		foreach (var key_value in data) {
			int id_of_seperator = key_value.IndexOf('=');
			var key = key_value.Substring(0, id_of_seperator);
			var value = key_value.Substring(id_of_seperator + 1, key_value.Length - id_of_seperator - 1);
			result[key] = value;
		}
		return result;
	}

	public static db_namespace.User? get_user_from_message(Message message, dbContext? context = null)
	{
		if (context == null)
			context = new dbContext();
		db_namespace.User user;
		if (!message.From.IsBot)
			return context.Users.Find(message.From.Id);
		var db_message = context.Archive.First(obj => obj.MessageId == message.MessageId &&
		                                       obj.ChatId == message.Chat.Id);
		if (db_message != null)
			return context.Users.Find(db_message.UserId);
		return null;
	}
}

public class TEXT
{
	private static Dictionary<string, JsonElement>? Text;
	public static string Get(string name, string name_of_default = "")
	{
		if (Text == null) {
			string file_path = "text.json";
			using (StreamReader r = new StreamReader(file_path)) {
				string json = r.ReadToEnd();
				Text = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
			}
		}

		var names = name.Split(".");
		try {

			JsonElement path = Text[names[0]];
			for (int i = 1; i < names.Length; i++)
				path = path.GetProperty(names[i]);
			return path.ToString();

		} catch (KeyNotFoundException) {
			return Get(name_of_default);
		}
	}

}
