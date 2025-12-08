using CodingTracker.kilozdazolik.Models;

namespace CodingTracker.kilozdazolik.Services;

public interface ITrackerService
{
    void InsertSession(DateTime startDate, DateTime endDate);
    List<Tracker> GetAllSession();
    void DeleteSession(Tracker tracker);
    void UpdateSession(Tracker tracker);
    List<Tracker> GetSessionsOrderByDate(bool ascending);
}