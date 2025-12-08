using System.Data;
using Microsoft.Data.Sqlite;
using CodingTracker.kilozdazolik.Data;
using CodingTracker.kilozdazolik.Models;
using Dapper;

namespace CodingTracker.kilozdazolik.Services;

public class TrackerService : ITrackerService
{
    private readonly string _connectionString;

    public TrackerService(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
    
    public void InsertSession(DateTime startDate, DateTime endDate)
    {
        try
        {
            using (var conn = CreateConnection())
            {
                var sql =
                    "INSERT INTO CodingTracker (StartTime, EndTime, Duration) VALUES (@StartTime, @EndTime, @Duration)";

                Tracker tracker = new()
                {
                    StartTime = startDate,
                    EndTime = endDate,
                };

                conn.Execute(sql, tracker);
            }
        }
        catch (SqliteException ex)
        {
            throw new Exception("Database operation failed", ex);
        }
    }
    
    public List<Tracker> GetAllSession()
    {
        try
        {
            using (var conn = CreateConnection())
            {
                var sql =
                    "SELECT * FROM CodingTracker";
                var sessions = conn.Query<Tracker>(sql).ToList();
                return sessions;
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Unexpected error happened: {ex.Message}");
            return new List<Tracker>();
        }
    }
    
    public void DeleteSession(Tracker tracker)
    {
        try
        {
            using (var conn = CreateConnection())
            {
                conn.Execute("DELETE FROM CodingTracker WHERE ID = @Id", tracker);
            }
        }
        catch (SqliteException ex)
        {
            throw new Exception("Database operation failed", ex);
        }
    }

    public void UpdateSession(Tracker tracker)
    {
        try
        {
            using (var conn = CreateConnection())
            {
                conn.Execute("UPDATE CodingTracker SET StartTime = @StartTime, EndTime = @EndTime WHERE ID = @Id", tracker);
            }
        }
        catch (SqliteException ex)
        {
            throw new Exception("Database operation failed", ex);
        }
    }
    
    // FILTER METHODS
    public List<Tracker> GetSessionsOrderByDate(bool ascending)
    {
        try
        {
            using (var conn = CreateConnection())
            {
                var orderDirection = ascending ? "ASC" : "DESC";
                var sql =
                    $"SELECT * FROM CodingTracker ORDER BY StartTime {orderDirection}";
                var sessions = conn.Query<Tracker>(sql).ToList();
                return sessions;
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Unexpected error happened: {ex.Message}");
            return new List<Tracker>();
        }
    }
}