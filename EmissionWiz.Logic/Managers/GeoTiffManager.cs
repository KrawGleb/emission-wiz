using BitMiracle.LibTiff.Classic;
using EmissionWiz.Models.Dto;
using EmissionWiz.Models.Interfaces.Managers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace EmissionWiz.Logic.Managers;

internal class GeoTiffManager : BaseManager, IGeoTiffManager
{
    private readonly IMapManager _mapManager;

    public GeoTiffManager(IMapManager mapManager)
    {
        _mapManager = mapManager;
    }

    public async Task GenerateGeoTiffAsync(GeoTiffOptions options)
    {
        // 1. Build tiff image
        var tif = BuildTiff(options);

        // 2. Get real map image
        var tile = await _mapManager.GetTileAsync(new MapTileOptions()
        {
            Distance = options.Distance,
            Center = options.Center,
            Height = tif.Height,
            Width = tif.Width,
        });

        tile.Seek(0, SeekOrigin.Begin);

        // 3. Combine 1st and 2nd
        using var tifImage = await Image.LoadAsync(File.OpenRead(tif.TempFileName));
        
        using var tileImage = await Image.LoadAsync(tile);
        using var resizedTileImage = tileImage.Clone(x => x.Resize(tif.Width, tif.Height));

        using var output = tifImage.Clone(x => x.DrawImage(resizedTileImage, PixelColorBlendingMode.Overlay, PixelAlphaCompositionMode.SrcAtop, 0.25f));
        await output.SaveAsync($"res_{Guid.NewGuid()}.jpg");
    }

    public TiffResult BuildTiff(GeoTiffOptions options)
    {
        var raster = BuildRaster(options);
        var size = raster.Count;

        var tempFile = Path.GetTempFileName();
        using var tif = Tiff.Open(tempFile, "w");

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

        tif.SetField(TiffTag.EXTRASAMPLES, 1, new[] { (short)ExtraSample.UNASSALPHA });

        var rowIndex = 0;
        foreach (var row in raster)
        {
            var buffer = new byte[row.Count * sizeof(short)];
            Buffer.BlockCopy(row.ToArray(), 0, buffer, 0, buffer.Length);

            tif.WriteScanline(buffer, rowIndex);
            rowIndex++;
        }

        tif.WriteDirectory();

        return new TiffResult
        {
            Height = size,
            Width = size,
            Image = tif.GetStream(),
            TempFileName = tempFile
        };
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

        return PaintRaster(unifiedValues, options.HighlightValue, options.AcceptableError);
    }

    private List<List<short>> PaintRaster(List<List<short>> raster, double? highlightValue, double? acceptableError)
    {
        short halfOfShort = short.MaxValue / 2;

        var colored = raster
            .Select(x => x.SelectMany(v =>
            {
                var defaultValue = new List<short> { v, v, v };
                if (highlightValue == null)
                {
                    return defaultValue;
                }

                return ShouldHighlightValue(v, highlightValue.Value, acceptableError)
                    ? [0, 0, 0]
                    : defaultValue;
            }).ToList())
            .ToList();

        return colored;
    }

    private bool ShouldHighlightValue(double value, double highlightValue, double? acceptableError)
    {
        acceptableError ??= 0;
        return value >= highlightValue - acceptableError && value <= highlightValue + acceptableError;
    }
}
