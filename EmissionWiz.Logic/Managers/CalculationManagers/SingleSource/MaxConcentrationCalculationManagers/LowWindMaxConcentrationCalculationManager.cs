using EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;
using static EmissionWiz.Models.Constants;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationCalculationManagers;

internal class LowWindMaxConcentrationCalculationManager : BaseMaxConcentrationCalculationManager, IMaxConcentrationCalculationSubManager
{
    private readonly ICalculationReportManager _reportManager;

    public LowWindMaxConcentrationCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    public double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var formulaModel = new LowWindCFormula.Model
        {
            A = model.A,
            Eta = model.Eta,
            F = model.F,
            H = model.H,
            M = model.M,
        };

        var reportBlock = new FormulaBlock()
        {
            Comment = $"При {MathChars.f} < 100 и v{MathChars.Lower.m} < 0,5 или {MathChars.f} {MathChars.GoE} 100 и v{MathChars.Lower.m}\' < 0,5 (случаи предельно малых опасных скоростей ветра) расчет c{MathChars.Lower.m} производится по формуле:"
        };

        var numerator = GetNumeratort(model, sourceProperties, formulaModel, reportBlock);
        var denominator = GetDenomerator(model);

        var result = numerator / denominator;
        formulaModel.Result = result;
       
        reportBlock.PushFormula(new LowWindCFormula(), formulaModel);
        _reportManager.AddBlock(reportBlock);

        return result;
    }

    private double GetNumeratort(SingleSourceInputModel model, EmissionSourceProperties sourceProperties, LowWindCFormula.Model formulaModel, FormulaBlock block)
    {
        var m = GetMICoefficient(sourceProperties, block);

        formulaModel.Mi = m;

        return model.A * model.M * model.F * m * model.Eta;
    }

    private double GetDenomerator(SingleSourceInputModel model)
    {
        return Math.Pow(Math.Cbrt(model.H), 7d);
    }

    private double GetMICoefficient(EmissionSourceProperties sourceProperties, FormulaBlock block)
    {
        var (m, _) = GetMCoefficient(sourceProperties);

        double result;
        if (sourceProperties.Vm < 0.5)
            result = 2.86 * m;
        else
            result = 0.9;

        block.PushFormula(new MIFormula(sourceProperties.Vm), new MIFormula.Model
        {
            MCoef = m,
            Result = result
        });

        return result;
    }
}
