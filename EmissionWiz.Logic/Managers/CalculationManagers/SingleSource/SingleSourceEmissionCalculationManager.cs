using Autofac;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Database;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Interfaces.Providers;
using EmissionWiz.Models.Interfaces.Repositories;
using EmissionWiz.Models.Templates;
using System.Text.Json;

namespace EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;

// Метод расчета максимальных разовых концентраций от выбросов одиночного точечного источника
public class SingleSourceEmissionCalculationManager : BaseManager, ISingleSourceEmissionCalculationManager
{
    private readonly ISingleSourceEmissionReportModelBuilder _reportModelBuilder;
    private readonly IReportRepository _reportRepository;
    private readonly ICalculationResultRepository _calculationResultRepository;
    private readonly IReportManager _reportManager;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SingleSourceEmissionCalculationManager(
        ISingleSourceEmissionReportModelBuilder reportModelBuilder,
        IReportRepository reportRepository,
        ICalculationResultRepository calculationResultRepository,
        IReportManager reportManager,
        IDateTimeProvider dateTimeProvider)
    {
        _reportModelBuilder = reportModelBuilder;
        _reportRepository = reportRepository;
        _calculationResultRepository = calculationResultRepository;
        _reportManager = reportManager;
        _dateTimeProvider = dateTimeProvider;
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

        var r = GetRCoef(calculationData, results);
        var p = GetPCoef(calculationData, results);

        sourceProperties.RCoef = r;
        sourceProperties.PCoef = p;

        results.Cmu = CalculateCmu(sourceProperties, results);
        results.Xmu = CalculateXmu(sourceProperties, results);
        results.C = CalculateC(calculationData, results);
        results.Cy = CalculateCy(calculationData, results);

        var path = Path.GetDirectoryName(typeof(SingleSourceReportModel).Assembly.Location) + @"\ReportTemplates\SingleSource\main.xml";
        using var ms = new MemoryStream();
        await _reportManager.FromTemplate(ms, path, _reportModelBuilder.Build());
        var fileName = $"SingleSource_{calculationData.EmissionName}_{_dateTimeProvider.NowUtc}.pdf";

        var calculationResult = new CalculationResult()
        {
            Id = Guid.NewGuid(),
            Results = JsonSerializer.Serialize(results),
            Timestamp = _dateTimeProvider.NowUtc
        };

        var report = new Report()
        {
            Id = Guid.NewGuid(),
            OperationId = calculationResult.Id,
            ContentType = "application/pdf",
            FileName = fileName,
            Label = "SingleSource",
            Timestamp = _dateTimeProvider.NowUtc,
            Data = ms.ToArray()
        };

        _calculationResultRepository.Add(calculationResult);
        _reportRepository.Add(report);

        results.ReportId = report.Id;
        return results;
    }

    private double CalculateCm(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        IMaxConcentrationCalculationManager? subManager;

        if ((sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5)) && sourceProperties.VmI >= 0.5)
            subManager = ComponentContext.Resolve<IColdEmissionMaxConcentrationCalculationManager>();
        else if (sourceProperties.F < 100 && sourceProperties.Vm < 0.5 || sourceProperties.F >= 100 && sourceProperties.VmI < 0.5)
            subManager = ComponentContext.Resolve<ILowWindMaxConcentrationCalculationManager>();
        else
            subManager = ComponentContext.Resolve<IHotEmissionMaxConcentrationCalculationManager>();

        if (subManager == null)
            throw new InvalidOperationException("Failed to get required calculation manager");

        var cm = subManager.CalculateMaxConcentration(model, sourceProperties);

        _reportModelBuilder.SetCmResultValue(cm);

