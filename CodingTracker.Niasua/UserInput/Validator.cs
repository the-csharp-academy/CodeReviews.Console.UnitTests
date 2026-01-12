using Spectre.Console;
using System.Globalization;

namespace CodingTracker.Niasua.UserInput;

public static class Validator
{
    public static bool IsValidDateTime(string input, string format, out DateTime date)
    {
        return DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    public static bool IsValidSessionTimes(DateTime start, DateTime end)
    {
        return end >= start;
    }

    public static bool IsValidInt(string input, out int id)
    {
        return int.TryParse(input, out id);
    }

    public static bool IsValidOrder(string input)
    {
        return input.ToUpper() == "ASC" || input.ToUpper() == "DESC";
    }
}
