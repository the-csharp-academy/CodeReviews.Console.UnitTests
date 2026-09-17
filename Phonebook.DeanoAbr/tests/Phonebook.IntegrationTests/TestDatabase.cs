using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Phonebook.Data;

namespace Phonebook.IntegrationTests;

/// <summary>
/// A throwaway, file-backed SQLite database created fresh for every test.
/// It is the real SQLite engine (real SQL, real constraints, real commit boundary),
/// which is why the EF InMemory provider is not used anywhere in this project.
/// </summary>
public sealed class TestDatabase : IDisposable
{
    public string FilePath { get; }

    public TestDatabase()
    {
        FilePath = Path.Combine(Path.GetTempPath(), $"phonebook-tests-{Guid.NewGuid():N}.db");

        using var context = NewContext();
        context.Database.EnsureCreated();
    }

    public AppDbContext NewContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={FilePath}")
            .Options;

        return new AppDbContext(options);
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();   // release pooled handles before deleting the file
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }
}
