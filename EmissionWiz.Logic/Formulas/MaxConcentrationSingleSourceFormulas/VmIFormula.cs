namespace EmissionWiz.Logic.Formulas.MaxConcentrationSingleSourceFormulas;

internal class VmIFormula : BaseFormula
{
    internal class Model
    {
        public double W { get; set; }
        public double D { get; set; }
        public double H { get; set; }
        public double Result { get; set; }
    }

    public override string Template => "{\\nu_{m}}' = 1.3 \\cdot \\frac{\\omega_{0} \\cdot D }{ H } = 1.3 \\cdot \\frac{ {{trimByPrecision W}} \\cdot {{trimByPrecision D}} }{ {{trimByPrecision H}} } = {{trimByPrecision Result}}";
}
