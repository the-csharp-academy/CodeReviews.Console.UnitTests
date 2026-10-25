using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using silvermax.PhoneBook;
using silvermax.PhoneBook.DbAcess;
using silvermax.PhoneBook.Services;

public class Program
{
    private static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddUserSecrets<Program>(optional: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<ContactDbContext>(opts =>
                    opts.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
                services.AddTransient<UserInput>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ContactDbContext>();

        await DataSeeder.SeedAsync(db);
        Console.Clear();

        var configuration = host.Services.GetRequiredService<IConfiguration>();

        var input = scope.ServiceProvider.GetRequiredService<UserInput>();
        var contactService = new ContactService(db, input, CancellationToken.None);
        var mailService = new MailService(configuration, input, db, CancellationToken.None);
        var smsService = new SMSService(db, configuration, input, CancellationToken.None);

        UserInterface ui = new(contactService, mailService, smsService);
        await ui.Start();
    }
}
