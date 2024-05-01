using EmissionWiz.Models.Calculations.SingleSource;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface ISingleSourceEmissionCalculationManager : IBaseManager
{
    Task<SingleSourceEmissionCalculationResult> Calculate(SingleSourceCalculationData calculationData);
}

public interface ISingleSourceCmCalculationManager : IBaseManager
{
    double CalculateCm(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties);
}

public interface ISingleSourceXmCalculationManager : IBaseManager
{
    double CalculateXm(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties);
}

public interface ISingleSourceUmCalculationManager
{
    double CalculateUm(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties);
}
