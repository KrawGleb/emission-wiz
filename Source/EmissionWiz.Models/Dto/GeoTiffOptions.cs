using CoordinateSharp;

namespace EmissionWiz.Models.Dto;

public class GeoTiffOptions
{
    public string OutputFileLabel { get; set; } = null!;
    public string OutputFileName { get; set; } = null!;
    public double MeterInPixel { get; set; }
    public Coordinate Center { get; set; } = null!;

    /// <summary>
    /// Input: Distance, Degree
    /// Output: Value
    /// </summary>
    public Func<double, double, double>? GetValueFunc { get; set; }

    public double StartDistance { get; set; } = double.MaxValue;
    public double Distance { get; set; } = double.MinValue;
    public double Step { get; set; }

    public bool PrintMap { get; set; } = true;
    public double? HighlightValue { get; set; }
    public double? AcceptableError { get; set; }
}