using Microsoft.EntityFrameworkCore;
using ResourceReservation.Core.Entities;
using ResourceReservation.Core.Interfaces;
using ResourceReservation.Infrastructure.Data;

namespace ResourceReservation.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation?> GetByIdAsync(int id)
        => await _context.Reservations
            .Include(r => r.Resource)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<Reservation>> GetByResourceAndDateRangeAsync(
        int resourceId, DateTime from, DateTime to)
        => await _context.Reservations
            .Include(r => r.User)
            .Where(r => r.ResourceId == resourceId
                     && r.Status == ReservationStatus.Active
                     && r.StartTime < to
                     && r.EndTime > from)
            .OrderBy(r => r.StartTime)
            .ToListAsync();

    public async Task<int> GetActiveReservationCountForUserAsync(int userId)
        => await _context.Reservations
            .CountAsync(r => r.UserId == userId
                          && r.Status == ReservationStatus.Active
                          && r.StartTime > DateTime.Now);

    /// <summary>
    /// بررسی تداخل زمانی - اگر تعداد رزروهای همزمان به MaxConcurrentUsage رسیده باشد تداخل داریم
    /// </summary>
    public async Task<bool> HasTimeConflictAsync(
        int resourceId, DateTime start, DateTime end, int maxConcurrent)
    {
        var overlappingCount = await _context.Reservations
            .CountAsync(r => r.ResourceId == resourceId
                          && r.Status == ReservationStatus.Active
                          && r.StartTime < end
                          && r.EndTime > start);

        return overlappingCount >= maxConcurrent;
    }

    public async Task AddAsync(Reservation reservation)
        => await _context.Reservations.AddAsync(reservation);

    public async Task<List<Reservation>> GetResourceReservationsTodayAsync(int resourceId)
    {
        var todayStart = DateTime.Today;
        var todayEnd = todayStart.AddDays(1);

        return await _context.Reservations
            .Where(r => r.ResourceId == resourceId
                     && r.Status == ReservationStatus.Active
                     && r.StartTime < todayEnd
                     && r.EndTime > todayStart)
            .OrderBy(r => r.StartTime)
            .ToListAsync();
    }
}