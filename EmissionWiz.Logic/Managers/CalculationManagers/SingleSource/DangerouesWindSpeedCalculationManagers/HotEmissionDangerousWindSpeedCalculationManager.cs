using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerouesWindSpeedCalculationManagers;

internal class HotEmissionDangerousWindSpeedCalculationManager : IDangerousWindSpeedCalculationManager
{
    private readonly ICalculationReportManager _reportManager;

    public HotEmissionDangerousWindSpeedCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    public double CalculateDangerousWindSpeed(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        var reportBlock = new FormulaBlock();
        reportBlock.Comment = "Для источника выброса фиксированной высоты H при 0 {{math LoE}} v{{math Lower|m}}\' < 0.5 и -0.5 {{math LoE}} {{math Delta}}T {{math LoE}} 0 принимается u{{math Lower|m}} = 0.5 м/c";

        _reportManager.AddBlock(reportBlock);

        return 0.5;
    }
}
