namespace codingTracker._0lcm.Interfaces;

public interface ITimeValidationService
{
    public bool TryValidateStartAndEndTimePair(string startTimeInput, string endTimeInput, out DateTime startTime,
        out DateTime endTime, out string? errorMessage);

    public bool TryValidateDateTime(string dateInput, out DateOnly date, out string? errorMessage);
}