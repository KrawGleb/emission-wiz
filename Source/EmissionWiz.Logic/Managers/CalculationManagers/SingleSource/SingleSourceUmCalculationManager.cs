using Autofac;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource;

internal class SingleSourceUmCalculationManager : BaseManager, ISingleSourceUmCalculationManager
{
    public double CalculateUm(SingleSourceCalculationData model, EmissionSourceProperties sourceProperties)
    {
        if ((sourceProperties.F >= 100 || (model.DeltaT >= 0 && model.DeltaT <= 0.5)) && sourceProperties.VmI >= 0.5)
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
        else if (sourceProperties.F < 100)
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
        else
        {
            return 0.5;
        }
    }
}
