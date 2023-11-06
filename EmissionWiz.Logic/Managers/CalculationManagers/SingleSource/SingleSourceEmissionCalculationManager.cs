using EmissionWiz.Logic.Formulas.SingleSource;
using EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;
using EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerouesWindSpeedCalculationManagers;
using EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousDistanceCalculationManagers;
using EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationCalculationManagers;
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

    public SingleSourceEmissionCalculationResult Calculate(SingleSourceInputModel model, string reportName)
    {
        var sourceProperties = GetEmissionSourceProperties(model);

        var intermediateResults = new SingleSourceEmissionCalculationResult()
        {
            MaxConcentration = CalculateMaxConcentration(model, sourceProperties),
            DangerousDistance = CalculateDangerousDistance(model, sourceProperties),
            DangerousWindSpeed = CalculateDangerousWindSpeed(model, sourceProperties)
        };

        intermediateResults.MaxUntowardConcentrationDistance = CalculateMaxUntowardConcentrationDistance(model, sourceProperties, intermediateResults);

        using var testFile = File.Open($"C:\\Users\\krawc\\Desktop\\Test\\{reportName}", FileMode.OpenOrCreate);
        _reportManager
            .SetTitle("Расчет максимальных разовых концентраций от выбросов \nодиночного точечного источника")
            .Generate(testFile);

        return intermediateResults;
    }

    private double CalculateMaxConcentration(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        IMaxConcentrationCalculationSubManager? subManager;
        if ((sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5)) && sourceProperties.VmI >= 0.5)
        {
            subManager = new ColdEmissionMaxConcentrationCalculationManager(_reportManager);
        }
        else if (sourceProperties.F < 100 && sourceProperties.Vm < 0.5 || sourceProperties.F >= 100 && sourceProperties.VmI < 0.5)
        {
            subManager = new LowWindMaxConcentrationCalculationManager(_reportManager);
        }
        else
        {
            subManager = new HotEmissionMaxConcentrationCalculationManager(_reportManager);
        }

        var maxConcentration = subManager.CalculateMaxConcentration(model, sourceProperties);

        return maxConcentration;
    }
    
    private double CalculateDangerousWindSpeed(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        IDangerousWindSpeedCalculationManager? subManager;
        if ((sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5)) && sourceProperties.VmI >= 0.5)
        {
            subManager = new ColdEmissionDangerousWindSpeedCalculationManager(_reportManager);
        }
        else if (sourceProperties.F < 100 && sourceProperties.Vm < 0.5 || sourceProperties.F >= 100 && sourceProperties.VmI < 0.5)
        {
            subManager = new LowWindDangerousWindSpeedCalculationManager(_reportManager);
        }
        else
        {
            subManager = new HotEmissionDangerousWindSpeedCalculationManager(_reportManager);
        }

        return subManager.CalculateDangerousWindSpeed(model, sourceProperties);
    }

    private double CalculateDangerousDistance(SingleSourceInputModel model, EmissionSourceProperties sourceProperties)
    {
        IDangerousDistanceCalculationManager? subManager;
        if (sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5))
        {
            subManager = new ColdEmissionDangerousDistanceCalculationManager(_reportManager);
        }
        else if (sourceProperties.F < 100)
        {
            subManager = new LowWindDangerousDistanceCalculationManager(_reportManager);
        }
        else
        {
            subManager = new HotEmissionDangerousDistanceCalculationManager(_reportManager);
        }

        var dangerousDistance = subManager.CalculateDangerousDistance(model,sourceProperties);

        return dangerousDistance;
    }

    public double CalculateMaxUntowardConcentrationDistance(SingleSourceInputModel model, EmissionSourceProperties sourceProperties, SingleSourceEmissionCalculationResult intermediateResults)
    {
        var reportBlock = new FormulaBlock();
        
        var (r, rBlock) = GetRCoef(model, intermediateResults);
        reportBlock.PushBlock(rBlock);

        var result = intermediateResults.MaxConcentration * r;

        _reportManager.AddBlock(reportBlock);

        return result;
    }

    private (double, FormulaBlock) GetRCoef(SingleSourceInputModel model, SingleSourceEmissionCalculationResult intermediateResults)
    {
        var ratio = model.U / intermediateResults.DangerousWindSpeed;
        
        double result;
        if (ratio <= 1)
        {
            result = 0.67 * ratio + 1.67 * Math.Pow(ratio, 2) - 1.34 * Math.Pow(ratio, 3);
        }
        else
        {
            result = (3 * ratio) / (2 * Math.Pow(ratio, 2) - ratio + 2);
        }

        var reportBlock = new FormulaBlock();
        reportBlock.PushFormula(new RCoefFormula(ratio), new RCoefFormula.Model
        {
            U = model.U,
            Um = intermediateResults.DangerousWindSpeed,
            Result = result
        });

        return (result, reportBlock);
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
