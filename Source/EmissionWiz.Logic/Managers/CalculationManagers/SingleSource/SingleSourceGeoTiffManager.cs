using EDW.AtoSales.Logic.Classes;
using EmissionWiz.Models;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Database;
using EmissionWiz.Models.Dto;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Interfaces.Providers;
using EmissionWiz.Models.Interfaces.Repositories;
using NPOI.XSSF.UserModel;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource;

internal class SingleSourceGeoTiffManager : BaseManager, ISingleSourceGeoTiffManager
{
    private readonly IGeoTiffManager _geoTiffManager;
    private readonly IMathManager _mathManager;
    private readonly ITempFileRepository _tempFileRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SingleSourceGeoTiffManager(
        IGeoTiffManager geoTiffManager,
        IMathManager mathManager,
        ITempFileRepository tempFileRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _geoTiffManager = geoTiffManager;
        _mathManager = mathManager;
        _tempFileRepository = tempFileRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<SingleSourceGeoTiffResult> BuildGeoTiff(SingleSourceEmissionCalculationResult results, SingleSourceCalculationData data, SingleSourceGeoTiffOptions options)
    {
        var xs = data.WindRose.Select(x => x.Degree).ToList();
        var xsCont = xs.Concat(xs.Select(x => x + 360)).ToArray();

        var ys = data.WindRose.Select(x => x.Speed).ToList();
        var ysCont = ys.Concat(ys).ToArray();

        var splinedWindSpeed = _mathManager.Spline(xsCont, ysCont, 360);

        var (cells, raster) = BuildRaster(options, results, data, splinedWindSpeed);
        var geoTiffOptions = new GeoTiffOptions()
        {
            Distance = options.Distance,
            Center = options.Center,
            PrintMap = options.PrintMap,
            OutputFileLabel = options.OutputFileLabel,
            OutputFileName = options.OutputGeoTiffFileName,
            Raster = raster,
        };

        var geoTiffId = await _geoTiffManager.GenerateGeoTiffAsync(geoTiffOptions);

        Guid? geoTiffDataFileId = null;
        if (options.IncludeGeoTiffData)
        {

            var ms = new MemoryStream();
            GenerateExcelReport(cells, ms, options.OutputExcelFileName);
            var tempFile = new TempFile
            {
                Id = Guid.NewGuid(),
                Data = ms.ToArray(),
                FileName = options.OutputExcelFileName,
                Label = options.OutputFileLabel,
                ContentType = Constants.ContentType.Xls,
                Timestamp = _dateTimeProvider.NowUtc

            };
            _tempFileRepository.Add(tempFile);
            geoTiffDataFileId = tempFile.Id;
        }

        return new SingleSourceGeoTiffResult
        {
            GeoTiffId = geoTiffId,
            CalculationReportId = geoTiffDataFileId
        };
    }

    private (List<SingleSourceGeoTiffData>, List<List<short>>) BuildRaster(SingleSourceGeoTiffOptions options, SingleSourceEmissionCalculationResult results, SingleSourceCalculationData data, SplineData windSpeed)
    {
        var values = new List<List<GeoTiffCellInfo>>();
        var cells = new List<SingleSourceGeoTiffData>();

        var maxValue = double.MinValue;
        var minValue = double.MaxValue;

        for (var row = -options.Distance; row <= options.Distance; row += options.Step)
        {
            var rowValues = new List<GeoTiffCellInfo>();
            values.Add(rowValues);

            for (var col = -options.Distance; col <= options.Distance; col += options.Step)
            {
                var distance = Math.Sqrt(Math.Pow(row, 2) + Math.Pow(col, 2));
                var degree = Math.Atan(Math.Abs(col / row)) * (180 / Math.PI);

                if (col < 0)
                {
                    if (row > 0)
                        degree = 180 + degree;
                    else
                        degree = 360 - degree;
                }
                else
                {
                    if (row > 0)
                        degree = 180 - degree;
                }

                var cellData = GetPointValue(distance, degree, results, data, windSpeed);
                cells.Add(cellData);

                var value = cellData.Value;
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

        return (cells, PaintRaster(unifiedValues, options.HighlightValue != null));
    }

    private SingleSourceGeoTiffData GetPointValue(double distance, double degree, SingleSourceEmissionCalculationResult results, SingleSourceCalculationData data, SplineData windSpeed)
    {
        var closestSpeedX = windSpeed.Xs.Select((x, i) => new { Index = i, Value = x }).OrderBy(x => Math.Abs(x.Value - degree)).First();
        var closestSpeed = windSpeed.Ys[closestSpeedX.Index];
        var rCoef = SingleSourceCommon.GetRCoef(closestSpeed, results.Um);
        var s1Coef = SingleSourceCommon.GetS1Coef(distance, results.Xm, data.FCoef);

        var value = results.Cm * rCoef * s1Coef;

        var cellData = new SingleSourceGeoTiffData
        {
            Degree = degree,
            Distance = distance,
            Value = value,
            WindSpeed = closestSpeed,
            RCoef = rCoef,
            S1Coef = s1Coef
        };

        return cellData;
    }

    private List<List<short>> PaintRaster(List<List<GeoTiffUnifiedCellInfo>> raster, bool enableHighlighting)
    {
        var colored = raster
            .Select(x => x.SelectMany(v => v.IsHighlighted && enableHighlighting
                ? [short.MaxValue / 2, 0, 0]
                : new List<short> { (short)(short.MaxValue - v.Value), (short)(short.MaxValue - v.Value), (short)(short.MaxValue - v.Value) }).ToList())
            .ToList();

        return colored;
    }

    private bool ShouldHighlightValue(double value, double? highlightValue, double? acceptableError)
    {
        if (highlightValue == null) return false;

        acceptableError ??= 0;
        return value >= highlightValue - acceptableError && value <= highlightValue + acceptableError;
    }

    private void GenerateExcelReport(List<SingleSourceGeoTiffData> rows, Stream destination, string name)
    {
        var wb = new XSSFWorkbook();
        var report = new NpoiReport<SingleSourceGeoTiffData>(wb, "Sheet1", name);

        report.AddColumn(x => x.Distance, "Дистанция", formatter: (v) => v.Distance.ToString("##.##"));
        report.AddColumn(x => x.Degree, "Ротация", formatter: (v) => v.Degree.ToString("##.##"));
        report.AddColumn(x => x.WindSpeed, "Скорость ветра", formatter: (v) => v.WindSpeed.ToString("##.##"));
        report.AddColumn(x => x.RCoef, "R", formatter: (v) => v.RCoef.ToString("##.##"));
        report.AddColumn(x => x.S1Coef, "S1", formatter: (v) => v.S1Coef.ToString("##.##"));
        report.AddColumn(x => x.Value, "Значение", formatter: (v) => v.Value.ToString("##.##"));

        report.Generate(rows);

        wb.Write(destination, true);
        destination.Seek(0, SeekOrigin.Begin);
    }
}
