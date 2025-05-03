using application.DTOs;
using application.Interfaces;
using domain.Models.Policy;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<Policy>> CreatePolicy([FromBody] Policy policy)
    {
        var createdPolicy = await _policyRepository.CreateAsync(policy);
        return CreatedAtAction(nameof(GetPolicyById), new {id = createdPolicy.Id}, createdPolicy);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<Policy>> UpdatePolicy(int id, [FromBody] UpdatePolicyDTO updateDTO)
    {
        var existingPolicy = await _policyRepository.GetByIdAsync(id);
        if (existingPolicy == null) 
            return NotFound();
        
        existingPolicy.PolicyName = updateDTO.PolicyName;
        existingPolicy.Components = updateDTO.Components.Select(c => new PolicyComponent
        {
            Sequence = c.Sequence,
            Name = c.Name,
            Operation = c.Operation,
            FlatValue = c.FlatValue,
            PercentageValue = c.PercentageValue
        }).ToList();
        
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