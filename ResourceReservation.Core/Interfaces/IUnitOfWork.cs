namespace ResourceReservation.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IReservationRepository Reservations { get; }
    IResourceRepository Resources { get; }
    Task<int> SaveChangesAsync();
}