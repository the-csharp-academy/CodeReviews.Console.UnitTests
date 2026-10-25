using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace silvermax.PhoneBook.DbAcess;

public class ContactDbContextFactory : IDesignTimeDbContextFactory<ContactDbContext>
{
    public ContactDbContext CreateDbContext(string[] args)
    {
        try
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection String not configured");

            var optionsBuilder = new DbContextOptionsBuilder<ContactDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new ContactDbContext(optionsBuilder);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create ContactDbContext: {ex}");
        }
        
    }
}
