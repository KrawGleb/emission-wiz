namespace EmissionWiz.Models.Calculations.SingleSource;

public class SingleSourceEmissionCalculationResult
{
    public double MaxConcentration { get; set; }
    public double MaxUntowardConcentrationDistance { get; set; }
    public double DangerousWindSpeed { get; set; }
    public double DangerousDistance { get; set; }
}
