using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerouesWindSpeedCalculationManagers;

public class HotEmissionDangerousWindSpeedCalculationManager : IHotEmissionDangerousWindSpeedCalculationManager
{

    public HotEmissionDangerousWindSpeedCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder)
    {
        ReportModelBuilder = reportModelBuilder;
    }

    public ISingleSourceEmissionReportModelBuilder ReportModelBuilder { get; }

    public double CalculateDangerousWindSpeed(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        return 0.5;
    }
}
