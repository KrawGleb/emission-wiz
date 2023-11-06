using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousDistanceCalculationManagers;

public class BaseDangerousDistanceCalculationManager : IDangerousDistanceCalculationManager
{
    protected readonly ISingleSourceEmissionReportModelBuilder _reportModelBuilder;

    public BaseDangerousDistanceCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder)
    {
        _reportModelBuilder = reportModelBuilder;
    }

    public virtual double CalculateDangerousDistance(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var d = CalculateDCoef(model, sourceProperties);
        var result = ((5 - model.FCoef) / 4d) * d * model.H;

        _reportModelBuilder
            .SetDCoefValue(d)
            .SetXmValue(result);

        return result;
    }

    protected virtual double CalculateDCoef(SingleSourceInputModel model, EmissionSourceProperties sourceProperties) => 0;
}
