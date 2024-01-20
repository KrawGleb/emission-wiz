using EmissionWiz.Models.Map.Shapes;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface IMapManager : IBaseManager
{
    void DrawShape(Shape shape);
    Task<Stream?> PrintAsync();
}
