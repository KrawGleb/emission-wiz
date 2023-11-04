using EmissionWiz.Models.Calculations;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;

internal class ColdEmissionMaxConcentrationSingleSourceCalculationManager : BaseMaxConcentrationSingleSourceCalculationManager, IMaxConcentrationSingleSourceCalculationSubManager
{
    private readonly ICalculationReportManager _reportManager;

    public ColdEmissionMaxConcentrationSingleSourceCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    public double CalculateMaxConcentration(MaxConcentrationInputModel model, EmissionSourceProperties sourceProperties)
    {
        var numerator = GetNumerator(model, sourceProperties);
        var denumerator = GetDenominator(model);

        return numerator / denumerator;
    }

    private double GetNumerator(MaxConcentrationInputModel model, EmissionSourceProperties sourceProperties)
    {
        var k = GetK(model, sourceProperties);
        var n = GetNCoefficient(sourceProperties.VmI);

        return model.A * model.M * model.F * n * model.N * k;
    }

    private double GetDenominator(MaxConcentrationInputModel model)
    {
        return Math.Pow(Math.Sqrt(model.H), 4d);
    }

    private double GetK(MaxConcentrationInputModel model, EmissionSourceProperties sourceProperties)
    {
        return model.D / (8d * sourceProperties.V);
    }
}
