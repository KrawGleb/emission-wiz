using EmissionWiz.Logic.Formulas.MaxConcentrationSingleSourceFormulas;
using EmissionWiz.Models.Calculations;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;

// Метод расчета максимальных разовых концентраций от выбросов одиночного точечного источника
public class MaxConcentrationSingleSourceCalculationManager : IMaxConcentrationSingleSourceCalculationManager
{
    private readonly ICalculationReportManager _reportManager;

    public MaxConcentrationSingleSourceCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    // TODO: Add report

    public double CalculateMaxConcentration(MaxConcentrationInputModel model)
    {
        var sourceProperties = GetEmissionSourceProperties(model);
        IMaxConcentrationSingleSourceCalculationSubManager? subManager;
        if ((sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5)) && sourceProperties.VmI >= 0.5)
        {
            subManager = new ColdEmissionMaxConcentrationSingleSourceCalculationManager(_reportManager);
        }
        else if (sourceProperties.F < 100 && sourceProperties.Vm < 0.5 || sourceProperties.F >= 100 && sourceProperties.VmI < 0.5)
        {
            subManager = new LowWindMaxConcentrationSingleSourceCalculationManager(_reportManager);
        }
        else
        {
            subManager = new HotEmissionMaxConcentrationSingleSourceCalculationManager(_reportManager);
        }

        using var testFile = File.Open("C:\\Users\\krawc\\Desktop\\Test\\test.pdf", FileMode.OpenOrCreate);
        _reportManager.Generate(testFile);

        return subManager?.CalculateMaxConcentration(model, sourceProperties) 
            ?? throw new InvalidOperationException();
    }
    
    private double GetV(MaxConcentrationInputModel model)
    {
        var result =  Math.PI * Math.Pow(model.D, 2d) / 4 * model.W;

        var reportBlock = new FormulaBlock();
        reportBlock.PushFormula(new V1Formula(), new V1Formula.Model
        {
            D = model.D,
            W = model.W,
            Result = result
        });

        _reportManager.AddBlock(reportBlock);


        return result;
    }

    private EmissionSourceProperties GetEmissionSourceProperties(MaxConcentrationInputModel model)
    {
        var v = GetV(model);

        var vm = 0.65d * Math.Cbrt((v * model.DeltaT + 0.0d) / model.H);
        var vmI = 1.3d * (model.W * model.D / model.H);
        var f = 1000d * (Math.Pow(model.W, 2d) * model.D) / (Math.Pow(model.H, 2d) * model.DeltaT);
        var fe = 800d * Math.Pow(vmI, 3d);

        var reportBlock = new FormulaBlock();
        reportBlock.PushFormula(new VmFormula(),
            new VmFormula.VFormulaModel()
            {
                V1 = v,
                DeltaT = model.DeltaT,
                H = model.H,
                Result = vm
            });

        _reportManager.AddBlock(reportBlock);

        return new EmissionSourceProperties
        {
            V = v,
            Vm = vm,
            VmI = vmI,
            F = f,
            Fe = fe
        };
    }
}
