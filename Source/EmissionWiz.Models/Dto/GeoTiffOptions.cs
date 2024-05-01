using CoordinateSharp;

namespace EmissionWiz.Models.Dto;

public class GeoTiffOptions
{
    public string OutputFileLabel { get; set; } = null!;
    public string OutputFileName { get; set; } = null!;
    public Coordinate Center { get; set; } = null!;
    public List<List<short>> Raster { get; set; } = new();
    public double Distance { get; set; } = double.MinValue;
    public bool PrintMap { get; set; } = true;
}