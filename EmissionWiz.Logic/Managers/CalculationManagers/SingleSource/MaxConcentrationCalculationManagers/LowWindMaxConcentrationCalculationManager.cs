using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationCalculationManagers;

public class LowWindMaxConcentrationCalculationManager : BaseMaxConcentrationCalculationManager, ILowWindMaxConcentrationCalculationManager
{
    public LowWindMaxConcentrationCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder) : base(reportModelBuilder)
    {
    }

    public double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var numerator = GetNumeratort(model, sourceProperties);
        var denominator = GetDenomerator(model);

        var result = numerator / denominator;

        return result;
    }

    private double GetNumeratort(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var mi = GetMICoefficient(sourceProperties);

        return model.A * model.M * model.FCoef * mi * model.Eta;
    }

    private double GetDenomerator(SingleSourceInputModel model)
    {
        return Math.Pow(Math.Cbrt(model.H), 7d);
    }

    private double GetMICoefficient(EmissionSourceProperties sourceProperties)
    {
        var m = GetMCoefficient(sourceProperties);

        double result;
        if (sourceProperties.Vm < 0.5)
            result = 2.86 * m;
        else if (sourceProperties.F >= 100 && sourceProperties.VmI < 0.5)
            result = 0.9;
        else
            throw new InvalidOperationException("Cannot calculate mi coef");

        _reportModelBuilder.SetMICoefValue(result);

        return result;
    }
}
