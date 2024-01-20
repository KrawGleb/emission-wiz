using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationCalculationManagers;

public class ColdEmissionMaxConcentrationCalculationManager : BaseMaxConcentrationCalculationManager, IColdEmissionMaxConcentrationCalculationManager
{
    public ColdEmissionMaxConcentrationCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder) : base(reportModelBuilder)
    {
    }

    public double CalculateMaxConcentration(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        var numerator = GetNumerator(model, sourceProperties);
        var denumerator = GetDenominator(model);

        var result = numerator / denumerator;

        return result;
    }

    private double GetNumerator(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        var k = GetK(model, sourceProperties);
        var n = GetNCoefficient(sourceProperties.VmI);

        return model.A * model.M * model.FCoef * n * model.Eta * k;
    }

    private double GetDenominator(SingleSourceCalculationData model)
    {
        return Math.Pow(Math.Cbrt(model.H), 4d);
    }

    private double GetK(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        var result = model.D / (8d * sourceProperties.V1);

        _reportModelBuilder.SetKCoefValue(result);

        return result;
    }
}
