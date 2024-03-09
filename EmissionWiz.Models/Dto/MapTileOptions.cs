using CoordinateSharp;

namespace EmissionWiz.Models.Dto;

public class MapTileOptions
{
    public Coordinate Center { get; set; } = null!;
    public double Distance { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
}