using CodingTracker.Niasua.Configuration;
using CodingTracker.Niasua.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CodingTracker.Niasua.Data;

internal static class DatabaseSeeder
{
    public static void Seed()
    {
        using var connection = new SqliteConnection(AppConfig.GetConnectionString());

        connection.Open();

        var createSessionsTable = @"CREATE TABLE IF NOT EXISTS coding_sessions (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        StartTime TEXT NOT NULL,
                                        EndTime TEXT NOT NULL,
                                        DurationHours REAL NOT NULL
                                    );";

        var createGoalsTable = @"CREATE TABLE IF NOT EXISTS coding_goals (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        TargetHours REAL NOT NULL,
                                        Deadline TEXT NOT NULL
                                    );";

        connection.Execute(createSessionsTable);
        connection.Execute(createGoalsTable);

        var sessionsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM coding_sessions;");
        var goalsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM coding_goals;");

        if (sessionsCount == 0)
        {
            var sessions = new List<CodingSession>
            {
                new() { StartTime = DateTime.Now.AddDays(-2).AddHours(-2), EndTime = DateTime.Now.AddDays(-2), DurationHours = 2 },
                new() { StartTime = DateTime.Now.AddDays(-1).AddHours(-1), EndTime = DateTime.Now.AddDays(-1), DurationHours = 1 },
                new() { StartTime = DateTime.Now.AddHours(-3), EndTime = DateTime.Now, DurationHours = 3 }
            };

            foreach (var s in sessions)
            {
                var insertQuery = @"INSERT INTO coding_sessions (StartTime, EndTime, DurationHours)
                                    VALUES (@StartTime, @EndTime, @DurationHours);";

                var parameters = new
                {
                    StartTime = s.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    EndTime = s.EndTime.ToString("yyyy-MM-dd HH:mm"),
                    s.DurationHours
                };

                connection.Execute(insertQuery, parameters);
            }
        }

        if (goalsCount == 0)
        {
            var goalQuery = @"INSERT INTO coding_goals (TargetHours, Deadline)
                              VALUES (@TargetHours, @Deadline);";

            var goal = new
            {
                TargetHours = 20.0,
                Deadline = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd")
            };

            connection.Execute(goalQuery, goal);
        }
    }
}
