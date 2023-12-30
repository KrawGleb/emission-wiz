using EmissionWiz.Models.Calculations.SingleSource;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface ISingleSourceEmissionCalculationManager : IBaseManager
{
    Task<(SingleSourceEmissionCalculationResult, Stream)> Calculate(SingleSourceInputModel model, string reportName);
}

public interface IMaxConcentrationCalculationManager : IBaseManager
{
    double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties);
}

public interface IColdEmissionMaxConcentrationCalculationManager : IMaxConcentrationCalculationManager { }
public interface IHotEmissionMaxConcentrationCalculationManager : IMaxConcentrationCalculationManager { }
public interface ILowWindMaxConcentrationCalculationManager : IMaxConcentrationCalculationManager { }

public interface IDangerousDistanceCalculationManager : IBaseManager
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