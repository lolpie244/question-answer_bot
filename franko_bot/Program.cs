using Microsoft.EntityFrameworkCore;
using settings;
using helping;
using Telegram.Bot;
using BotSettings;
using db_namespace;

/// RUN NGROK: ngrok http 59890
/// OPTIMIZE DB: 

var builder = WebApplication.CreateBuilder(args);

SetupConfiguration.Services(builder.Services, builder.Configuration);

var app = builder.Build();

SetupConfiguration.App(app, builder.Configuration);
app.Run();