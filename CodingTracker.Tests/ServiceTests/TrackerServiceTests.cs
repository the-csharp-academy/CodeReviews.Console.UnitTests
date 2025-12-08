using CodingTracker.kilozdazolik.Models;
using CodingTracker.kilozdazolik.Services;
using Dapper;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Data.Sqlite;

namespace CodingTracker.Tests.Services;

public class TrackerServiceTests : IDisposable
{
    private const string TestConnectionString = "Data Source=CodingTrackerTest;Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _keepAliveConnection;

    public TrackerServiceTests()
    {
        _keepAliveConnection = new SqliteConnection(TestConnectionString);
        _keepAliveConnection.Open();
        _keepAliveConnection.Execute(@"
            CREATE TABLE IF NOT EXISTS CodingTracker (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StartTime TEXT,
                EndTime TEXT,
                Duration TEXT
            )");
    }

    [Fact]
    public void InsertSession_StoresData_InMemory()
    {
        //Arrange
        var service = new TrackerService(TestConnectionString);
        
        var startTime = DateTime.Now;
        var endTime = startTime.AddHours(5);

        //Act
        service.InsertSession(startTime, endTime);

        //Assert
        var result = _keepAliveConnection.QuerySingle<Tracker>("SELECT * FROM CodingTracker");

        result.Should().NotBeNull();
        result.Duration.Should().NotBe(200.Milliseconds());
    }
    
    public void Dispose()
    {
        _keepAliveConnection.Close();
    }
}