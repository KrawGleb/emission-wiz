using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousWindSpeedCalculationManagers;

public class LowWindDangerousWindSpeedCalculationManager : BaseManager, ILowWindDangerousWindSpeedCalculationManager
{
    public double CalculateDangerousWindSpeed(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        double result;

        if (sourceProperties.Vm <= 0.5)
        {
            result = 0.5;
        }
        else if (sourceProperties.Vm <= 2)
        {
            result = sourceProperties.Vm;
        }
        else
        {
            result = sourceProperties.Vm * (1 + 0.12d * Math.Sqrt(sourceProperties.F));
        }

        return result;
    }
}
