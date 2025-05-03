using application.DTOs;
using application.Interfaces;
using domain.Models.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PolicyController : ControllerBase
{
    private readonly IPolicyRepository _policyRepository;
    
    public PolicyController(IPolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<Policy>>> GetAllPolicies()
    {
        var policies = await _policyRepository.GetAllAsync();
        return Ok(policies);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Policy>> GetPolicyById(int id)
    {
        var policy = await _policyRepository.GetByIdAsync(id);
        if (policy == null) return NotFound();
        return Ok(policy);
    }
    
    
    [HttpPost]
    public async Task<ActionResult<Policy>> CreatePolicy([FromBody] CreatePolicyDTO createDTO)
    {
        var policy = new Policy
        {
            PolicyName = createDTO.PolicyName,
            Components = createDTO.Components.Select(c => new PolicyComponent
            {
                Sequence = c.Sequence,
                Name = c.Name,
                Operation = c.Operation,
                FlatValue = c.FlatValue,
                PercentageValue = c.PercentageValue
            }).ToList()
        };

        var createdPolicy = await _policyRepository.CreateAsync(policy);
        return CreatedAtAction(nameof(GetPolicyById), new { id = createdPolicy.Id }, createdPolicy);
    }

    
    [HttpPatch("{id}")]
    public async Task<ActionResult<Policy>> PatchPolicy(int id, [FromBody] UpdatePolicyDTO updateDTO)
    {
        var existingPolicy = await _policyRepository.GetByIdAsync(id);
        if (existingPolicy == null)
            return NotFound();
        
        // Update Policy Name if provided
        if (updateDTO.PolicyName != null)
        {
            existingPolicy.PolicyName = updateDTO.PolicyName;
        }

        // Update Components if provided
        if (updateDTO.Components != null)
        {
            foreach (var componentPatch in updateDTO.Components)
            {
                var existingComponent = existingPolicy.Components
                    .FirstOrDefault(c => c.Sequence == componentPatch.Sequence);

                if (existingComponent != null)
                {
                    // Update only provided values
                    if (componentPatch.Name != null)
                        existingComponent.Name = componentPatch.Name;
                
                    if (componentPatch.Operation.HasValue)
                        existingComponent.Operation = componentPatch.Operation.Value;
                
                    if (componentPatch.FlatValue.HasValue)
                        existingComponent.FlatValue = componentPatch.FlatValue.Value;
                
                    if (componentPatch.PercentageValue.HasValue)
                        existingComponent.PercentageValue = componentPatch.PercentageValue.Value;
                }
            }
        }

        existingPolicy.UpdatedAt = DateTime.UtcNow;
        var updatedPolicy = await _policyRepository.UpdateAsync(existingPolicy);
        return Ok(updatedPolicy);
    }



    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeletePolicy(int id)
    {
        var success = await _policyRepository.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}