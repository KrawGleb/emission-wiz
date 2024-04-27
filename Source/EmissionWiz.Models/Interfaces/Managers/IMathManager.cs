using EmissionWiz.Models.Dto;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface IMathManager : IBaseManager
{
    SplineData Spline(double[] xs, double[] ys, int count);
}
