using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Phonebook.Services;

namespace Phonebook.IntegrationTests;

public class SqliteSchemaTests : IDisposable
{
    private readonly TestDatabase _db = new();

    public void Dispose() => _db.Dispose();

    [Fact]
    public void Database_WhenCreated_UsesSqliteProviderAndNotTheInMemoryProvider()
    {
        using var context = _db.NewContext();

        Assert.Equal("Microsoft.EntityFrameworkCore.Sqlite", context.Database.ProviderName);
        Assert.IsType<SqliteConnection>(context.Database.GetDbConnection());
    }

    [Fact]
    public async Task EnsureCreated_WhenRun_BuildsContactsTableWithExpectedColumns()
    {
        await using var connection = new SqliteConnection($"Data Source={_db.FilePath}");
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM pragma_table_info('Contacts')";

        var columns = new List<string>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            columns.Add(reader.GetString(0));

        Assert.Equal(
            new[] { "Category", "Email", "Id", "Name", "PhoneNumber" },
            columns.OrderBy(c => c, StringComparer.Ordinal).ToArray());
    }

    [Fact]
    public async Task SeedData_WhenSchemaIsCreated_InsertsTheThreeStarterContacts()
    {
        using var context = _db.NewContext();
        var service = new ContactService(context);

        var (success, data, error) = await service.GetAllAsync();

        Assert.True(success, error);
        Assert.NotNull(data);
        Assert.Equal(
            new[] { "Alice van der Merwe", "Bob Smith", "Carol Nkosi" },
            data!.Select(c => c.Name).ToArray());
    }

    [Fact]
    public async Task Category_WhenPersisted_IsStoredAsItsReadableNameNotItsNumericValue()
    {
        await using var connection = new SqliteConnection($"Data Source={_db.FilePath}");
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Category FROM Contacts WHERE Id = 1";

        var stored = (string?)await command.ExecuteScalarAsync();

        Assert.Equal("Family", stored);
    }
}
