namespace EmissionWiz.Models.Interfaces.Managers;

public interface IGeoTiffManager : IBaseManager
{
    void BuildTiffFromFlatArray(double[] source);
}