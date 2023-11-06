using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationCalculationManagers;

public abstract class BaseMaxConcentrationCalculationManager
{
    protected readonly ISingleSourceEmissionReportModelBuilder _reportModelBuilder;

    public BaseMaxConcentrationCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder)
    {
        _reportModelBuilder = reportModelBuilder;
    }

    protected double GetNCoefficient(double vm)
    {
        double result;
        if (vm < 0.5d)
        {
            result = 4.4d * vm;
        }
        else if (vm < 2)
        {
            result = 0.532 * Math.Pow(vm, 2d) - 2.13 * vm + 3.13;
        }
        else
        {
            result = 1;
        }

        _reportModelBuilder.SetNCoefValue(result);

        return result;
    }

    protected double GetMCoefficient(EmissionSourceProperties sourceProperties)
    {
        var f = sourceProperties.F;

        if (sourceProperties.Fe < sourceProperties.F && sourceProperties.F < 100)
            f = sourceProperties.Fe;

        var result = f < 100
            ? Math.Pow(0.67d + 0.1 * Math.Sqrt(f) + 0.34 * Math.Cbrt(f), -1d)
            : 1.47d / Math.Cbrt(f);

        _reportModelBuilder.SetMCoefValue(result);
        
        return result;
    }
}
