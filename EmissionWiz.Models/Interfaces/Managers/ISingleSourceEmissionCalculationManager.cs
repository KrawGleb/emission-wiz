using EmissionWiz.Models.Calculations.SingleSource;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface ISingleSourceEmissionCalculationManager : IBaseManager
{
    Task<SingleSourceEmissionCalculationResult> Calculate(SingleSourceCalculationData calculationData);
}

public interface IMaxConcentrationCalculationManager : IBaseManager
{
    double CalculateMaxConcentration(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties);
}

public interface IColdEmissionMaxConcentrationCalculationManager : IMaxConcentrationCalculationManager { }
public interface IHotEmissionMaxConcentrationCalculationManager : IMaxConcentrationCalculationManager { }
public interface ILowWindMaxConcentrationCalculationManager : IMaxConcentrationCalculationManager { }

public interface IDangerousDistanceCalculationManager : IBaseManager
{
    double CalculateDangerousDistance(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties);
}

public interface IColdEmissionDangerousDistanceCalculationManager : IDangerousDistanceCalculationManager { }
public interface IHotEmissionDangerousDistanceCalculationManager : IDangerousDistanceCalculationManager { }
public interface ILowWindDangerousDistanceCalculationManager : IDangerousDistanceCalculationManager { }

public interface IDangerousWindSpeedCalculationManager
{
    double CalculateDangerousWindSpeed(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties);
}

public interface IColdEmissionDangerousWindSpeedCalculationManager : IDangerousWindSpeedCalculationManager { }
public interface IHotEmissionDangerousWindSpeedCalculationManager : IDangerousWindSpeedCalculationManager { }
public interface ILowWindDangerousWindSpeedCalculationManager : IDangerousWindSpeedCalculationManager { }