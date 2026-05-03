namespace ResourceReservation.Core.Entities;

public enum ReservationStatus
{
    Active = 1,
    Cancelled = 2
}

public class Reservation
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public int UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;

    // Navigation properties
    public Resource Resource { get; set; } = default!;
    public User User { get; set; } = default!;
}