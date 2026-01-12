using CodingTracker.Niasua.Configuration;
using CodingTracker.Niasua.Data;
using CodingTracker.Niasua.Models;
using Microsoft.Data.Sqlite;

namespace CodingTracker.Tests;

public class CodingControllerTests : IDisposable
{
    private readonly string _testConnectionString = "Data Source=CodingTrackerTest.db";

    public CodingControllerTests()
    {
        if (File.Exists("CodingTrackerTest.db"))
        {
            File.Delete("CodingTrackerTest.db");
        }

        AppConfig.ConnectionString = _testConnectionString;
        CodingController.InitializeDatabases();
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();

        if (File.Exists("CodingTrackerTest.db"))
        {
            File.Delete("CodingTrackerTest.db");
        }
    }

    [Fact]
    public void InsertSession_AddsSessionToDatabase()
    {
        // Arrange
        var session = new CodingSession
        {
            StartTime = DateTime.Now.AddHours(-2),
            EndTime = DateTime.Now,
        };

        // Act
        CodingController.InsertSession(session);

        // Assert
        var sessions = CodingController.GetAllSessions();

        Assert.Single(sessions);
        Assert.Equal(session.StartTime.ToString("yyyy-MM-dd HH:mm"), sessions[0].StartTime.ToString("yyyy-MM-dd HH:mm"));
    }

    [Fact]
    public void DeleteSession_RemovesSession_WhenSessionExists()
    {
        // Arrange
        var session = new CodingSession
        {
            StartTime = DateTime.Now.AddHours(-2),
            EndTime = DateTime.Now,
        };

        CodingController.InsertSession(session);

        var sessionSaved = CodingController.GetAllSessions().First();

        // Act
        CodingController.DeleteSession(sessionSaved);

        // Assert
        var sessions = CodingController.GetAllSessions();

        Assert.Empty(sessions);
    }

    [Fact]
    public void UpdateSession_WhenSessionExists()
    {
        // Arrange
        var session = new CodingSession
        {
            StartTime = DateTime.Now.AddHours(-2),
            EndTime = DateTime.Now,
        };

        CodingController.InsertSession(session);

        var sessionSaved = CodingController.GetAllSessions().First();

        var newSession = new CodingSession
        {
            Id = sessionSaved.Id,
            StartTime = DateTime.Now.AddMinutes(10),
            EndTime = DateTime.Now.AddMinutes(10)
        };

        // Act
        CodingController.UpdateSession(newSession);

        //Assert
        var updatedSessionFromDb = CodingController.GetAllSessions().First();

        Assert.Equal(newSession.Id, updatedSessionFromDb.Id);
        Assert.Equal(
            newSession.StartTime.ToString("yyyy-MM-dd HH:mm"),
            updatedSessionFromDb.StartTime.ToString("yyyy-MM-dd HH:mm")
        );
        Assert.Equal(
            newSession.EndTime.ToString("yyyy-MM-dd HH:mm"),
            updatedSessionFromDb.EndTime.ToString("yyyy-MM-dd HH:mm")
        );
    }

    [Fact]
    public void FilterSessionPerPeriod_ReturnsOnlySessionWithinPeriod()
    {
        // Arrange
        var filterStart = new DateTime(2023, 1, 1, 0, 0, 0);
        var filterEnd = new DateTime(2023, 1, 31, 23, 59, 59);

        var sessionInside1 = new CodingSession
        {
            StartTime = new DateTime(2023, 1, 2, 10, 0, 0),
            EndTime = new DateTime (2023, 1, 2, 12, 0, 0)
        };

        var sessionInside2 = new CodingSession
        {
            StartTime = new DateTime(2023, 1, 15, 14, 0, 0),
            EndTime = new DateTime(2023, 1, 15, 16, 0, 0)
        };

        var sessionOutside = new CodingSession
        {
            StartTime = new DateTime(2023, 2, 1, 10, 0, 0),
            EndTime = new DateTime(2023, 2, 1, 12, 0, 0)
        };

        CodingController.InsertSession(sessionInside1);
        CodingController.InsertSession(sessionInside2);
        CodingController.InsertSession(sessionOutside);

        // Act
        var filterParams = new CodingSession
        {
            StartTime = filterStart,
            EndTime = filterEnd
        };

        var result = CodingController.FilterSessionPerPeriod(filterParams, "ASC");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, s => s.StartTime.Month == 2);
    }
}
