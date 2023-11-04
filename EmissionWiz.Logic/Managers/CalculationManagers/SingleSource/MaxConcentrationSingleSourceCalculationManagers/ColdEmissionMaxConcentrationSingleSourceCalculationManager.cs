using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationSingleSourceCalculationManagers;

internal class ColdEmissionMaxConcentrationSingleSourceCalculationManager : BaseMaxConcentrationSingleSourceCalculationManager, IMaxConcentrationSingleSourceCalculationSubManager
{
    private readonly ICalculationReportManager _reportManager;

    public ColdEmissionMaxConcentrationSingleSourceCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    public double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var numerator = GetNumerator(model, sourceProperties);
        var denumerator = GetDenominator(model);

        return numerator / denumerator;
    }

    private double GetNumerator(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var k = GetK(model, sourceProperties);
        var (n, _) = GetNCoefficient(sourceProperties.VmI);

        return model.A * model.M * model.F * n * model.Eta * k;
    }

    private double GetDenominator(SingleSourceInputModel model)
    {
        return Math.Pow(Math.Sqrt(model.H), 4d);
    }

    private double GetK(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        return model.D / (8d * sourceProperties.V);
    }
}
