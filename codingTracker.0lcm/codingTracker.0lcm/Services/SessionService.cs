using codingTracker._0lcm.Interfaces;
using codingTracker._0lcm.Models;
using Timer = codingTracker._0lcm.Models.Timer;

namespace codingTracker._0lcm.Services;

public class SessionService(ISqliteRepository sqliteRepository) : ISessionService
{
    public async Task CreateSession(DateTime startTime, DateTime endTime, TimeSpan duration)
    {
        CodingSession session = new(
            startTime,
            endTime,
            duration
        );

        await sqliteRepository.InsertCodingSessionAsync(session);
    }

    public Task CreateTimerTask(Timer timer, CancellationToken cancellationToken)
    {
        var runTimer = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                timer.Stop();
            }
        });
        return runTimer;
    }

    public async Task SaveTimer(Timer timer)
    {
        CodingSession session = new(
            timer.StartTime,
            timer.EndTime,
            timer.EndTime - timer.StartTime
        );

        await sqliteRepository.InsertCodingSessionAsync(session);
    }

    public async Task<List<CodingSession>> GetFilteredSessions(DateOnly? filterDate = null, bool? ascending = true)
    {
        var sessions = await sqliteRepository.GetSessionsAsync();

        if (filterDate.HasValue) sessions = sessions.Where(s => s.Date == filterDate.Value).ToList();

        if (ascending != null)
            sessions = (bool)ascending
                ? sessions.OrderBy(s => s.Duration).ToList()
                : sessions.OrderByDescending(s => s.Duration).ToList();

        return sessions;
    }

    public async Task<bool> CheckForExistingSessionDate(DateOnly date)
    {
        var sessions = await sqliteRepository.GetSessionsAsync();
        foreach (var session in sessions)
            if (date == session.Date)
                return true;
        return false;
    }
}