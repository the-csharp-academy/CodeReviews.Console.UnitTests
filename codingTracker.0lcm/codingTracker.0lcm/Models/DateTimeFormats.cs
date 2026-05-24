using codingTracker._0lcm.Interfaces;
using Microsoft.Extensions.Configuration;

namespace codingTracker._0lcm.Models;

public class DateTimeFormats(IConfiguration configuration) : IDateTimeFormats
{
    public string[] HourFormats { get; } =
        configuration.GetSection("TimeFormats:HourFormat").Get<string[]>()
        ?? ["H:mm", "HH:mm"];

    public string[] DateFormats { get; } =
        configuration.GetSection("TimeFormats:DateFormats").Get<string[]>()
        ?? ["yyyy-MM-dd", "yyyy-MM-d", "yyyy-M-dd", "yyyy-M-d"];

    public string DateIso { get; } =
        configuration["TimeFormats:DateIso"] ?? "yyyy-MM-dd";

    public string FullDateWithTimeFormat { get; } =
        configuration["TimeFormats:FullDateWithTimeFormat"] ?? "yyyy-MM-dd HH:mm:ss";
}