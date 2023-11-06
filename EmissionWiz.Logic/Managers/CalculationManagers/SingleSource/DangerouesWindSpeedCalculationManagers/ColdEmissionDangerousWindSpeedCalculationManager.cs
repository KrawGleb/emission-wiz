using EmissionWiz.Logic.Formulas.SingleSource.DangerousWindSpeedFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerouesWindSpeedCalculationManagers;

public class ColdEmissionDangerousWindSpeedCalculationManager : IColdEmissionDangerousWindSpeedCalculationManager
{
    private readonly ISingleSourceEmissionReportModelBuilder _reportModelBuilder;

    public ColdEmissionDangerousWindSpeedCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder)
    {
        _reportModelBuilder = reportModelBuilder;
    }

    public double CalculateDangerousWindSpeed(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        double result;

        if (sourceProperties.VmI <= 0.5)
        {
            result = 0.5;
        }
        else if (sourceProperties.VmI <= 2)
        {
            result = sourceProperties.VmI;
        }
        else
        {
            result = 2.2 * sourceProperties.VmI;
        }

        return result;
    }
}
