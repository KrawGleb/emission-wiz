namespace EmissionWiz.Logic.Managers.CalculationManagers.SingleSource;

public static class SingleSourceCommon
{
    public static double GetRCoef(double u, double um)
    {
        var ratio = u / um;

        double result;
        if (ratio <= 1)
        {
            result = 0.67 * ratio + 1.67 * Math.Pow(ratio, 2) - 1.34 * Math.Pow(ratio, 3);
        }
        else
        {
            result = (3 * ratio) / (2 * Math.Pow(ratio, 2) - ratio + 2);
        }


        return result;
    }

    public static double GetS1Coef(double x, double xm, double fCoef)
    {
        double result;
        var ratio = x / xm;
        if (ratio <= 1)
        {
            result = 3 * Math.Pow(ratio, 4d) - 8 * Math.Pow(ratio, 3d) + 6 * Math.Pow(ratio, 2d);
        }
        else if (1 < ratio && ratio <= 8)
        {
            result = 1.13 / (0.13 * Math.Pow(ratio, 2d) + 1);
        }
        else if (8 < ratio && ratio <= 100 && fCoef <= 1.5)
        {
            result = ratio / (3.556 * Math.Pow(ratio, 2d) - 35.2 * ratio + 120);
        }
        else if (8 < ratio && ratio <= 100 && fCoef > 1.5)
        {
            result = 1 / (0.1 * Math.Pow(ratio, 2d) + 2.456 * ratio - 17.8);
        }
        else if (ratio > 100 && fCoef <= 1.5)
        {
            result = 144.3 * Math.Cbrt(Math.Pow(ratio, -7d));
        }
        else
        {
            result = 37.76 * Math.Cbrt(Math.Pow(ratio, -7d));
        }

        return result;
    }
}