        return cm;
    }

    public double CalculateCy(SingleSourceCalculationData model, SingleSourceEmissionCalculationResult intermediateResults)
    {
        double ty;
        if (model.U <= 5)
        {
            ty = model.U * Math.Pow(model.Y, 2.0d) / Math.Pow(model.X, 2.0d);
        }
        else
        {
            ty = 5 * Math.Pow(model.Y, 2.0d) / Math.Pow(model.X, 2.0d);
        }

        var s2 = GetS2Coef(ty);
        var cy = s2 * intermediateResults.C;

        _reportModelBuilder
            .SetTyValue(ty)
            .SetCyValue(cy);

        return cy;
    }

    private double CalculateUm(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        IDangerousWindSpeedCalculationManager? subManager;
        if ((sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5)) && sourceProperties.VmI >= 0.5)
            subManager = ComponentContext.Resolve<IColdEmissionDangerousWindSpeedCalculationManager>();
        else if (sourceProperties.F < 100)
            subManager = ComponentContext.Resolve<ILowWindDangerousWindSpeedCalculationManager>();
        else
            subManager = ComponentContext.Resolve<IHotEmissionDangerousWindSpeedCalculationManager>();

        if (subManager == null)
            throw new InvalidOperationException("Failed to get required calculation manager");

        var um = subManager.CalculateDangerousWindSpeed(model, sourceProperties);

        _reportModelBuilder.SetUmValue(um);

        return um;
    }

    private double CalculateXm(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        IDangerousDistanceCalculationManager? subManager;
        if (sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5))
            subManager = ComponentContext.Resolve<IColdEmissionDangerousDistanceCalculationManager>();
        else if (sourceProperties.F < 100)
            subManager = ComponentContext.Resolve<ILowWindDangerousDistanceCalculationManager>();
        else
            subManager = ComponentContext.Resolve<IHotEmissionDangerousDistanceCalculationManager>();

        if (subManager == null)
            throw new InvalidOperationException("Failed to get required calculation manager");

        return subManager.CalculateDangerousDistance(model, sourceProperties);
    }

    public double CalculateCmu(EmissionSourceProperties sourceProperties, SingleSourceEmissionCalculationResult intermediateResults)
    {
        var result = intermediateResults.Cm * sourceProperties.RCoef;

        _reportModelBuilder.SetCmuValue(result);

        return result;
    }

    public double CalculateXmu(EmissionSourceProperties sourceProperties, SingleSourceEmissionCalculationResult intermediateResults)
    {
        var result = sourceProperties.PCoef * intermediateResults.Xm;

        _reportModelBuilder.SetXmuValue(result);

        return result;
    }

    public double CalculateC(SingleSourceCalculationData model, SingleSourceEmissionCalculationResult intermediateResults)
    {
        var s1 = GetS1Coef(model, intermediateResults);

        double result;
        if (model.H < 10 && model.X / intermediateResults.Xm < 1)
        {
            var s1h = 0.125 * (10 - model.H) + 0.125 * (model.H - 2) * s1;
            _reportModelBuilder.SetS1HValue(s1h);
            result = s1h * intermediateResults.Cm;
        }
        else
        {
            result = s1 * intermediateResults.Cm;
        }

        _reportModelBuilder.SetCValue(result);

        return result;
    }

    private double GetS1Coef(SingleSourceCalculationData model, SingleSourceEmissionCalculationResult intermediateResult)
    {
        double result;
        var ratio = model.X / intermediateResult.Xm;
        if (ratio <= 1)
        {
            result = 3 * Math.Pow(ratio, 4d) - 8 * Math.Pow(ratio, 3d) + 6 * Math.Pow(ratio, 2d);
        }
        else if (1 < ratio && ratio <= 8)
        {
            result = 1.13 / (0.13 * Math.Pow(ratio, 2d) + 1);
        }
        else if (8 < ratio && ratio <= 100 && model.FCoef <= 1.5)
        {
            result = ratio / (3.556 * Math.Pow(ratio, 2d) - 35.2 * ratio + 120);
        }
        else if (8 < ratio && ratio <= 100 && model.FCoef > 1.5)
        {
            result = 1 / (0.1 * Math.Pow(ratio, 2d) + 2.456 * ratio - 17.8);
        }
        else if (ratio > 100 && model.FCoef <= 1.5)
        {
            result = 144.3 * Math.Cbrt(Math.Pow(ratio, -7d));
        }
        else
        {
            result = 37.76 * Math.Cbrt(Math.Pow(ratio, -7d));
        }

        _reportModelBuilder.SetS1Value(result);

        return result;
    }

    private double GetS2Coef(double ty)
    {
        var result = 1.0d / Math.Pow((1 + 5d * ty + 12.8d * Math.Pow(ty, 2.0d) + 17 * Math.Pow(ty, 3.0d) + 45.1d * Math.Pow(ty, 4)), 2.0d);

        _reportModelBuilder.SetS2Value(result);

        return result;
    }

    private double GetRCoef(SingleSourceCalculationData model, SingleSourceEmissionCalculationResult intermediateResults)
    {
        var ratio = model.U / intermediateResults.Um;

        double result;
        if (ratio <= 1)
        {
            result = 0.67 * ratio + 1.67 * Math.Pow(ratio, 2) - 1.34 * Math.Pow(ratio, 3);
        }
        else
        {
            result = (3 * ratio) / (2 * Math.Pow(ratio, 2) - ratio + 2);
        }

        _reportModelBuilder.SetRCoefValue(result);

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

    private double GetV1(SingleSourceCalculationData model)
    {
        var result = Math.PI * Math.Pow(model.D, 2d) / 4 * model.W;

        return result;
    }

    private EmissionSourceProperties GetEmissionSourceProperties(SingleSourceCalculationData model)
    {
        var v = GetV1(model);

        var vm = 0.65d * Math.Cbrt((v * model.DeltaT + 0.0d) / model.H);
        var vmI = 1.3d * (model.W * model.D / model.H);
        var f = 1000d * (Math.Pow(model.W, 2d) * model.D) / (Math.Pow(model.H, 2d) * model.DeltaT);
        var fe = 800d * Math.Pow(vmI, 3d);

        return new EmissionSourceProperties
        {
            V1 = v,
            Vm = vm,
            VmI = vmI,
            F = f,
            Fe = fe
        };
    }
}
