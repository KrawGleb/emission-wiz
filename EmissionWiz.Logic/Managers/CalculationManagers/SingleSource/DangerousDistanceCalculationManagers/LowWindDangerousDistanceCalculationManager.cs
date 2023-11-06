using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousDistanceCalculationManagers;

public class LowWindDangerousDistanceCalculationManager : BaseDangerousDistanceCalculationManager, ILowWindDangerousDistanceCalculationManager
{
    public LowWindDangerousDistanceCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder) : base(reportModelBuilder)
    {
    }

    protected override double CalculateDCoef(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
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
}
