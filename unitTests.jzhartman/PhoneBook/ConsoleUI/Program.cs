using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhoneBook.Application;
using PhoneBook.ConsoleUI;
using PhoneBook.ConsoleUI.Services;
using PhoneBook.Infrastructure;
using PhoneBook.Infrastructure.Database;

namespace PhoneBook.Presentation;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddPresentation();
                services.AddInfrastructure(context);
                services.AddApplication();
            })
            .Build();


        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PhoneBookDbContext>();
        db.Database.Migrate();

        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        await initializer.EnsureUncategorizedCategoryExistsAsync();

        var mainMenu = host.Services.GetRequiredService<MainMenuService>();
        await mainMenu.RunAsync();
    }
}
