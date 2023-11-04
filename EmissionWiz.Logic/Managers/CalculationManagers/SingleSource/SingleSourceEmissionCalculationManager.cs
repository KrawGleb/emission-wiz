using EmissionWiz.Logic.Formulas.MaxConcentrationSingleSourceFormulas;
using EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationSingleSourceCalculationManagers;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Reports.Blocks;

namespace EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;

// Метод расчета максимальных разовых концентраций от выбросов одиночного точечного источника
public class SingleSourceEmissionCalculationManager : ISingleSourceEmissionCalculationManager
{
    private readonly ICalculationReportManager _reportManager;

    public SingleSourceEmissionCalculationManager(ICalculationReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    public SingleSourceEmissionCalculationResult Calculate(SingleSourceInputModel model)
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

        var result = subManager?.CalculateMaxConcentration(model, sourceProperties)
            ?? throw new InvalidOperationException();

        using var testFile = File.Open("C:\\Users\\krawc\\Desktop\\Test\\test.pdf", FileMode.OpenOrCreate);
        _reportManager.Generate(testFile);

        return result;
    }
    
    private double GetV(SingleSourceInputModel model)
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

    private EmissionSourceProperties GetEmissionSourceProperties(SingleSourceInputModel model)
    {
        var v = GetV(model);

        var vm = 0.65d * Math.Cbrt((v * model.DeltaT + 0.0d) / model.H);
        var vmI = 1.3d * (model.W * model.D / model.H);
        var f = 1000d * (Math.Pow(model.W, 2d) * model.D) / (Math.Pow(model.H, 2d) * model.DeltaT);
        var fe = 800d * Math.Pow(vmI, 3d);

        var reportBlock = new FormulaBlock("Коэффициенты m и n определяются в зависимости от характеризующих свойства источника выброса параметров:");
        reportBlock.PushFormula(new VmFormula(),
            new VmFormula.Model()
            {
                V1 = v,
                DeltaT = model.DeltaT,
                H = model.H,
                Result = vm
            });
        reportBlock.PushFormula(new VmIFormula(),
            new VmIFormula.Model
            {
                D = model.D,
                H = model.H,
                W = model.W,
                Result = vmI
            });
        reportBlock.PushFormula(new FFormula(),
            new FFormula.Model
            {
                D = model.D,
                H = model.H,
                W = model.W,
                DeltaT = model.DeltaT,
                Result = f
            });
        reportBlock.PushFormula(new FeFormula(),
            new FeFormula.Model
            {
                Vm = vm,
                Result = fe
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
