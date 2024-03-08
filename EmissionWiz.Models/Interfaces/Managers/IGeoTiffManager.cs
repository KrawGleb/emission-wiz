using EmissionWiz.Models.Dto;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface IGeoTiffManager : IBaseManager
{
    void GenerateGeoTiff(GeoTiffOptions options);
}