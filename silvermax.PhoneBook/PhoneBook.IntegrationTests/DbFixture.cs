
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using silvermax.PhoneBook.DbAcess;

namespace PhoneBook.IntegrationTests;

public class DbFixture : IAsyncLifetime
{
    public ContactDbContext Db { get; set; } = null!;
    public async Task DisposeAsync()
    {
        await Db.Database.EnsureDeletedAsync();
        await Db.DisposeAsync();
;    }

    public async Task InitializeAsync()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var baseConnection = config.GetConnectionString("DefaultTestConnection");

        var builder = new SqlConnectionStringBuilder(baseConnection)
        {
            InitialCatalog = $"ContactsTestDb_{Guid.NewGuid():N}"
        };

        var options = new DbContextOptionsBuilder<ContactDbContext>()
            .UseSqlServer(builder.ConnectionString)
            .Options;

        Db = new ContactDbContext(options);
        await Db.Database.EnsureDeletedAsync();
        await Db.Database.MigrateAsync();
    }

    public async Task ResetAsync()
    {
        Db.ChangeTracker.Clear();
        await Db.Contacts.ExecuteDeleteAsync();
    }
}
