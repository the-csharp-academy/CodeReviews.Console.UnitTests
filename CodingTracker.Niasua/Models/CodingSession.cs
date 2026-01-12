namespace CodingTracker.Niasua.Models;

public class CodingSession
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double DurationHours { get; set; }

    public void CalculateDuration()
    {
        DurationHours = (EndTime - StartTime).TotalHours;
    }
}
