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

        Tiff.SetTagExtender(TagExtender);
    }

    public const TiffTag ProjCenterLongGeoKey = (TiffTag)3088;
    public const TiffTag ProjCenterLatGeoKey = (TiffTag)3089;

    public static void TagExtender(Tiff tif)
    {
        TiffFieldInfo[] tiffFieldInfo = [
             new TiffFieldInfo(ProjCenterLatGeoKey, 2, 2, TiffType.DOUBLE,
                    FieldBit.Custom, true, true, "ProjCenterLatGeoKey"),
            new TiffFieldInfo(ProjCenterLongGeoKey, 2, 2, TiffType.DOUBLE,
                    FieldBit.Custom, true, true, "ProjCenterLongGeoKey")];

        tif.MergeFieldInfo(tiffFieldInfo, tiffFieldInfo.Length);
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

        var filePath = $"res_{Guid.NewGuid()}.tif";

        using var output = tifImage.Clone(x => x.DrawImage(resizedTileImage, PixelColorBlendingMode.Overlay, PixelAlphaCompositionMode.SrcAtop, 0.25f));
        await output.SaveAsTiffAsync(filePath);
    }

    public TiffResult BuildTiff(GeoTiffOptions options)
    {
        var raster = BuildRaster(options);
        var size = raster.Count;

        var tempFile = Path.GetTempFileName();
        using var tif = Tiff.Open(tempFile, "w8");

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

        // Geo data
        tif.SetField(ProjCenterLatGeoKey, 1, new double[] { options.Center.Latitude.DecimalDegree });
        tif.SetField(ProjCenterLongGeoKey, 1, new double[] { options.Center.Longitude.DecimalDegree });

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
        var values = new List<List<GeoTiffCellInfo>>();

        var maxValue = double.MinValue;
        var minValue = double.MaxValue;

        for (var row = options.Distance; row >= -options.Distance; row -= options.Step)
        {
            var rowValues = new List<GeoTiffCellInfo>();
            values.Add(rowValues);

            for (var col = options.Distance; col >= -options.Distance; col -= options.Step)
            {
                var distance = Math.Sqrt(Math.Pow(row, 2) + Math.Pow(col, 2));
                var value = options.GetValueFunc!(distance);
                rowValues.Add(new GeoTiffCellInfo()
                {
                    Value = value,
                    IsHighlighted = ShouldHighlightValue(value, options.HighlightValue, options.AcceptableError)
                });

                maxValue = double.Max(maxValue, value);
                minValue = double.Min(minValue, value);
            }
        }

        var unifiedValues = values
            .Select(a => a.Select(x => new GeoTiffUnifiedCellInfo()
            {
                Value = (short)((x.Value - minValue) / (maxValue - minValue) * short.MaxValue),
                IsHighlighted = x.IsHighlighted
            }).ToList())
            .ToList();

        return PaintRaster(unifiedValues);
    }

    private List<List<short>> PaintRaster(List<List<GeoTiffUnifiedCellInfo>> raster)
    {
        var colored = raster
            .Select(x => x.SelectMany(v => v.IsHighlighted
                ? [short.MaxValue / 2, 0, 0]
                : new List<short> { v.Value, v.Value, v.Value }).ToList())
            .ToList();

        return colored;
    }

    private bool ShouldHighlightValue(double value, double? highlightValue, double? acceptableError)
    {
        if (highlightValue == null) return false;

        acceptableError ??= 0;
        return value >= highlightValue - acceptableError && value <= highlightValue + acceptableError;
    }
}
