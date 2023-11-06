using EmissionWiz.Logic.Formulas.SingleSource.DangerousDistanceFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousDistanceCalculationManagers;

public class HotEmissionDangerousDistanceCalculationManager : BaseDangerousDistanceCalculationManager, IHotEmissionDangerousDistanceCalculationManager
{
    public HotEmissionDangerousDistanceCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder) : base(reportModelBuilder)
    {
    }

    protected override double CalculateDCoef(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var result = 5.7d * model.H;

        return result;
    }
}
