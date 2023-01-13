using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace settings;

public class WebhookSetting: IHostedService
{
    private readonly ILogger<WebhookSetting> logger;
    private readonly IServiceProvider services;
    private readonly IConfiguration config;

    public WebhookSetting(ILogger<WebhookSetting> logger, IServiceProvider serviceProvider, IConfiguration new_config)
    {
        this.logger = logger;
        services = serviceProvider;
        config = new_config;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        var bot_config = config.Get<BotConfig>();

        var webhookAddress = $"{bot_config.webhook}/bot/{bot_config.token}";
        logger.LogInformation("Setting webhook: {WebhookAddress}", webhookAddress);
        await botClient.SetWebhookAsync(
            url: webhookAddress,
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        logger.LogInformation("Removing webhook");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}
