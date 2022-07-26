namespace settings;
using Microsoft.Extensions.Configuration;
// using System.Configuration; 
using System;

public class DbConfig
{
    public string host { get; set; }
    public string user { get; set; }
    public string database { get; set; }
    public string password { get; set; }
    public string port { get; set; }
}

public class BotConfig
{
    public string token { get; set; }
    public string webhook { get; set; }
}
