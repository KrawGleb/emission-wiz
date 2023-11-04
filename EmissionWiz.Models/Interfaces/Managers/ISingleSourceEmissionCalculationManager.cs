
using EmissionWiz.Models.Calculations.SingleSource;

public interface ISingleSourceEmissionCalculationManager
{
    SingleSourceEmissionCalculationResult Calculate(SingleSourceInputModel model);
}


public interface IMaxConcentrationSingleSourceCalculationSubManager
{
    double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties);
}