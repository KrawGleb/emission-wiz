using EmissionWiz.Logic.Formulas.SingleSource.DangerousDistanceFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

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

        var reportBlock = new FormulaBlock();
        reportBlock.PushFormula(new DangerousDistanceFormula(), new DangerousDistanceFormula.Model
        {
            DCoef = d,
            F = model.F,
            H = model.H,
            Result = result
        });

        _reportManager.AddBlock(reportBlock);

        return result;
    }

    protected virtual double CalculateDCoef(SingleSourceInputModel model, EmissionSourceProperties sourceProperties) => 0;
}
