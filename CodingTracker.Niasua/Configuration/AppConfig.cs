using Microsoft.Extensions.Configuration;

namespace CodingTracker.Niasua.Configuration;

public static class AppConfig
{
    private static IConfigurationRoot configuration;
    public static string ConnectionString { get; set; } // this property is accessible from the test project

    static AppConfig()
    {
        configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public static string GetConnectionString()
    {
        return ConnectionString;
    }
}
