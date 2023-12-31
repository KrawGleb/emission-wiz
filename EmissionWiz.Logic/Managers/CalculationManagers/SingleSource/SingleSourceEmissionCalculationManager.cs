using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Templates;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;

// Метод расчета максимальных разовых концентраций от выбросов одиночного точечного источника
public class SingleSourceEmissionCalculationManager : BaseManager, ISingleSourceEmissionCalculationManager
{
    private readonly ISingleSourceEmissionReportModelBuilder _reportModelBuilder;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IReportManager _reportManager;

    public SingleSourceEmissionCalculationManager(
        ISingleSourceEmissionReportModelBuilder reportModelBuilder,
        IHttpContextAccessor httpContextAccessor,
        IReportManager reportManager)
    {
        _reportModelBuilder = reportModelBuilder;
        _httpContextAccessor = httpContextAccessor;
        _reportManager = reportManager;
    }

    public async Task<(SingleSourceEmissionCalculationResult, Stream)> Calculate(SingleSourceInputModel model, string reportName)
    {
        var sourceProperties = GetEmissionSourceProperties(model);

        _reportModelBuilder
            .UseInputModel(model)
            .UseSourceProperties(sourceProperties);

        var intermediateResults = new SingleSourceEmissionCalculationResult()
        {
            Cm = CalculateCm(model, sourceProperties),
            Xm = CalculateXm(model, sourceProperties),
            Um = CalculateUm(model, sourceProperties)
        };

        var r = GetRCoef(model, intermediateResults);
        var p = GetPCoef(model, intermediateResults);

        sourceProperties.RCoef = r;
        sourceProperties.PCoef = p;

        intermediateResults.Cmu = CalculateCmu(sourceProperties, intermediateResults);
        intermediateResults.Xmu = CalculateXmu(sourceProperties, intermediateResults);
        intermediateResults.C = CalculateC(model, intermediateResults);

        var path = Path.GetDirectoryName(typeof(SingleSourceReportModel).Assembly.Location) + @"\ReportTemplates\SingleSource\main.xml";
        var ms = new MemoryStream();
        await _reportManager.FromTemplate(ms, path, _reportModelBuilder.Build());

        return (intermediateResults, ms);
    }

    private double CalculateCm(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        IMaxConcentrationCalculationManager? subManager;

        if ((sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5)) && sourceProperties.VmI >= 0.5)
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IColdEmissionMaxConcentrationCalculationManager>();
        else if (sourceProperties.F < 100 && sourceProperties.Vm < 0.5 || sourceProperties.F >= 100 && sourceProperties.VmI < 0.5)
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<ILowWindMaxConcentrationCalculationManager>();
        else
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IHotEmissionMaxConcentrationCalculationManager>();

        if (subManager == null)
            throw new InvalidOperationException("Failed to get required calculation manager");

        var cm = subManager.CalculateMaxConcentration(model, sourceProperties);

        _reportModelBuilder.SetCmResultValue(cm);

        return cm;
    }

    private double CalculateUm(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        IDangerousWindSpeedCalculationManager? subManager;
        if ((sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5)) && sourceProperties.VmI >= 0.5)
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IColdEmissionDangerousWindSpeedCalculationManager>();
        else if (sourceProperties.F < 100)
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<ILowWindDangerousWindSpeedCalculationManager>();
        else
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IHotEmissionDangerousWindSpeedCalculationManager>();

        if (subManager == null)
            throw new InvalidOperationException("Failed to get required calculation manager");

        var um = subManager.CalculateDangerousWindSpeed(model, sourceProperties);

        _reportModelBuilder.SetUmValue(um);

        return um;
    }

    private double CalculateXm(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        IDangerousDistanceCalculationManager? subManager;
        if (sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5))
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IColdEmissionDangerousDistanceCalculationManager>();
        else if (sourceProperties.F < 100)
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<ILowWindDangerousDistanceCalculationManager>();
        else
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IHotEmissionDangerousDistanceCalculationManager>();

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

    public double CalculateC(SingleSourceInputModel model, SingleSourceEmissionCalculationResult intermediateResults)
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

    private double GetS1Coef(SingleSourceInputModel model, SingleSourceEmissionCalculationResult intermediateResult)
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

    private double GetRCoef(SingleSourceInputModel model, SingleSourceEmissionCalculationResult intermediateResults)
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

    private double GetPCoef(SingleSourceInputModel model, SingleSourceEmissionCalculationResult intermediateReults)
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

    private double GetV1(SingleSourceInputModel model)
    {
        var result = Math.PI * Math.Pow(model.D, 2d) / 4 * model.W;

        return result;
    }

    private EmissionSourceProperties GetEmissionSourceProperties(SingleSourceInputModel model)
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
