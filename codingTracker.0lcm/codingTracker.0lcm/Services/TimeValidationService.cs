using System.Globalization;
using codingTracker._0lcm.Interfaces;

namespace codingTracker._0lcm.Services;

public class TimeValidationService(IDateTimeFormats dateTimeFormats) : ITimeValidationService
{
    private readonly string[] _dateFormats = dateTimeFormats.DateFormats;
    private readonly string[] _hourFormats = dateTimeFormats.HourFormats;

    /// <summary>
    ///     Takes a startTime string and endTime string and attempts to parse them by 'H:mm' and 'HH:mm'. Also checks that
    ///     endTime
    ///     cannot be before starTime. outputs the validated startTime and endTime, as well as an error message if any
    ///     validation
    ///     check fails.
    /// </summary>
    public bool TryValidateStartAndEndTimePair(
        string startTimeInput,
        string endTimeInput,
        out DateTime startTime,
        out DateTime endTime,
        out string? errorMessage)
    {
        startTime = default;
        endTime = default;
        errorMessage = null;

        var isValidStartTime = DateTime.TryParseExact(startTimeInput.Trim(),
            _hourFormats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var parsedStartTime);
        var isValidEndTime = DateTime.TryParseExact(endTimeInput.Trim(),
            _hourFormats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var parsedEndTime);

        if (!isValidStartTime || !isValidEndTime)
        {
            errorMessage = "Invalid Time Format. Please Use HH:mm (e.g 09:30 or 9:30)";
            return false;
        }

        startTime = DateTime.Today + parsedStartTime.TimeOfDay;
        endTime = DateTime.Today + parsedEndTime.TimeOfDay;

        if (endTime <= startTime) endTime = endTime.AddDays(1);

        return true;
    }

    /// <summary>
    ///     Trys to Parse a string to a DateTime following Iso yyyy-MM-dd to be converted to DateOnly and outputted.
    /// </summary>
    public bool TryValidateDateTime(string dateInput, out DateOnly date, out string? errorMessage)
    {
        date = default;
        errorMessage = null;

        var isValidDate = DateTime.TryParseExact
        (dateInput.Trim(), _dateFormats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var parsedDate);

        if (!isValidDate)
        {
            errorMessage = "Invalid Time Format. Please Use yyyy-MM-dd";
            return false;
        }

        date = DateOnly.FromDateTime(parsedDate);
        return true;
    }
}