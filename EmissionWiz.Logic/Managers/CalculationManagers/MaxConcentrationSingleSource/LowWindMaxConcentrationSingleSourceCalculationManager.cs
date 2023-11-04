using EmissionWiz.Models.Calculations;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;

internal class LowWindMaxConcentrationSingleSourceCalculationManager : BaseMaxConcentrationSingleSourceCalculationManager, IMaxConcentrationSingleSourceCalculationSubManager
{
    private readonly ICalculationReportManager _reportManager;

    public LowWindMaxConcentrationSingleSourceCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    public double CalculateMaxConcentration(MaxConcentrationInputModel model, EmissionSourceProperties sourceProperties)
    {
        var numerator = GetNumeratort(model, sourceProperties);
        var denominator = GetDenomerator(model);

        return numerator / denominator;
    }

    private double GetNumeratort(MaxConcentrationInputModel model, EmissionSourceProperties sourceProperties)
    {
        var m = GetMICoefficient(sourceProperties);

        return model.A * model.M * model.F * m * model.N;
    }

    private double GetDenomerator(MaxConcentrationInputModel model)
    {
        return Math.Pow(Math.Cbrt(model.H), 7d);
    }

    private double GetMICoefficient(EmissionSourceProperties sourceProperties)
    {
        if (sourceProperties.Vm < 0.5)
            return 2.86 * GetMCoefficient(sourceProperties);
        else
            return 0.9;
    }
}
