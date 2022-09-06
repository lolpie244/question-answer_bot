using BotSettings;
using db_namespace;
using helping;
using settings;
using Telegram.Bot;

namespace franko_bot;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        Console.WriteLine("IN ConfigureServices");
        services.AddControllers().AddNewtonsoftJson();
        dbContext.ConnectionString = Helping.get_connection_string(Configuration);
        services.AddHostedService<WebhookSetting>();
        services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
        services.AddScoped<BotService>().AddScoped<dbContext>().AddSingleton<UpdateHandlerManager>();
        services.AddHttpClient("tgwebhook")
            .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(Configuration.Get<BotConfig>().token, httpClient));

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        
        app.UseEndpoints(endpoints =>
        {
            var token = Configuration.Get<BotConfig>().token;
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