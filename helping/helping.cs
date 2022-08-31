using System.Text.Json;
using settings;

namespace helping;

public class Helping
{
    public static string get_connection_string(IConfiguration config)
    {
        var db_data = config.GetSection("db").Get<DbConfig>();
        return $"Host={db_data.host};Port={db_data.port};Database={db_data.database};" +
                                 $"Username={db_data.user};Password={db_data.password}";
    }
    public static Dictionary<string, string> get_data_from_string(string input, char seperator=';')
    {
        var result = new Dictionary<string, string>();

        input = input.Remove(0, input.IndexOf(":") + 1);
        var data = input.Split(seperator);
        foreach (var key_value in data)
        {
            int id_of_seperator = key_value.IndexOf('=');
            var key = key_value.Substring(0, id_of_seperator);
            var value = key_value.Substring(id_of_seperator + 1, key_value.Length - id_of_seperator - 1);
            result[key] = value;
        }
        return result;
    }
}

public class TEXT
{
    private static Dictionary<string, string>? Text;
    public static string Get(string name)
    {
        if (Text == null)
        {
            string path = "text.json";
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                Text = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
        }

        return Text[name];
    }

}