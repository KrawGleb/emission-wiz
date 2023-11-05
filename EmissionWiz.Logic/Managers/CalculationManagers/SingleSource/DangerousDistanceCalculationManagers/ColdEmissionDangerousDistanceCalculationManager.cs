﻿using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousDistanceCalculationManagers;

internal class ColdEmissionDangerousDistanceCalculationManager : BaseDangerousDistanceCalculationManager
{
    public ColdEmissionDangerousDistanceCalculationManager(ICalculationReportManager reportManager) : base(reportManager)
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