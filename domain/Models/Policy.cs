namespace domain.Models.Policy;

public class Policy: BaseEntity
{
    public string PolicyName { get; set; }
    public List<PolicyComponent> Components { get; set; }
}