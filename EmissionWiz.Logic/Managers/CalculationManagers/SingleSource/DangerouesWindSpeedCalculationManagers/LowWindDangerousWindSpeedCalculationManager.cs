using EmissionWiz.Logic.Formulas.SingleSource.DangerousWindSpeedFormulas;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerouesWindSpeedCalculationManagers;

internal class LowWindDangerousWindSpeedCalculationManager : IDangerousWindSpeedCalculationManager
{
    private readonly ICalculationReportManager _reportManager;

    public LowWindDangerousWindSpeedCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
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

        var reportBlock = new FormulaBlock();
        reportBlock.Comment = "Опасная скорость ветра u{{math Lower|m}} на стандартном уровне флюгера (10м от уровня земли), при которой достигается наибольшая приземная концентрация ЗВ с{{math Lower|m}}";
        reportBlock.PushFormula(new LowWindDangerousWindSpeedFormula(sourceProperties.Vm), new LowWindDangerousWindSpeedFormula.Model
        {
            Vm = sourceProperties.Vm,
            F = sourceProperties.F,
            Result = result
        });

        _reportManager.AddBlock(reportBlock);

        return result;
    }
}
