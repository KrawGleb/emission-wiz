namespace EmissionWiz.Models.Calculations.SingleSource;

public class SingleSourceResultsConfig
{
    public bool PrintMap { get; set; } = true;
    public double? HighlightValue { get; set; }
    public double? AcceptableError { get; set; }
    public bool IncludeGeoTiffData { get; set; } = false;
}
