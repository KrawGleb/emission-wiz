using BitMiracle.LibTiff.Classic;
using CoordinateSharp;
using EmissionWiz.Models.Dto;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers;

internal class GeoTiffManager : BaseManager, IGeoTiffManager
{
    private readonly IMapManager _mapManager;

    public GeoTiffManager(IMapManager mapManager)
    {
        _mapManager = mapManager;
    }

    public void GenerateGeoTiff(GeoTiffOptions options)
    {
        // 1. Get real map image
        // 2. Build tiff image
        // 3. Combine 1st and 2nd

        BuildTiff(options);

    }

    public void BuildTiff(GeoTiffOptions options)
    {
        var raster = BuildRaster(options);
        var size = raster.Count;

        using var tif = Tiff.Open($"{Guid.NewGuid()}_test.tif", "w");

        tif.SetField(TiffTag.IMAGEWIDTH, size);
        tif.SetField(TiffTag.IMAGELENGTH, size);

        tif.SetField(TiffTag.COMPRESSION, Compression.LZW);
        tif.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);

        tif.SetField(TiffTag.SAMPLESPERPIXEL, 3);
        tif.SetField(TiffTag.BITSPERSAMPLE, 16);

        tif.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);

        tif.SetField(TiffTag.ROWSPERSTRIP, size);

        tif.SetField(TiffTag.XRESOLUTION, 88.0);
        tif.SetField(TiffTag.YRESOLUTION, 88.0);

        tif.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
        tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
        tif.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

        var rowIndex = 0;
        foreach (var row in raster)
        {
            var buffer = new byte[row.Count * sizeof(short)];
            Buffer.BlockCopy(row.ToArray(), 0, buffer, 0, buffer.Length);

            tif.WriteScanline(buffer, rowIndex);
            rowIndex++;
        }

        tif.WriteDirectory();
    }

    private List<List<short>> BuildRaster(GeoTiffOptions options)
    {
        var values = new List<List<double>>();

        var maxValue = double.MinValue;
        var minValue = double.MaxValue;

        for (var row = options.Distance; row >= -options.Distance; row -= options.Step)
        {
            var rowValues = new List<double>();
            values.Add(rowValues);

            for (var col = options.Distance; col >= -options.Distance; col -= options.Step)
            {
                var distance = Math.Sqrt(Math.Pow(row, 2) + Math.Pow(col, 2));
                var value = options.GetValueFunc!(distance);
                rowValues.Add(value);

                maxValue = double.Max(maxValue, value);
                minValue = double.Min(minValue, value);
            }
        }

        var unifiedValues = values
            .Select(a => a.Select(x => (short)((x - minValue) / (maxValue - minValue) * short.MaxValue)).ToList())
            .ToList();

        return PaintRaster(unifiedValues);
    }

    private List<List<short>> PaintRaster(List<List<short>> raster)
    {
        var colored = raster
            .Select(x => x.SelectMany(v => new List<short> { short.MaxValue / 2, v, short.MaxValue / 2 }).ToList())
            .ToList();

        return colored;
    }
}
