using EmissionWiz.Models.Dto;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface IMapManager : IBaseManager
{
    Task<Stream> GetTileAsync(MapTileOptions options);
}
