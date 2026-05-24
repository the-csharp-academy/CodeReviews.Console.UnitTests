using codingTracker._0lcm.Models;

namespace codingTracker._0lcm.Interfaces;

public interface ISqliteRepository
{
    public Task InsertCodingSessionAsync(CodingSession session);
    public Task DeleteCodingSessionAsync(CodingSession session);
    public Task UpdateCodingSessionAsync(CodingSession session);
    public Task<List<CodingSession>> GetSessionsAsync();
}