using CoordinateSharp;

namespace EmissionWiz.Models.Calculations.SingleSource;

public class SingleSourceGeoTiffOptions
{
    public string OutputFileLabel { get; set; } = null!;
    public string OutputGeoTiffFileName { get; set; } = null!;
    public string OutputExcelFileName { get; set; } = null!;
    public double MeterInPixel { get; set; }
    public Coordinate Center { get; set; } = null!;

    public double StartDistance { get; set; } = double.MaxValue;
    public double Distance { get; set; } = double.MinValue;
    public double Step { get; set; }

    public bool IncludeGeoTiffData { get; set; } = false;
    public bool PrintMap { get; set; } = true;
    public double? HighlightValue { get; set; }
    public double? AcceptableError { get; set; }
}
