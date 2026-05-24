namespace codingTracker._0lcm.Models;

public class Timer
{
    public Timer()
    {
        StartTime = DateTime.Now;
    }

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public bool IsStopped { get; private set; }

    public void Stop()
    {
        EndTime = DateTime.Now;
        IsStopped = true;
    }
}