using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationSingleSourceCalculationManagers;

internal class LowWindMaxConcentrationSingleSourceCalculationManager : BaseMaxConcentrationSingleSourceCalculationManager, IMaxConcentrationSingleSourceCalculationSubManager
{
    private readonly ICalculationReportManager _reportManager;

    public LowWindMaxConcentrationSingleSourceCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    public double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var numerator = GetNumeratort(model, sourceProperties);
        var denominator = GetDenomerator(model);

        return numerator / denominator;
    }

    private double GetNumeratort(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var m = GetMICoefficient(sourceProperties);

        return model.A * model.M * model.F * m * model.Eta;
    }

    private double GetDenomerator(SingleSourceInputModel model)
    {
        return Math.Pow(Math.Cbrt(model.H), 7d);
    }

    private double GetMICoefficient(EmissionSourceProperties sourceProperties)
    {
        var (m, _) = GetMCoefficient(sourceProperties);
        if (sourceProperties.Vm < 0.5)
            return 2.86 * m;
        else
            return 0.9;
    }
}
