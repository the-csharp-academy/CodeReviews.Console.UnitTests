using codingTracker._0lcm.Data;
using codingTracker._0lcm.Interfaces;
using codingTracker._0lcm.Models;
using codingTracker._0lcm.Repositories;
using Microsoft.EntityFrameworkCore;

namespace codingTracker._0lcm.IntegrationTests;

public class SqliteRepositoryTests
{
    private AppDbContext _dbContext;
    private ISqliteRepository _sqliteRepo;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _sqliteRepo = new SqliteRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task InsertCodingSessionAsync_ShouldCreateNewCodingSession()
    {
        var codingSession = new CodingSession
        {
            StartTime = DateTime.Now,
            EndTime = DateTime.Now,
            Duration = TimeSpan.FromSeconds(1)
        };

        await _sqliteRepo.InsertCodingSessionAsync(codingSession);

        var sessions = await _sqliteRepo.GetSessionsAsync();
        Assert.That(sessions, Is.Not.Empty);
        Assert.That(sessions[0].StartTime, Is.EqualTo(codingSession.StartTime));
    }

    [Test]
    public async Task DeleteCodingSessionAsync_ShouldDeleteSession()
    {
        var codingSession = new CodingSession
        {
            StartTime = DateTime.Now,
            EndTime = DateTime.Now,
            Duration = TimeSpan.FromSeconds(1)
        };

        await _sqliteRepo.InsertCodingSessionAsync(codingSession);
        await _sqliteRepo.DeleteCodingSessionAsync(codingSession);

        var sessions = await _sqliteRepo.GetSessionsAsync();
        Assert.That(sessions, Is.Empty);
    }

    [Test]
    public async Task UpdateCodingSessionAsync_ShouldUpdateSession()
    {
        var originalDuration = TimeSpan.FromSeconds(1);

        var codingSession = new CodingSession
        {
            StartTime = DateTime.Now,
            EndTime = DateTime.Now,
            Duration = originalDuration
        };

        await _sqliteRepo.InsertCodingSessionAsync(codingSession);

        codingSession.Duration = TimeSpan.FromSeconds(2);

        await _sqliteRepo.UpdateCodingSessionAsync(codingSession);

        var sessions = await _sqliteRepo.GetSessionsAsync();

        Assert.That(sessions[0].Duration, Is.EqualTo(codingSession.Duration));
        Assert.That(sessions[0].Duration, Is.Not.EqualTo(originalDuration));
    }

    [Test]
    public async Task GetSessionsAsync_WhenDatabaseIsEmpty_ShouldReturnEmptyList()
    {
        var sessions = await _sqliteRepo.GetSessionsAsync();

        Assert.That(sessions, Is.Empty);
    }
}