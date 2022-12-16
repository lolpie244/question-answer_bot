using BotSettings;

namespace franko_bot;

public class LambdaEntryPoint :

    Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
{
    public LambdaEntryPoint(): base()
    {
        // new UpdateHandlerManagerSingelton().Get();
    }
    /// <summary>
    /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
    /// needs to be configured in this method using the UseStartup<>() method.
    /// </summary>
    /// <param name="builder"></param>
    protected override void Init(IWebHostBuilder builder)
    {
        // new UpdateHandlerManagerSingelton().Get();
        builder
            .UseStartup<Startup>();
    }
    
    /// <summary>
    /// Use this override to customize the services registered with the IHostBuilder. 
    /// 
    /// It is recommended not to call ConfigureWebHostDefaults to configure the IWebHostBuilder inside this method.
    /// Instead customize the IWebHostBuilder in the Init(IWebHostBuilder) overload.
    /// </summary>
    /// <param name="builder"></param>
    protected override void Init(IHostBuilder builder)
    {
        // new UpdateHandlerManagerSingelton().Get();
    }
}
