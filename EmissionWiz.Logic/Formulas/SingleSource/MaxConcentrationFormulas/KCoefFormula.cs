namespace EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;

internal class KCoefFormula : BaseFormula
{
    internal class Model
    {
        public double D { get; set; }
        public double V1 { get; set; }
        public double Result { get; set; }
    }

    public override string Template => @"K = \frac{ D }{ 8 * V_{1} } = \frac{ {{trimByPrecision D}} }{ 8 * {{trimByPrecision V}} } = {{trimByPrecision Result}}";
}
