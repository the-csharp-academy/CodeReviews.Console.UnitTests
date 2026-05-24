using codingTracker._0lcm.Data;
using codingTracker._0lcm.Interfaces;
using codingTracker._0lcm.Models;
using codingTracker._0lcm.Repositories;
using codingTracker._0lcm.Services;
using codingTracker._0lcm.User_Input;
using codingTracker._0lcm.User_Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", false, true);

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseSqlite(DbConfig.GetConnectionString()));

builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<ITimeValidationService, TimeValidationService>();
builder.Services.AddTransient<ISqliteRepository, SqliteRepository>();

builder.Services.AddTransient<IDateTimeFormats, DateTimeFormats>();
builder.Services.AddTransient<UserInputHelper>();

builder.Services.AddTransient<ConsoleUi>();

builder.Logging.SetMinimumLevel(LogLevel.Warning);

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var dbDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "CodingTracker");
    Directory.CreateDirectory(dbDirectory);

    await db.Database.MigrateAsync();
}

await app.RunAsync();

internal class Worker : BackgroundService
{
    private readonly ConsoleUi _consoleUi;

    public Worker(ConsoleUi consoleUi)
    {
        _consoleUi = consoleUi;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consoleUi.MainMenu();
    }
}

// {
//     internal class Program
//     {
//         internal static readonly IConfiguration configuration = new ConfigurationBuilder()
//             .SetBasePath(AppContext.BaseDirectory)
//             .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
//             .Build();
//
//         static async Task Main(string[] args)
//         {
//             var repo = new SqliteController();
//             repo.CreateDatabase();
//             await ConsoleUi.MainMenu();
//         }
//     }
// }