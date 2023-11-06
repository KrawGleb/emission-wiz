using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousDistanceCalculationManagers;

public class ColdEmissionDangerousDistanceCalculationManager : BaseDangerousDistanceCalculationManager, IColdEmissionDangerousDistanceCalculationManager
{
    public ColdEmissionDangerousDistanceCalculationManager(ISingleSourceEmissionReportModelBuilder reportModelBuilder) : base(reportModelBuilder)
    { }

    protected override double CalculateDCoef(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        double result;

        if (sourceProperties.VmI <= 0.5)
        {
            result = 5.7;
        }
        else if (sourceProperties.VmI <= 2)
        {
            result = 11.4d * sourceProperties.VmI;
        }
        else
        {
            result = 16d * Math.Sqrt(sourceProperties.VmI); 
        }

        return result;
    }
}
