using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationCalculationManagers;

public class HotEmissionMaxConcentrationCalculationManager : BaseMaxConcentrationCalculationManager, IHotEmissionMaxConcentrationCalculationManager
{

    public HotEmissionMaxConcentrationCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder
        ) : base(reportModelBuilder)
    {
    }

    public double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var numerator = GetNumerator(model, sourceProperties);
        var denominator = GetDenominator(model, sourceProperties);

        var result = numerator / denominator;

        return result;
    }

    private double GetNumerator(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var m = GetMCoefficient(sourceProperties);
        var n = GetNCoefficient(sourceProperties.Vm);

        return model.A * model.M * m * n * model.Eta;
    }

    private double GetDenominator(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        return Math.Pow(model.H, 2d) * Math.Cbrt(sourceProperties.V1 * model.DeltaT);
    }
}
