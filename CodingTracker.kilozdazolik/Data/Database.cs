using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace CodingTracker.kilozdazolik.Data;

internal static class Database
{
    private static readonly IConfiguration _config;

    static Database()
    {
        _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();
    }
    
    public static string ConnectionString(string name)
    {
        return _config.GetConnectionString(name);
    }

    public static void CreateDatabase()
    {
        using (IDbConnection db = new SqliteConnection(ConnectionString("DefaultConnection")))
        {
            string createTable = @"
            CREATE TABLE IF NOT EXISTS CodingTracker(
            ID INTEGER PRIMARY KEY,
            StartTime TEXT NOT NULL,
            EndTime TEXT NOT NULL,
            Duration TEXT
        )";

            db.Execute(createTable);
        }
    }
    
    public static void InsertDummyData()
{
    using (IDbConnection db = new SqliteConnection(ConnectionString("DefaultConnection")))
    {
        int existingCount = db.QuerySingle<int>("SELECT COUNT(*) FROM CodingTracker");
        
        if (existingCount > 0)
        {
            return;
        }
        
        var random = new Random();
        var insertSql = @"
            INSERT INTO CodingTracker (StartTime, EndTime, Duration)
            VALUES (@StartTime, @EndTime, @Duration)";

        var dummyData = new List<object>();
        
        for (int i = 0; i < 50; i++)
        {
            var baseDate = DateTime.Now.AddMonths(-6).AddDays(random.Next(0, 180));
            
            var startTime = baseDate.Date.AddHours(random.Next(6, 23)).AddMinutes(random.Next(0, 60));
            
            var durationMinutes = random.Next(15, 480); // 15 perc - 8 óra
            var endTime = startTime.AddMinutes(durationMinutes);
            
            var duration = TimeSpan.FromMinutes(durationMinutes);
            var durationString = $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
            
            dummyData.Add(new
            {
                StartTime = startTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = endTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Duration = durationString
            });
        }
        
        db.Execute(insertSql, dummyData);
    }
}
    
}