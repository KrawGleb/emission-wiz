using EmissionWiz.Logic.Formulas.SingleSource.DangerousWindSpeedFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerouesWindSpeedCalculationManagers;

internal class ColdEmissionDangerousWindSpeedCalculationManager : IDangerousWindSpeedCalculationManager
{
    private readonly ICalculationReportManager _reportManager;

    public ColdEmissionDangerousWindSpeedCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
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

        var reportBlock = new FormulaBlock();
        reportBlock.Comment = "Опасная скорость ветра u{{math Lower|m}} на стандартном уровне флюгера (10м от уровня земли), при которой достигается наибольшая приземная концентрация ЗВ с{{math Lower|m}}";
        reportBlock.PushFormula(new ColdEmissionDangerousWindSpeedFormula(sourceProperties.VmI), new ColdEmissionDangerousWindSpeedFormula.Model
        {
            VmI = sourceProperties.VmI,
            Result = result
        });

        _reportManager.AddBlock(reportBlock);

        return result;
    }
}
