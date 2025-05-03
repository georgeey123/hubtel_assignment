using application.Interfaces;
using domain.Enums;
using domain.Models.Policy;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure.Data;

public static class DataSeeder
{
    public static void SeedData(IServiceProvider serviceProvider)
    {
        var repository = serviceProvider.GetRequiredService<IPolicyRepository>();
        
        if (repository.GetAllAsync().Result.Count == 0)
        {
            repository.CreateAsync(new Policy
            {
                PolicyName = "Low Claim Policy",
                Components = new List<PolicyComponent>
                {
                    new() { Sequence = 1, Name = "Premium Base", Operation = OperationType.Add, FlatValue = 300 },
                    new() { Sequence = 2, Name = "Extra Perils", Operation = OperationType.Add, FlatValue = 100 },
                    new() { Sequence = 3, Name = "Market Value Premium", Operation = OperationType.Add, PercentageValue = 10 },
                    new() { Sequence = 4, Name = "Promo Discount", Operation = OperationType.Subtract }
                }
            }).Wait();
        }
    }
}