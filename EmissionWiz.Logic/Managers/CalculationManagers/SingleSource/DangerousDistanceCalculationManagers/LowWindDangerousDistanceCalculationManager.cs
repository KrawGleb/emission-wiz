using EmissionWiz.Logic.Formulas.SingleSource.DangerousDistanceFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousDistanceCalculationManagers;

internal class LowWindDangerousDistanceCalculationManager : BaseDangerousDistanceCalculationManager
{
    public LowWindDangerousDistanceCalculationManager(ICalculationReportManager reportManager) : base(reportManager)
    {
    }

    protected override double CalculateDCoef(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        double result;
        if (sourceProperties.Vm <= 0.5)
        {
            result = 2.48d * (1 + 0.28d * Math.Cbrt(sourceProperties.Fe));
        }
        else if (sourceProperties.Vm <= 2)
        {
            result = 4.95 * sourceProperties.Vm * (1 + 0.28d * Math.Cbrt(sourceProperties.F));
        }
        else
        {
            result = 7 * Math.Sqrt(sourceProperties.Vm) * (1 + 0.28d * Math.Cbrt(sourceProperties.F));
        }

        var reportBlock = new FormulaBlock();
        reportBlock.PushFormula(new LowWindDCoefFormula(sourceProperties.Vm), new LowWindDCoefFormula.Model
        {
            F = sourceProperties.F,
            Fe = sourceProperties.Fe,
            Vm = sourceProperties.Vm,
            Result = result
        });
        _reportManager.AddBlock(reportBlock);

        return result;
    }
}
