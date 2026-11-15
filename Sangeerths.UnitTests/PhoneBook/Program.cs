using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using PhoneBook.Repository;
using PhoneBook.UI;

Env.Load();

using (var context = new PhoneBookContext())
{
    context.Database.Migrate();

    if (!context.Contacts.Any())
    {
        try
        {
            context.SeedDataFromExcel();
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unable to seed database: {ex.Message}");

            Console.WriteLine(
                "The application will continue without seed data.");

            Console.WriteLine(
                "Press any key to continue...");

            Console.ReadKey();
        }
    }
}

ConsoleMenu menu = new();
await menu.OnStart();