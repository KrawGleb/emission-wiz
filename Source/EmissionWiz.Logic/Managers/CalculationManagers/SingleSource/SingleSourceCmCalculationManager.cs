using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource;

internal class SingleSourceCmCalculationManager : BaseManager, ISingleSourceCmCalculationManager
{
    private readonly ISingleSourceEmissionReportModelBuilder _reportModelBuilder;

    public SingleSourceCmCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder)
    {
        _reportModelBuilder = reportModelBuilder;
    }

    public double CalculateCm(SingleSourceCalculationData data, EmissionSourceProperties sourceProperties)
    {
        if ((sourceProperties.F >= 100 || (data.DeltaT >= 0 && data.DeltaT <= 0.5)) && sourceProperties.VmI >= 0.5)
        {
            var k = GetK(data, sourceProperties);
            var n = GetNCoefficient(sourceProperties.VmI);
            var numerator = data.A * data.M * data.FCoef * n * data.Eta * k;
            var denominator = Math.Pow(Math.Cbrt(data.H), 4d);

            return numerator / denominator;

        }
        else if ((sourceProperties.F < 100 && sourceProperties.Vm < 0.5) || (sourceProperties.F >= 100 && sourceProperties.VmI < 0.5))
        {
            var mi = GetMICoefficient(sourceProperties);
            var numerator = data.A * data.M * data.FCoef * mi * data.Eta;
            var denominator = Math.Pow(Math.Cbrt(data.H), 7d);

            return numerator / denominator;
        }
        else {
            var m = GetMCoefficient(sourceProperties);
            var n = GetNCoefficient(sourceProperties.Vm);
            var numerator = data.A * data.M * sourceProperties.F * m * n * data.Eta;
            var denominator = Math.Pow(data.H, 2d) * Math.Cbrt(sourceProperties.V1 * data.DeltaT);

            return numerator / denominator;
        }
    }

    private double GetNCoefficient(double vm)
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

    private double GetMCoefficient(EmissionSourceProperties sourceProperties)
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

    private double GetK(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        var result = model.D / (8d * sourceProperties.V1);

        _reportModelBuilder.SetKCoefValue(result);

        return result;
    }

}
