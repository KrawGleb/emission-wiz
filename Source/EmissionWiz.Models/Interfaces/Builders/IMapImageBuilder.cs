using EmissionWiz.Models.Map.Shapes;

namespace EmissionWiz.Models.Interfaces.Builders;

public interface IMapImageBuilder
{
    void AddMarker(Marker marker);
    void DrawShape(Shape shape);
    Task<(Stream?, Dictionary<string, string>)> PrintAsync();
}