using System.Text.Json;
using Autofac;
using CoordinateSharp;
using EmissionWiz.Models;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Database;
using EmissionWiz.Models.Dto;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Interfaces.Providers;
using EmissionWiz.Models.Interfaces.Repositories;
using EmissionWiz.Models.Templates;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource;

// Метод расчета максимальных разовых концентраций от выбросов одиночного точечного источника
public class SingleSourceEmissionCalculationManager : BaseManager, ISingleSourceEmissionCalculationManager
{
    private readonly ISingleSourceEmissionReportModelBuilder _reportModelBuilder;
    private readonly ISingleSourceGeoTiffManager _singleSourceGeoTiffManager;
    private readonly IReportManager _reportManager;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMathManager _mathManager;
    private readonly ITempFileRepository _tempFileRepository;

    public SingleSourceEmissionCalculationManager(
        ISingleSourceEmissionReportModelBuilder reportModelBuilder,
        ISingleSourceGeoTiffManager singleSourceGeoTiffManager,
        IReportManager reportManager,
        IDateTimeProvider dateTimeProvider,
        IMathManager mathManager,
        ITempFileRepository tempFileRepository)
    {
        _reportModelBuilder = reportModelBuilder;
        _singleSourceGeoTiffManager = singleSourceGeoTiffManager;
        _reportManager = reportManager;
        _dateTimeProvider = dateTimeProvider;
        _mathManager = mathManager;
        _tempFileRepository = tempFileRepository;
    }

    public async Task<SingleSourceEmissionCalculationResult> Calculate(SingleSourceCalculationData calculationData)
    {
        var sourceProperties = GetEmissionSourceProperties(calculationData);

        _reportModelBuilder
            .UseInputModel(calculationData)
            .UseSourceProperties(sourceProperties);

        var results = new SingleSourceEmissionCalculationResult()
        {
            Name = calculationData.EmissionName,
            Cm = CalculateCm(calculationData, sourceProperties),
            Xm = CalculateXm(calculationData, sourceProperties),
            Um = CalculateUm(calculationData, sourceProperties)
        };

        var r = SingleSourceCommon.GetRCoef(calculationData.U, results.Um);
        var p = GetPCoef(calculationData, results);

        sourceProperties.RCoef = r;
        sourceProperties.PCoef = p;

        results.Cmu = CalculateCmu(sourceProperties, results);
        results.Xmu = CalculateXmu(sourceProperties, results);

        _reportModelBuilder
            .SetCmResultValue(results.Cm)
            .SetXmValue(results.Xm)
            .SetUmValue(results.Um)
            .SetRCoefValue(r)
            .SetPCoefValue(p)
            .SetCmuValue(results.Cmu)
            .SetXmuValue(results.Xmu);

        var sharedLabel = $"SingleSource_{calculationData.EmissionName}_{_dateTimeProvider.NowUtc}";

        var geoTiffResult = await _singleSourceGeoTiffManager.BuildGeoTiff(results, calculationData, new SingleSourceGeoTiffOptions()
        {
            StartDistance = 0,
            Distance = 2 * results.Xm,
            Step = 1,
            MeterInPixel = 1,
            Center = new Coordinate(calculationData.Lat, calculationData.Lon),

            HighlightValue = calculationData.ResultsConfig.HighlightValue == null ? null : calculationData.ResultsConfig.HighlightValue * results.Cm,
            AcceptableError = calculationData.ResultsConfig.AcceptableError == null ? null : calculationData.ResultsConfig.AcceptableError * results.Cm,
            PrintMap = calculationData.ResultsConfig.PrintMap,
            IncludeGeoTiffData = calculationData.ResultsConfig.IncludeGeoTiffData,

            OutputFileLabel = sharedLabel,
            OutputGeoTiffFileName = sharedLabel + ".tif",
            OutputExcelFileName = sharedLabel + ".xlsx"
        });

        results.Files.Add(new Models.Calculations.FileContent
        {
            FileId = geoTiffResult.GeoTiffId,
            Name = "GeoTiff",
            SortOrder = 1,
            Type = Models.Enums.FileContentType.Image
        });

        if (geoTiffResult.CalculationReportId != null)
        {
            results.Files.Add(new Models.Calculations.FileContent
            {
                FileId = geoTiffResult.CalculationReportId.Value,
                Name = "GeoTiff data (Excel)",
                SortOrder = 2,
                Type = Models.Enums.FileContentType.Unknown
            });
        }

        var reportModel = _reportModelBuilder.Build();

        var basePath = Path.GetDirectoryName(typeof(SingleSourceReportModel).Assembly.Location);
        var mainPath = basePath + @"\ReportTemplates\SingleSource\main.xml";

        using var ms = new MemoryStream();
        await _reportManager.FromTemplate(ms, mainPath, reportModel);
        var fileName = $"SingleSource_{calculationData.EmissionName}_{_dateTimeProvider.NowUtc}.pdf";

        var tempFile = new TempFile
        {
            Id = Guid.NewGuid(),
            Label = sharedLabel,
            Data = ms.ToArray(),
            Timestamp = _dateTimeProvider.NowUtc,
            ContentType = Constants.ContentType.Pdf,
            FileName = sharedLabel + ".pdf",
        };

        _tempFileRepository.Add(tempFile);
        results.Files.Add(new Models.Calculations.FileContent
        {
            FileId = tempFile.Id,
            Name = $"Вычисления",
            SortOrder = 3,
            Type = Models.Enums.FileContentType.Pdf
        });

        return results;
    }

