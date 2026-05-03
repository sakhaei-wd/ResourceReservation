namespace ResourceReservation.Core.DTOs;

public record CreateReservationDto(
    int ResourceId,
    int UserId,
    DateTime StartTime,
    DateTime EndTime
);

public record ReservationDto(
    int Id,
    int ResourceId,
    string ResourceName,
    int UserId,
    string UserName,
    DateTime StartTime,
    DateTime EndTime,
    string Status
);

public record CancelReservationDto(int ReservationId);

public record TimeSlotDto(DateTime StartTime, DateTime EndTime);

public record ResourceWithSlotsDto(
    int ResourceId,
    string ResourceName,
    string ResourceType,
    List<TimeSlotDto> AvailableSlots
);

public record ReservationReportDto(
    int ResourceId,
    DateTime FromDate,
    DateTime ToDate
);