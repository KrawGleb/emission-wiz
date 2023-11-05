using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousDistanceCalculationManagers;

internal class BaseDangerousDistanceCalculationManager : IDangerousDistanceCalculationManager
{
    protected readonly ICalculationReportManager _reportManager;

    public BaseDangerousDistanceCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    public virtual double CalculateDangerousDistance(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var d = CalculateDCoef(model, sourceProperties);
        var result = ((5 - model.F) / 4d) * d * model.H;

        return result;
    }

    protected virtual double CalculateDCoef(SingleSourceInputModel model, EmissionSourceProperties sourceProperties) => 0;
}
