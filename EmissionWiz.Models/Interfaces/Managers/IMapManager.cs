using EmissionWiz.Models.Map.Shapes;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface IMapManager : IBaseManager
{
    void AddMarker(Marker marker);
    void DrawShape(Shape shape);
    Task<(Stream?, Dictionary<string, string>)> PrintAsync();
}
