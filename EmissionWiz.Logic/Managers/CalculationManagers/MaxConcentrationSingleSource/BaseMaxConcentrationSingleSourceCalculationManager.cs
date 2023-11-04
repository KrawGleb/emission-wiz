using EmissionWiz.Models.Calculations;

namespace EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;

internal abstract class BaseMaxConcentrationSingleSourceCalculationManager
{
    protected double GetNCoefficient(double vm)
    {
        if (vm < 0.5d)
        {
            return 4.4d * vm;
        }
        else if (vm < 2)
        {
            return 0.532 * Math.Pow(vm, 2d) - 2.13 * vm + 3.13;
        }
        else
        {
            return 1;
        }
    }

    protected double GetMCoefficient(EmissionSourceProperties sourceProperties)
    {
        var f = sourceProperties.F;

        if (sourceProperties.Fe < sourceProperties.F && sourceProperties.F < 100)
            f = sourceProperties.Fe;

        return f < 100
            ? Math.Pow(0.67d + 0.1 * Math.Sqrt(f) + 0.34 * Math.Cbrt(f), -1d)
            : 1.47d / Math.Cbrt(f);
    }
}
