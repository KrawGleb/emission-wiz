using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousDistanceCalculationManagers;

internal class HotEmissionDangerousDistanceCalculationManager : BaseDangerousDistanceCalculationManager
{
    public HotEmissionDangerousDistanceCalculationManager(ICalculationReportManager reportManager) : base(reportManager)
    {
    }

    protected override double CalculateDCoef(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var result = 5.7d * model.H;

        return result;
    }
}
