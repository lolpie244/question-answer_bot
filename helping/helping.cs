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
}