using ResourceReservation.Core.Entities;

namespace ResourceReservation.Core.Interfaces;

public interface IResourceRepository
{
    Task<List<Resource>> GetAllAsync();
    Task<Resource?> GetByIdAsync(int id);
}