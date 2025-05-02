using domain.Models.Policy;

namespace application.Interfaces;

public interface IPolicyRepository
{
    Task<Policy> CreateAsync (Policy policy);
    Task<Policy?> GetByIdAsync (int id);
    Task<List<Policy>> GetAllAsync ();
    Task<Policy?> UpdateAsync (Policy policy);
    Task<bool> DeleteAsync (int id);
}