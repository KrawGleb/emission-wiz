using BitMiracle.LibTiff.Classic;

namespace EmissionWiz.Models.Dto;

public class GeoKeyDirectoryResult
{
    public int[] GeoKeyDirectoryTag { get; set; } = null!;
    public object[] GeoDoubleParamsTag { get; set; } = null!;
    public object[] GeoAsciiParamsTag { get; set; } = null!;
}

