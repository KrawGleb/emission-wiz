using EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationCalculationManagers;

internal class HotEmissionMaxConcentrationCalculationManager : BaseMaxConcentrationCalculationManager, IMaxConcentrationCalculationSubManager
{
    private readonly ICalculationReportManager _reportManager;

    public HotEmissionMaxConcentrationCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    public double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var formulaModel = new HotEmissionCFormula.Model
        {
            A = model.A,
            DeltaT = model.DeltaT,
            F = model.F,
            H = model.H,
            M = model.M,
            V1 = sourceProperties.V,
            eta = model.Eta,
        };

        var numerator = GetNumerator(model, sourceProperties, formulaModel);
        var denominator = GetDenominator(model, sourceProperties);

        var result = numerator / denominator;

        formulaModel.Result = result;

        var reportBlock = new FormulaBlock();
        reportBlock.PushFormula(new HotEmissionCFormula(), formulaModel);
        _reportManager.AddBlock(reportBlock);

        return result;
    }

    private double GetNumerator(SingleSourceInputModel model, EmissionSourceProperties sourceProperties, HotEmissionCFormula.Model formulaModel)
    {
        var (m, mReportBlock) = GetMCoefficient(sourceProperties);
        var (n, nReportBlock) = GetNCoefficient(sourceProperties.Vm);

        _reportManager.AddBlock(mReportBlock);
        _reportManager.AddBlock(nReportBlock);

        formulaModel.m = m;
        formulaModel.n = n;

        return model.A * model.M * m * n * model.Eta;
    }

    private double GetDenominator(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        return Math.Pow(model.H, 2d) * Math.Cbrt(sourceProperties.V * model.DeltaT);
    }
}
