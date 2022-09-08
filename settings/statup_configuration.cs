using BotSettings;
using db_namespace;
using helping;
using Telegram.Bot;

namespace settings;

public static class SetupConfiguration
{
    public static void Services(IServiceCollection services, IConfiguration config)
    {
        dbContext.ConnectionString = Helping.get_connection_string(config);
        Task.Run(() =>
        {
            var context = new dbContext();
            var model = context.Users.FirstOrDefault();
        });
        
        services.AddControllers().AddNewtonsoftJson();
        
        
        services.AddHostedService<WebhookSetting>();
        services.AddScoped<BotService>().AddScoped<dbContext>().AddSingleton<UpdateHandlerManager>();
        services.AddHttpClient("tgwebhook")
            .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(config.Get<BotConfig>().token, httpClient));
    }

    public static void App(IApplicationBuilder app, IConfiguration config)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            var token = config.Get<BotConfig>().token;
            endpoints.MapControllerRoute(name: "tgwebhook",
                pattern: $"bot/{token}",
                new { controller = "BotWebhook", action = "Post" });
            endpoints.MapControllers();
            endpoints.MapGet("/",
                async context =>
                {
                    await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                });
        });
    }
}