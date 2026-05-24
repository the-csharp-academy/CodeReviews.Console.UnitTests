using codingTracker._0lcm.Data;
using codingTracker._0lcm.Interfaces;
using codingTracker._0lcm.Models;
using Microsoft.EntityFrameworkCore;

namespace codingTracker._0lcm.Repositories;

public class SqliteRepository(AppDbContext db) : ISqliteRepository
{
    public async Task InsertCodingSessionAsync(CodingSession session)
    {
        await db.CodingSessions.AddAsync(session);
        await db.SaveChangesAsync();
    }

    public async Task DeleteCodingSessionAsync(CodingSession session)
    {
        db.CodingSessions.Remove(session);
        await db.SaveChangesAsync();
    }

    public async Task UpdateCodingSessionAsync(CodingSession session)
    {
        db.CodingSessions.Update(session);
        await db.SaveChangesAsync();
    }

    //------- Queries -------
    public Task<List<CodingSession>> GetSessionsAsync()
    {
        return db.CodingSessions.ToListAsync();
    }
}