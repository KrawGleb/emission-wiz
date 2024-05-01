using EmissionWiz.Models.Calculations.SingleSource;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface ISingleSourceGeoTiffManager : IBaseManager
{
    Task<SingleSourceGeoTiffResult> BuildGeoTiff(SingleSourceEmissionCalculationResult results, SingleSourceCalculationData data, SingleSourceGeoTiffOptions options);
}
