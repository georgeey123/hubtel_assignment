using domain.Enums;

namespace domain.Models.Policy;
public class PolicyComponent
{
    public int Sequence { get; set; }
    public string Name { get; set; }
    public OperationType Operation { get; set; }
    public decimal FlatValue { get; set; }
    public decimal PercentageValue { get; set; }
}