using EmissionWiz.Models.Calculations;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;

internal class HotEmissionMaxConcentrationSingleSourceCalculationManager : BaseMaxConcentrationSingleSourceCalculationManager, IMaxConcentrationSingleSourceCalculationSubManager
{
    private readonly ICalculationReportManager _reportManager;

    public HotEmissionMaxConcentrationSingleSourceCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    public double CalculateMaxConcentration(MaxConcentrationInputModel model, EmissionSourceProperties sourceProperties)
    {
        var numerator = GetNumerator(model, sourceProperties);
        var denominator = GetDenominator(model, sourceProperties);

        return numerator / denominator;
    }

    private double GetNumerator(MaxConcentrationInputModel model, EmissionSourceProperties sourceProperties)
    {
        var m = GetMCoefficient(sourceProperties);
        var n = GetNCoefficient(sourceProperties.Vm);

        return model.A * model.M * m * n * model.N;
    }

    private double GetDenominator(MaxConcentrationInputModel model, EmissionSourceProperties sourceProperties)
    {
        return Math.Pow(model.H, 2d) * Math.Cbrt(sourceProperties.V * model.DeltaT);
    }
}
