using BitMiracle.LibTiff.Classic;

namespace EmissionWiz.Models.Dto;

public class TiffResult
{
    public int Height { get; set; }
    public int Width { get; set; }
    public TiffStream Image { get; set; } = null!;
    public string TempFileName { get; set; } = null!;
}