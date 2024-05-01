using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource;

internal class SingleSourceXmCalculationManager : BaseManager, ISingleSourceXmCalculationManager
{
    private readonly ISingleSourceEmissionReportModelBuilder _reportModelBuilder;

    public SingleSourceXmCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder)
    {
        _reportModelBuilder = reportModelBuilder;
    }

    public double CalculateXm(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        var d = CalculateDCoef(model, sourceProperties);
        var result = ((5 - model.FCoef) / 4d) * d * model.H;

        _reportModelBuilder.SetDCoefValue(d);

        return result;
    }

    private double CalculateDCoef(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        if (sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5))
        {
            double result;

            if (sourceProperties.VmI <= 0.5)
            {
                result = 5.7;
            }
            else if (sourceProperties.VmI <= 2)
            {
                result = 11.4d * sourceProperties.VmI;
            }
            else
            {
                result = 16d * Math.Sqrt(sourceProperties.VmI);
            }

            return result;
        }
        else if (sourceProperties.F < 100)
        {
            double result;
            if (sourceProperties.Vm <= 0.5)
            {
                result = 2.48d * (1 + 0.28d * Math.Cbrt(sourceProperties.Fe));
            }
            else if (sourceProperties.Vm <= 2)
            {
                result = 4.95 * sourceProperties.Vm * (1 + 0.28d * Math.Cbrt(sourceProperties.F));
            }
            else
            {
                result = 7 * Math.Sqrt(sourceProperties.Vm) * (1 + 0.28d * Math.Cbrt(sourceProperties.F));
            }

            return result;
        }
        else
        {
            var result = 5.7d * model.H;

            return result;
        }
    }
}
