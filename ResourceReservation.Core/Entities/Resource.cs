namespace ResourceReservation.Core.Entities;

public class Resource
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    
    /// <summary>نوع منبع: اتاق جلسه، پروژکتور و...</summary>
    public string Type { get; set; } = default!;
    
    /// <summary>حداکثر استفاده همزمان - برای منابعی مثل اتاق جلسه</summary>
    public int MaxConcurrentUsage { get; set; } = 1;
    
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}