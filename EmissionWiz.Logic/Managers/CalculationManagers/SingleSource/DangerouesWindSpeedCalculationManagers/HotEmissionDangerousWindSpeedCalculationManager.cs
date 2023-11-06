using EmissionWiz.Models.Calculations.SingleSource;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerouesWindSpeedCalculationManagers;

public class HotEmissionDangerousWindSpeedCalculationManager : IHotEmissionDangerousWindSpeedCalculationManager
{
    public double CalculateDangerousWindSpeed(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        return 0.5;
    }
}
