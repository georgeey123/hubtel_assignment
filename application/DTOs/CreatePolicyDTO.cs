using domain.Enums;

namespace application.DTOs;

public class CreatePolicyDTO
{
    public string PolicyName { get; set; }
    public List<CreatePolicyComponentDTO> Components { get; set; }
}

public class CreatePolicyComponentDTO
{
    public int Sequence { get; set; }   
    public string Name { get; set; }
    public OperationType Operation { get; set; }
    public decimal FlatValue { get; set; }
    public decimal PercentageValue { get; set; }
}