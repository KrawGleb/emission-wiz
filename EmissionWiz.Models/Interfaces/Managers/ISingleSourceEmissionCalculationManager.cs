
using EmissionWiz.Models.Calculations.SingleSource;

public interface ISingleSourceEmissionCalculationManager
{
    SingleSourceEmissionCalculationResult Calculate(SingleSourceInputModel model, string reportName);
}


public interface IMaxConcentrationCalculationSubManager
{
    double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties);
}

public interface IDangerousDistanceCalculationManager
{
    double CalculateDangerousDistance(SingleSourceInputModel model, EmissionSourceProperties sourceProperties);
}

public interface IDangerousWindSpeedCalculationManager
{
    double CalculateDangerousWindSpeed(SingleSourceInputModel model, EmissionSourceProperties sourceProperties);
}