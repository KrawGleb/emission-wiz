using EmissionWiz.Models.Calculations.SingleSource;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerouesWindSpeedCalculationManagers;

public class ColdEmissionDangerousWindSpeedCalculationManager : IColdEmissionDangerousWindSpeedCalculationManager
{
    public double CalculateDangerousWindSpeed(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        double result;

        if (sourceProperties.VmI <= 0.5)
        {
            result = 0.5;
        }
        else if (sourceProperties.VmI <= 2)
        {
            result = sourceProperties.VmI;
        }
        else
        {
            result = 2.2 * sourceProperties.VmI;
        }

        return result;
    }
}
