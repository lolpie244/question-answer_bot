using Microsoft.EntityFrameworkCore;
using settings;
using helping;
using Telegram.Bot;
using BotSettings;
using db_namespace;

/// run ngrok: ngrok http 59890
var builder = WebApplication.CreateBuilder(args);
var botConfig = builder.Configuration.Get<BotConfig>();
var a =builder.Configuration;

// builder.Services.AddDbContext<db_namespace.dbContext>(options => options.UseNpgsql(Helping.get_connection_string(builder.Configuration)));

builder.Services.AddControllers().AddNewtonsoftJson();
dbContext.ConnectionString = Helping.get_connection_string(builder.Configuration);

builder.Services.AddHostedService<WebhookSetting>();
builder.Services.AddScoped<BotService>().AddScoped<dbContext>().AddSingleton<UpdateHandlerManager>();;
builder.Services.AddHttpClient("tgwebhook")
    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botConfig.token, httpClient));

var app = builder.Build();
app.UseRouting();
app.UseCors();

Console.WriteLine(builder.Configuration);

app.UseEndpoints(endpoints =>
{
    var token = botConfig.token;
    endpoints.MapControllerRoute(name: "tgwebhook",
        pattern: $"bot/{token}",
        new { controller = "BotWebhook", action = "Post" });
    endpoints.MapControllers();
});
app.Run();