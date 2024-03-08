using CoordinateSharp;

namespace EmissionWiz.Models.Dto;

public class GeoTiffOptions
{
    public double MeterInPixel { get; set; }
    public Coordinate Center { get; set; } = null!;
    public double? HighlightValue { get; set; }
    public double? AcceptableError { get; set; }

    /// <summary>
    /// Input: Distance
    /// Output: Value
    /// </summary>
    public Func<double, double>? GetValueFunc { get; set; }

    public double StartDistance { get; set; } = double.MaxValue;
    public double Distance { get; set; } = double.MinValue;
    public double Step { get; set; }
}