using Microsoft.EntityFrameworkCore;
using settings;
using helping;
using Telegram.Bot;


var builder = WebApplication.CreateBuilder(args);
var botConfig = builder.Configuration.Get<BotConfig>();

builder.Services.AddDbContext<db_namespace.dbContext>(options => options.UseNpgsql(Helping.get_connection_string(builder.Configuration)));
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddHostedService<WebhookSetting>();
builder.Services.AddScoped<BotService>();
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