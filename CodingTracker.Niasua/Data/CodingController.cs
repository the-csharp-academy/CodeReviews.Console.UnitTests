using CodingTracker.Niasua.Configuration;
using CodingTracker.Niasua.Models;
using CodingTracker.Niasua.UserInput;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.ComponentModel.DataAnnotations;


namespace CodingTracker.Niasua.Data;

public static class CodingController
{
    public static void InitializeDatabases()
    {
        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        var tableQuery = @"CREATE TABLE IF NOT EXISTS coding_sessions (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            StartTime TEXT NOT NULL,
                            EndTime TEXT NOT NULL,
                            DurationHours REAL NOT NULL
                        );";

        var goalTable = @"
                        CREATE TABLE IF NOT EXISTS coding_goals (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            TargetHours REAL NOT NULL,
                            Deadline TEXT NOT NULL
                        );
        ";

        connection.Execute(tableQuery);
        connection.Execute(goalTable);
    }

    public static List<CodingSession> GetAllSessions()
    {
        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        var query = "SELECT * FROM coding_sessions";

        var sessions = connection.Query<CodingSession>(query).ToList();

        return sessions;
    }

    public static void InsertSession(CodingSession session)
    {
        session.CalculateDuration();

        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        var insertQuery = @"INSERT INTO coding_sessions (StartTime, EndTime, DurationHours)
                            VALUES (@StartTime, @EndTime, @DurationHours);";

        var parameters = new
        {
            StartTime = session.StartTime.ToString("yyyy-MM-dd HH:mm"),
            EndTime = session.EndTime.ToString("yyyy-MM-dd HH:mm"),
            session.DurationHours
        };

        connection.Execute(insertQuery, parameters);
    }

    public static void UpdateSession(CodingSession session)
    {
        session.CalculateDuration();
        
        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        var updateQuery = @"UPDATE coding_sessions
                            SET 
                                StartTime = @StartTime,
                                EndTime = @EndTime,
                                DurationHours = @DurationHours
                            WHERE Id = @Id;";


        var parameters = new
        {
            StartTime = session.StartTime.ToString("yyyy-MM-dd HH:mm"),
            EndTime = session.EndTime.ToString("yyyy-MM-dd HH:mm"),
            session.DurationHours,
            session.Id
        };

        connection.Execute(updateQuery, parameters);
    }

    public static void DeleteSession(CodingSession session)
    {
        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        var deleteQuery = @"DELETE FROM coding_sessions
                            WHERE Id = @Id;";

        connection.Execute(deleteQuery, session);
    }

    public static void DeleteAll()
    {
        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        var query = @"
                        DELETE FROM coding_sessions  
        ";

        connection.Execute(query);
    }

    public static CodingSession GetSessionById(int id)
    {
        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        var query = @"SELECT * FROM coding_sessions
                      WHERE Id = @Id;";

        var session = connection.QuerySingleOrDefault<CodingSession>(query, new {Id = id});

        return session;
    }

    public static List<CodingSession> FilterSessionPerPeriod(CodingSession session, string order)
    {
        if (!UserInput.Validator.IsValidOrder(order))
        {
            throw new ArgumentException("Invalid order parameter. Must be 'ASC' or 'DESC'");
        }

        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        var query = $@"
                    SELECT * FROM coding_sessions
                    WHERE StartTime >= @StartTime AND EndTime <= @EndTime
                    ORDER BY StartTime {order}";

        var parameters = new
        {
            StartTime = session.StartTime.ToString("yyyy-MM-dd HH:mm"),
            EndTime = session.EndTime.ToString("yyyy-MM-dd HH:mm")
        };

        return connection.Query<CodingSession>(query, parameters).ToList();
    }

    public static List<CodingSummary> ShowSummaryReport(string period)
    {
        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        string groupBy;
        string label;

        switch (period.ToLower())
        {
            case "daily":
                groupBy = "strftime('%Y-%m-%d', StartTime)";
                label = "Date";
                break;
            case "weekly":
                groupBy = "strftime('%Y-W%W', StartTime)";
                label = "Week";
                break;
            case "monthly":
                groupBy = "strftime('%Y-%m', StartTime)";
                label = "Month";
                break;
            case "yearly":
                groupBy = "strftime('%Y', StartTime)";
                label = "Year";
                break;
            default:
                throw new ArgumentException("Invalid period. Choose: daily, weekly, monthly or yearly.");
        }

        var query = $@"
            SELECT
                {groupBy} AS Period,
                COUNT(*) AS Sessions,
                ROUND(AVG(DurationHours), 2)AS AvgDuration
            FROM coding_sessions
            GROUP BY Period
            ORDER BY Period ASC;
        ";

        var results = connection.Query<CodingSummary>(query).ToList();

        return results;
    }

    public static void InsertGoal(CodingGoal goal)
    {
        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        var insertQuery = @"INSERT INTO coding_goals (TargetHours, Deadline)
                            VALUES (@TargetHours, @Deadline);";

        var parameters = new
        {
            goal.TargetHours,
            Deadline = goal.Deadline.ToString("yyyy-MM-dd HH:mm")
        };

        connection.Execute(insertQuery, parameters);
    }

    public static void ShowGoalProgress()
    {
        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        var goal = connection.QueryFirstOrDefault<CodingGoal>("SELECT * FROM coding_goals ORDER BY Id DESC LIMIT 1");

        if (goal == null)
        {
            Console.WriteLine("No coding goal found.");
            return;
        }

        var totalHours = connection.ExecuteScalar<double>(
            "SELECT SUM(DurationHours) FROM coding_sessions WHERE StartTime <= @Deadline",
            new { Deadline = goal.Deadline.ToString("yyyy-MM-dd HH-mm") });

        var remaining = goal.TargetHours - totalHours;
        var daysLeft = (goal.Deadline - DateTime.Now).TotalDays;
        var hoursPerDay = daysLeft > 0 ? remaining / daysLeft : remaining;

        Console.WriteLine($"\nGoal: {goal.TargetHours} hrs by {goal.Deadline:dd-MM-yyyy}");
        Console.WriteLine($"You’ve coded: {totalHours:0.##} hrs");
        Console.WriteLine($"Remaining: {remaining:0.##} hrs");

        if (remaining <= 0)
            Console.WriteLine("Goal reached!");
        else if (daysLeft > 0)
            Console.WriteLine($"You need to code ~{hoursPerDay:0.##} hrs/day to reach your goal.");
        else
            Console.WriteLine("Deadline passed!");
    }
}
