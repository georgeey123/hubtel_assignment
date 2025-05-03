using domain.Enums;
using domain.Models.Policy;

namespace application.DTOs;

public class UpdatePolicyDTO
{
    public string PolicyName { get; set; }
    public List<UpdatePolicyComponentDTO> Components { get; set; }
}

public class UpdatePolicyComponentDTO
{
    public int Sequence { get; set; }
    public string Name { get; set; }
    public OperationType Operation { get; set; }
    public decimal FlatValue { get; set; }
    public decimal PercentageValue { get; set; }
}