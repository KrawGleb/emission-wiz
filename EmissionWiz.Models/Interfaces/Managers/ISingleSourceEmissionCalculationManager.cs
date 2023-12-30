
using EmissionWiz.Models.Calculations.SingleSource;

public interface ISingleSourceEmissionCalculationManager
{
    Task<(SingleSourceEmissionCalculationResult, Stream)> Calculate(SingleSourceInputModel model, string reportName);
}


public interface IMaxConcentrationCalculationSubManager
{
    double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties);
}

public interface IColdEmissionMaxConcentrationCalculationSubManager : IMaxConcentrationCalculationSubManager { }
public interface IHotEmissionMaxConcentrationCalculationSubManager : IMaxConcentrationCalculationSubManager { }
public interface ILowWindMaxConcentrationCalculationSubManager : IMaxConcentrationCalculationSubManager { }

public interface IDangerousDistanceCalculationManager
{
    double CalculateDangerousDistance(SingleSourceInputModel model, EmissionSourceProperties sourceProperties);
}

public interface IColdEmissionDangerousDistanceCalculationManager : IDangerousDistanceCalculationManager { }
public interface IHotEmissionDangerousDistanceCalculationManager : IDangerousDistanceCalculationManager { }
public interface ILowWindDangerousDistanceCalculationManager : IDangerousDistanceCalculationManager { }

public interface IDangerousWindSpeedCalculationManager
{
    double CalculateDangerousWindSpeed(SingleSourceInputModel model, EmissionSourceProperties sourceProperties);
}
public interface IColdEmissionDangerousWindSpeedCalculationManager : IDangerousWindSpeedCalculationManager { }
public interface IHotEmissionDangerousWindSpeedCalculationManager : IDangerousWindSpeedCalculationManager { }
public interface ILowWindDangerousWindSpeedCalculationManager : IDangerousWindSpeedCalculationManager { }