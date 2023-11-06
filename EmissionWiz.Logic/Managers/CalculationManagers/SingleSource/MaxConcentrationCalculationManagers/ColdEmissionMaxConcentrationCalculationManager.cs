using EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;
using static EmissionWiz.Common.Constants;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationCalculationManagers;

internal class ColdEmissionMaxConcentrationCalculationManager : BaseMaxConcentrationCalculationManager, IMaxConcentrationCalculationSubManager
{
    private readonly ICalculationReportManager _reportManager;

    public ColdEmissionMaxConcentrationCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    public double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var formulaModel = new ColdEmissionCFormula.Model
        {
            A = model.A,
            Eta = model.Eta,
            F = model.F,
            M = model.M,
            H = model.H
        };
        var reportBlock = new FormulaBlock();
        reportBlock.Comment = "Для {{math f}} {{math GoE}} 100 (или 0 {{math LoE}} {{math Delta}} T {{math LoE}} 0.5) и v{{math Lower|m}}\' {{math GoE}} 0.5 (холодные выбросы) при расчете c{{math Lower|m}} используется формула: ";

        var numerator = GetNumerator(model, sourceProperties, formulaModel, reportBlock);
        var denumerator = GetDenominator(model);

        var result = numerator / denumerator;

        reportBlock.PushFormula(new ColdEmissionCFormula(), formulaModel);
        _reportManager.AddBlock(reportBlock);

        return result;
    }

    private double GetNumerator(SingleSourceInputModel model, EmissionSourceProperties sourceProperties, ColdEmissionCFormula.Model formulaModel, FormulaBlock block)
    {
        var (k, kBlock) = GetK(model, sourceProperties);
        var (n, nBlock) = GetNCoefficient(sourceProperties.VmI, true);

        nBlock.Comment = "Коэффициент n определяется при v{{math Lower|m}} = v{{math Lower|m}}\'.";

        block.PushBlock(nBlock);
        block.PushBlock(kBlock);

        formulaModel.KCoef = k;
        formulaModel.NCoef = n;

        return model.A * model.M * model.F * n * model.Eta * k;
    }

    private double GetDenominator(SingleSourceInputModel model)
    {
        return Math.Pow(Math.Sqrt(model.H), 4d);
    }

    private (double, FormulaBlock) GetK(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var result = model.D / (8d * sourceProperties.V);

        var reportBlock = new FormulaBlock();
        reportBlock.PushFormula(new KCoefFormula(), new KCoefFormula.Model
        {
            D = model.D,
            V1 = sourceProperties.V,
            Result = result
        });

        return (result, reportBlock);
    }

    private (double, FormulaBlock) GetSpecialNCoefficient(double vm)
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
}
