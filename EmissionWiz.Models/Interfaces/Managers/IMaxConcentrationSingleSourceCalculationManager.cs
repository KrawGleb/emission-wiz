using EmissionWiz.Models.Calculations;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface IMaxConcentrationSingleSourceCalculationManager
{
    double CalculateMaxConcentration(MaxConcentrationInputModel model);
}

public interface IMaxConcentrationSingleSourceCalculationSubManager
{
    double CalculateMaxConcentration(MaxConcentrationInputModel model, EmissionSourceProperties sourceProperties);
}