using BitMiracle.LibTiff.Classic;
using CoordinateSharp;
using EmissionWiz.Models;
using EmissionWiz.Models.Database;
using EmissionWiz.Models.Dto;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Interfaces.Providers;
using EmissionWiz.Models.Interfaces.Repositories;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace EmissionWiz.Logic.Managers;

internal class GeoTiffManager : BaseManager, IGeoTiffManager
{
    private readonly IMapManager _mapManager;
    private readonly ITempFileRepository _tempFileRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GeoTiffManager(
        IMapManager mapManager,
        ITempFileRepository tempFileRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _mapManager = mapManager;
        _tempFileRepository = tempFileRepository;
        _dateTimeProvider = dateTimeProvider;
        Tiff.SetTagExtender(TagExtender);
    }

    public const TiffTag ProjCenterLongGeoKey = (TiffTag)3088;
    public const TiffTag ProjCenterLatGeoKey = (TiffTag)3089;

    public static void TagExtender(Tiff tif)
    {
        TiffFieldInfo[] tiffFieldInfo = [
            new TiffFieldInfo(TiffTag.GEOTIFF_MODELTIEPOINTTAG, 6, 6, TiffType.DOUBLE,
                    FieldBit.Custom, false, true, "MODELTILEPOINTTAG"),
            new TiffFieldInfo(TiffTag.GEOTIFF_MODELPIXELSCALETAG, 3, 3, TiffType.DOUBLE,
                    FieldBit.Custom, false, true, "MODELPIXELSCALETAG")
            ];

        tif.MergeFieldInfo(tiffFieldInfo, tiffFieldInfo.Length);
    }

    public async Task<Guid> GenerateGeoTiffAsync(GeoTiffOptions options)
    {
        // 1. Build tiff image
        var tif = BuildTiff(options);
        using var tifImage = await Image.LoadAsync(File.OpenRead(tif.TempFileName));
        using var ms = new MemoryStream();
        
        if (options.PrintMap)
        {
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
            using var tileImage = await Image.LoadAsync(tile);
            using var resizedTileImage = tileImage.Clone(x => x.Resize(tif.Width, tif.Height));

            using var output = tifImage.Clone(x => x.DrawImage(resizedTileImage, PixelColorBlendingMode.Overlay, PixelAlphaCompositionMode.SrcAtop, 0.25f));
            await output.SaveAsTiffAsync(ms);
        }
        else
        {
            await tifImage.SaveAsTiffAsync(ms);
        }
        
        var data = ms.ToArray();
        var tempFile = new TempFile
        {
            Id = Guid.NewGuid(),
            ContentType = Constants.ContentType.Tiff,
            Data = data,
            Timestamp = _dateTimeProvider.NowUtc,
            Label = options.OutputFileLabel,
            FileName = options.OutputFileName
        };

        _tempFileRepository.Add(tempFile);
        return tempFile.Id;
    }

    public TiffResult BuildTiff(GeoTiffOptions options)
    {
        var raster = options.Raster;
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

        tif.SetField(TiffTag.XRESOLUTION, 80.0);
        tif.SetField(TiffTag.YRESOLUTION, 80.0);

        tif.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
        tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
        tif.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

        tif.SetField(TiffTag.EXTRASAMPLES, 1, new[] { (short)ExtraSample.UNASSALPHA });

        // Geo data
        var topLeftCorner = new Coordinate(options.Center.Latitude.DecimalDegree, options.Center.Longitude.DecimalDegree);
        topLeftCorner.Move(options.Distance, 0, Shape.Sphere);
        topLeftCorner.Move(options.Distance, -90, Shape.Sphere);

        var scale = new double[] { 1, 1, 0 };
        tif.SetField(TiffTag.GEOTIFF_MODELPIXELSCALETAG, 3, (object)scale);

        var point = new double[] {
            0, 0, 0,
            topLeftCorner.UTM.Easting, topLeftCorner.UTM.Northing, 0,
        };
        tif.SetField(TiffTag.GEOTIFF_MODELTIEPOINTTAG, 6, (object)point);

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
            TempFileName = tempFile
        };
    }
}
