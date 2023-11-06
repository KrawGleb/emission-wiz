using EmissionWiz.Logic.Formulas.SingleSource.DangerousWindSpeedFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerouesWindSpeedCalculationManagers;

public class LowWindDangerousWindSpeedCalculationManager : ILowWindDangerousWindSpeedCalculationManager
{
    private readonly ISingleSourceEmissionReportModelBuilder _reportModelBuilder;

    public LowWindDangerousWindSpeedCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder)
    {
        _reportModelBuilder = reportModelBuilder;
    }

    public double CalculateDangerousWindSpeed(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        double result;

        if (sourceProperties.Vm <= 0.5)
        {
            result = 0.5;
        }
        else if (sourceProperties.Vm <= 2)
        {
            result = sourceProperties.Vm;
        }
        else
        {
            result = sourceProperties.Vm * (1 + 0.12d * Math.Sqrt(sourceProperties.F));
        }

        return result;
    }
}
