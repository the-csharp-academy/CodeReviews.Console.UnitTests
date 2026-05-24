namespace codingTracker._0lcm.Models;

public class CodingSession
{
    public CodingSession()
    {
    }

    public CodingSession(DateTime startTime, DateTime endTime, TimeSpan duration)
    {
        StartTime = startTime;
        EndTime = endTime;
        Duration = TimeSpan.FromSeconds(Math.Floor(duration.TotalSeconds));
        Date = DateOnly.FromDateTime(startTime);
    }

    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
}