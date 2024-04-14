namespace EmissionWiz.Models.Dto;

public class SubstanceDto
{
    public int Code { get; set; }
    public string Name { get; set; } = null!;
    public string? ChemicalFormula { get; set; }
    public byte? DangerClass { get; set; }
    public decimal? SingleMaximumAllowableConcentration { get; set; }
    public decimal? DailyAverageMaximumAllowableConcentration { get; set; }
    public decimal? AnnualAverageMaximumAllowableConcentration { get; set; }
}
