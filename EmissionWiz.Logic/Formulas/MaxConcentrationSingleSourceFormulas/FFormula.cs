namespace EmissionWiz.Logic.Formulas.MaxConcentrationSingleSourceFormulas;

internal class FFormula : BaseFormula
{
    internal class Model
    {
        public double W { get; set; }
        public double D { get; set; }
        public double H { get; set; }
        public double DeltaT { get; set; }
        public double Result { get; set; }
    }

    public override string Template =>
        @"f = 1000 \cdot \frac{ \omega_{0}^2 \cdot D }{ H^2 \cdot \Delta T } = 1000 \cdot \frac{ {{trimByPrecision W }} \cdot {{trimByPrecision D }} }{ {{trimByPrecision H}} ^2 \cdot {{trimByPrecision DeltaT}} } = {{trimByPrecision Result}}";
}
