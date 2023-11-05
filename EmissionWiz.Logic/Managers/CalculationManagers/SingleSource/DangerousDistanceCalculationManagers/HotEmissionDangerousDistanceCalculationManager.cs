using EmissionWiz.Logic.Formulas.SingleSource.DangerousDistanceFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousDistanceCalculationManagers;

internal class HotEmissionDangerousDistanceCalculationManager : BaseDangerousDistanceCalculationManager
{
    public HotEmissionDangerousDistanceCalculationManager(ICalculationReportManager reportManager) : base(reportManager)
    {
    }

    protected override double CalculateDCoef(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var result = 5.7d * model.H;

        var reportBlock = new FormulaBlock();
        reportBlock.PushFormula(new HotEmissionDangerousDistanceFormula(), new DangerousDistanceFormula.Model
        {
            H = model.H,
            Result = result
        });

        _reportManager.AddBlock(reportBlock);

        return result;
    }
}