    private double CalculateCm(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        var manager = ComponentContext.Resolve<ISingleSourceCmCalculationManager>();
        var cm = manager.CalculateCm(model, sourceProperties);

        return cm;
    }

    private double CalculateUm(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        var manager = ComponentContext.Resolve<ISingleSourceUmCalculationManager>();
        var um = manager.CalculateUm(model, sourceProperties);

        return um;
    }

    private double CalculateXm(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        var manager = ComponentContext.Resolve<ISingleSourceXmCalculationManager>();
        var xm = manager.CalculateXm(model, sourceProperties);

        return xm;
    }

    public double CalculateCmu(EmissionSourceProperties sourceProperties, SingleSourceEmissionCalculationResult intermediateResults)
    {
        var result = intermediateResults.Cm * sourceProperties.RCoef;

        return result;
    }

    public double CalculateXmu(EmissionSourceProperties sourceProperties, SingleSourceEmissionCalculationResult intermediateResults)
    {
        var result = sourceProperties.PCoef * intermediateResults.Xm;

        return result;
    }

    private double GetPCoef(SingleSourceCalculationData model, SingleSourceEmissionCalculationResult intermediateReults)
    {
        var ratio = model.U / intermediateReults.Um;

        double result;
        if (ratio <= 0.25)
        {
            result = 3;
        }
        else if (ratio <= 1)
        {
            result = 8.43 * Math.Pow(1 - ratio, 5) + 1;
        }
        else
        {
            result = 0.32 * ratio + 0.68;
        }

        _reportModelBuilder.SetPCoefValue(result);

        return result;
    }

    private EmissionSourceProperties GetEmissionSourceProperties(SingleSourceCalculationData model)
    {
        var v = GetV1(model);

        var vm = 0.65d * Math.Cbrt((v * model.DeltaT + 0.0d) / model.H);
        var vmI = 1.3d * (model.W * model.D / model.H);
        var f = 1000d * (Math.Pow(model.W, 2d) * model.D) / (Math.Pow(model.H, 2d) * model.DeltaT);
        var fe = 800d * Math.Pow(vmI, 3d);

        // Для прямоугольных устьев
        double? v1e = null;
        if (model.B != null && model.L != null)
        {
            var w0 = v / (model.L * model.B);
            var d = (2d * model.L * model.B) / (model.L + model.B);
            v1e = ((Math.PI * Math.Pow(d!.Value, 2d)) / 4d) * w0;

            model.W = w0!.Value;
            model.D = d!.Value;
        }

        return new EmissionSourceProperties
        {
            V1Source = v,
            V1e = v1e,
            V1 = v,
            Vm = vm,
            VmI = vmI,
            F = f,
            Fe = fe
        };
    }

    private double GetS2Coef(double ty)
    {
        var result = 1.0d / Math.Pow((1 + 5d * ty + 12.8d * Math.Pow(ty, 2.0d) + 17 * Math.Pow(ty, 3.0d) + 45.1d * Math.Pow(ty, 4)), 2.0d);

        _reportModelBuilder.SetS2Value(result);

        return result;
    }

    private double GetV1(SingleSourceCalculationData model)
    {
        var result = Math.PI * Math.Pow(model.D, 2d) / 4 * model.W;

        return result;
    }
}
