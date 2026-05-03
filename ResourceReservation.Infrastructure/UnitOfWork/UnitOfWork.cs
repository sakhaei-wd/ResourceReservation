using ResourceReservation.Core.Interfaces;
using ResourceReservation.Infrastructure.Data;
using ResourceReservation.Infrastructure.Repositories;

namespace ResourceReservation.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IReservationRepository Reservations { get; }
    public IResourceRepository Resources { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Reservations = new ReservationRepository(context);
        Resources = new ResourceRepository(context);
    }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();
}