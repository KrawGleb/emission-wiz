namespace EmissionWiz.Logic.Formulas.MaxConcentrationSingleSourceFormulas;

internal class V1Formula : BaseFormula
{
    internal class Model
    {
        public double D { get; set; }
        public double W { get; set; }
        public double Result { get; set; }
    }

    public override string? Comment => "Расход ГВС:";

    public override string Template => 
        @"V_{1} = \frac{\pi \cdot D^{2}}{4}\cdot \omega_{0} = \frac{\pi \cdot {{trimByPrecision D}}^{2}}{4}\cdot {{trimByPrecision W}} = {{trimByPrecision Result}}";
}