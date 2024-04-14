using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousWindSpeedCalculationManagers;

public class HotEmissionDangerousWindSpeedCalculationManager : BaseManager, IHotEmissionDangerousWindSpeedCalculationManager
{
    public double CalculateDangerousWindSpeed(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        return 0.5;
    }
}
