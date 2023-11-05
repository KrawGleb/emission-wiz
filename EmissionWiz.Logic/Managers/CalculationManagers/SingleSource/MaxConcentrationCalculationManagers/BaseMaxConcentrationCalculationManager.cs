using EmissionWiz.Logic.Formulas.MaxConcentrationSingleSourceFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationCalculationManagers;

internal abstract class BaseMaxConcentrationCalculationManager
{
    protected (double, FormulaBlock) GetNCoefficient(double vm)
    {
        double result;
        if (vm < 0.5d)
        {
            result = 4.4d * vm;
        }
        else if (vm < 2)
        {
            result = 0.532 * Math.Pow(vm, 2d) - 2.13 * vm + 3.13;
        }
        else
        {
            result = 1;
        }

        var reportBlock = new FormulaBlock();
        reportBlock.PushFormula(new NCoefFormula(vm), new NCoefFormula.Model
        {
            Vm = vm,
            Result = result
        });

        return (result, reportBlock);
    }

    protected (double, FormulaBlock) GetMCoefficient(EmissionSourceProperties sourceProperties)
    {
        var f = sourceProperties.F;

        if (sourceProperties.Fe < sourceProperties.F && sourceProperties.F < 100)
            f = sourceProperties.Fe;

        var result = f < 100
            ? Math.Pow(0.67d + 0.1 * Math.Sqrt(f) + 0.34 * Math.Cbrt(f), -1d)
            : 1.47d / Math.Cbrt(f);

        var reportBlock = new FormulaBlock();
        reportBlock.PushFormula(new MCoefFormula(f), new MCoefFormula.Model
        {
            F = f,
            Result = result
        });

        return (result, reportBlock);
    }
}
