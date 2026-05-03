using ResourceReservation.Core.DTOs;
using ResourceReservation.Core.Entities;
using ResourceReservation.Core.Interfaces;

namespace ResourceReservation.API.Services;

public class ReservationService
{
    private readonly IUnitOfWork _uow;

    public ReservationService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationDto dto)
    {
        // قانون ۱: حداکثر ۴ ساعت
        if ((dto.EndTime - dto.StartTime).TotalHours > 4)
            throw new InvalidOperationException("حداکثر مدت هر رزرو ۴ ساعت است.");

        if (dto.StartTime >= dto.EndTime)
            throw new InvalidOperationException("زمان شروع باید قبل از پایان باشد.");

        // قانون ۲: حداکثر ۲ رزرو فعال همزمان برای هر کاربر
        var activeCount = await _uow.Reservations.GetActiveReservationCountForUserAsync(dto.UserId);
        if (activeCount >= 2)
            throw new InvalidOperationException("هر کاربر حداکثر می‌تواند ۲ رزرو فعال داشته باشد.");

        // قانون ۳: بررسی تداخل زمانی
        var resource = await _uow.Resources.GetByIdAsync(dto.ResourceId)
            ?? throw new KeyNotFoundException("منبع مورد نظر یافت نشد.");

        var hasConflict = await _uow.Reservations
            .HasTimeConflictAsync(dto.ResourceId, dto.StartTime, dto.EndTime, resource.MaxConcurrentUsage);

        if (hasConflict)
            throw new InvalidOperationException("این بازه زمانی برای منبع مورد نظر رزرو شده است.");

        var reservation = new Reservation
        {
            ResourceId = dto.ResourceId,
            UserId = dto.UserId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = ReservationStatus.Active
        };

        await _uow.Reservations.AddAsync(reservation);
        await _uow.SaveChangesAsync();

        return new ReservationDto(
            reservation.Id,
            resource.Id,
            resource.Name,
            dto.UserId,
            "",
            reservation.StartTime,
            reservation.EndTime,
            reservation.Status.ToString()
        );
    }

    public async Task CancelAsync(int reservationId)
    {
        var reservation = await _uow.Reservations.GetByIdAsync(reservationId)
            ?? throw new KeyNotFoundException("رزرو مورد نظر یافت نشد.");

        // فقط رزروهایی که هنوز شروع نشده قابل لغو هستند
        if (reservation.StartTime <= DateTime.Now)
            throw new InvalidOperationException("رزروهایی که شروع شده‌اند قابل لغو نیستند.");

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("این رزرو قبلاً لغو شده است.");

        reservation.Status = ReservationStatus.Cancelled;
        await _uow.SaveChangesAsync();
    }

    public async Task<List<ResourceWithSlotsDto>> GetResourcesWithAvailableSlotsAsync()
    {
        var resources = await _uow.Resources.GetAllAsync();
        var result = new List<ResourceWithSlotsDto>();

        var workStart = DateTime.Today.AddHours(8);
        var workEnd = DateTime.Today.AddHours(18);

        foreach (var resource in resources)
        {
            var todayReservations = await _uow.Reservations
                .GetResourceReservationsTodayAsync(resource.Id);

            var slots = CalculateFreeSlots(todayReservations, workStart, workEnd);

            result.Add(new ResourceWithSlotsDto(
                resource.Id,
                resource.Name,
                resource.Type,
                slots
            ));
        }

        return result;
    }

    /// <summary>
    /// محاسبه بازه‌های خالی بین رزروها - الگوریتم Gap Detection
    /// </summary>
    private static List<TimeSlotDto> CalculateFreeSlots(
        List<Reservation> reservations, DateTime dayStart, DateTime dayEnd)
    {
        var slots = new List<TimeSlotDto>();
        var cursor = dayStart;

        // رزروها بر اساس StartTime مرتب شده‌اند
        foreach (var res in reservations.OrderBy(r => r.StartTime))
        {
            var start = res.StartTime < dayStart ? dayStart : res.StartTime;
            var end = res.EndTime > dayEnd ? dayEnd : res.EndTime;

            if (cursor < start)
                slots.Add(new TimeSlotDto(cursor, start));

            if (end > cursor)
                cursor = end;
        }

        if (cursor < dayEnd)
            slots.Add(new TimeSlotDto(cursor, dayEnd));

        return slots;
    }

    public async Task<List<ReservationDto>> GetReportAsync(int resourceId, DateTime from, DateTime to)
    {
        var reservations = await _uow.Reservations
            .GetByResourceAndDateRangeAsync(resourceId, from, to);

        return reservations.Select(r => new ReservationDto(
            r.Id,
            r.ResourceId,
            r.Resource?.Name ?? "",
            r.UserId,
            r.User?.Name ?? "",
            r.StartTime,
            r.EndTime,
            r.Status.ToString()
        )).ToList();
    }
}