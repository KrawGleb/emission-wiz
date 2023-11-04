using EmissionWiz.Models.Reports.Areas;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface ICalculationReportManager
{
    void Generate(Stream destination);
    void AddBlock(BaseBlock block);
}
