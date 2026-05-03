using ResourceReservation.Core.Entities;

namespace ResourceReservation.Core.Interfaces;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(int id);
    Task<List<Reservation>> GetByResourceAndDateRangeAsync(int resourceId, DateTime from, DateTime to);
    Task<int> GetActiveReservationCountForUserAsync(int userId);
    Task<bool> HasTimeConflictAsync(int resourceId, DateTime start, DateTime end, int maxConcurrent);
    Task AddAsync(Reservation reservation);
    Task<List<Reservation>> GetResourceReservationsTodayAsync(int resourceId);
}