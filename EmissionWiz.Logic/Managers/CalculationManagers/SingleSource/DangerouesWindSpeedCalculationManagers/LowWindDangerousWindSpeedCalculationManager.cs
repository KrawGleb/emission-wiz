using EmissionWiz.Models.Calculations.SingleSource;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerouesWindSpeedCalculationManagers;

public class LowWindDangerousWindSpeedCalculationManager : ILowWindDangerousWindSpeedCalculationManager
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
