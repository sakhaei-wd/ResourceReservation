using Microsoft.EntityFrameworkCore;
using ResourceReservation.Core.Entities;
using ResourceReservation.Core.Interfaces;
using ResourceReservation.Infrastructure.Data;

namespace ResourceReservation.Infrastructure.Repositories;

public class ResourceRepository : IResourceRepository
{
    private readonly AppDbContext _context;

    public ResourceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Resource>> GetAllAsync()
        => await _context.Resources.ToListAsync();

    public async Task<Resource?> GetByIdAsync(int id)
        => await _context.Resources.FindAsync(id);
}