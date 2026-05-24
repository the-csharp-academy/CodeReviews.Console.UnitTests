namespace codingTracker._0lcm.Data;

public static class DbConfig
{
    public static string GetConnectionString()
    {
        return $"Data Source={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodingTracker",
            "app.db")}";
    }
}