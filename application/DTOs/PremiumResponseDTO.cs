namespace application.DTOs;

public class PremiumResponseDTO
{
    public int PolicyId { get; set; }
    public String PolicyName { get; set; }
    public decimal Premium { get; set; }
    public List<CalculationDetail> CalculationDetails { get; set; } = new();

}

public class CalculationDetail
{
    public int Sequence { get; set; }
    public string ComponentName { get; set; }
    public string Operation { get; set; }
    public decimal Amount { get; set; }
    public decimal RunningTotal { get; set; }
}
