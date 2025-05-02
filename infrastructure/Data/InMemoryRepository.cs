using application.Interfaces;
using domain.Models.Policy;

namespace infrastructure.Data;

public class InMemoryRepository : IPolicyRepository
{
    private readonly List<Policy> _policies = new();

    public Task<Policy> CreateAsync(Policy policy)
    {
        policy.Id = _policies.Count + 1;
        policy.CreatedAt = DateTime.UtcNow;
        policy.UpdatedAt = DateTime.UtcNow;
        _policies.Add(policy);
        return Task.FromResult(policy);
    }

    public Task<Policy?> GetByIdAsync(int id)
    {
        return Task.FromResult(_policies.FirstOrDefault(p => p.Id == id));
    }

    public Task<List<Policy>> GetAllAsync()
    {
        return Task.FromResult(_policies);
    }

    public Task<Policy?> UpdateAsync(Policy policy)
    {
        var existingPolicy = _policies.FirstOrDefault(p => p.Id == policy.Id);
        if (existingPolicy == null) return Task.FromResult<Policy?>(null);
        
        existingPolicy.PolicyName = policy.PolicyName;
        existingPolicy.Components = policy.Components;
        existingPolicy.UpdatedAt = DateTime.UtcNow;
        
        return Task.FromResult<Policy?>(existingPolicy);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var policy = _policies.FirstOrDefault(p => p.Id == id);
        if (policy == null) return Task.FromResult(false);
        
        _policies.Remove(policy);
        return Task.FromResult(true);
    }
}