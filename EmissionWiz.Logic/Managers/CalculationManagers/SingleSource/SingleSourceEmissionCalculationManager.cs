using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Templates;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;

// Метод расчета максимальных разовых концентраций от выбросов одиночного точечного источника
public class SingleSourceEmissionCalculationManager : ISingleSourceEmissionCalculationManager
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

    public async Task<SingleSourceEmissionCalculationResult> Calculate(SingleSourceInputModel model, string reportName)
    {
        var sourceProperties = GetEmissionSourceProperties(model);

        _reportModelBuilder
            .UseInputModel(model)
            .UseSourceProperties(sourceProperties);

        var intermediateResults = new SingleSourceEmissionCalculationResult()
        {
            MaxConcentration = CalculateMaxConcentration(model, sourceProperties),
            DangerousDistance = CalculateDangerousDistance(model, sourceProperties),
            DangerousWindSpeed = CalculateDangerousWindSpeed(model, sourceProperties)
        };

        intermediateResults.MaxUntowardConcentrationDistance = CalculateMaxUntowardConcentrationDistance(model, sourceProperties, intermediateResults);

        using var testFile = File.Open($"C:\\Users\\krawc\\Desktop\\Test\\{reportName}", FileMode.Truncate);

        var path = Path.GetDirectoryName(typeof(SingleSourceReportModel).Assembly.Location) + @"\ReportTemplates\SingleSource\main.xml";
        await _reportManager.FromTemplate(testFile, path, _reportModelBuilder.Build());
        
        return intermediateResults;
    }

    //TODO: Do we have only 3 cases? hot, cold, low wind? 

    private double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        IMaxConcentrationCalculationSubManager? subManager;

        if ((sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5)) && sourceProperties.VmI >= 0.5)
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IColdEmissionMaxConcentrationCalculationSubManager>();
        else if (sourceProperties.F < 100 && sourceProperties.Vm < 0.5 || sourceProperties.F >= 100 && sourceProperties.VmI < 0.5)
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<ILowWindMaxConcentrationCalculationSubManager>();
        else
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IHotEmissionMaxConcentrationCalculationSubManager>();

        if (subManager == null)
            throw new InvalidOperationException("Failed to get required calculation manager");

        return subManager.CalculateMaxConcentration(model, sourceProperties); 
    }
    
    private double CalculateDangerousWindSpeed(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        IDangerousWindSpeedCalculationManager? subManager;
        if ((sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5)) && sourceProperties.VmI >= 0.5)
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IColdEmissionDangerousWindSpeedCalculationManager>();
        else if (sourceProperties.F < 100 && sourceProperties.Vm < 0.5 || sourceProperties.F >= 100 && sourceProperties.VmI < 0.5)
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<ILowWindDangerousWindSpeedCalculationManager>();
        else
            subManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IHotEmissionDangerousWindSpeedCalculationManager>();

        if (subManager == null)
            throw new InvalidOperationException("Failed to get required calculation manager");

        return subManager.CalculateDangerousWindSpeed(model, sourceProperties);
    }
    
    private double CalculateDangerousDistance(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
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

    public double CalculateMaxUntowardConcentrationDistance(SingleSourceInputModel model, EmissionSourceProperties sourceProperties, SingleSourceEmissionCalculationResult intermediateResults)
    {
        var r = GetRCoef(model, intermediateResults);

        var result = intermediateResults.MaxConcentration * r;

        return result;
    }

    private double GetRCoef(SingleSourceInputModel model, SingleSourceEmissionCalculationResult intermediateResults)
    {
        var ratio = model.U / intermediateResults.DangerousWindSpeed;
        
        double result;
        if (ratio <= 1)
        {
            result = 0.67 * ratio + 1.67 * Math.Pow(ratio, 2) - 1.34 * Math.Pow(ratio, 3);
        }
        else
        {
            result = (3 * ratio) / (2 * Math.Pow(ratio, 2) - ratio + 2);
        }

        return result;
    }

    private double GetV1(SingleSourceInputModel model)
    {
        var result =  Math.PI * Math.Pow(model.D, 2d) / 4 * model.W;

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
