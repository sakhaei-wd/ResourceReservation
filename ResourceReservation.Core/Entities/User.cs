namespace ResourceReservation.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}