using codingTracker._0lcm.Models;
using Timer = codingTracker._0lcm.Models.Timer;

namespace codingTracker._0lcm.Interfaces;

public interface ISessionService
{
    public Task CreateSession(DateTime startTime, DateTime endTime, TimeSpan duration);
    public Task CreateTimerTask(Timer timer, CancellationToken cancellationToken);
    public Task SaveTimer(Timer timer);
    public Task<List<CodingSession>> GetFilteredSessions(DateOnly? filterDate = null, bool? ascending = true);
    public Task<bool> CheckForExistingSessionDate(DateOnly date);
}