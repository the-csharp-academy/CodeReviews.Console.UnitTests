using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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
}
