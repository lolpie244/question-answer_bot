using settings;

namespace franko_bot;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		SetupConfiguration.Services(services, Configuration);
		services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		SetupConfiguration.App(app, Configuration);
	}
}
