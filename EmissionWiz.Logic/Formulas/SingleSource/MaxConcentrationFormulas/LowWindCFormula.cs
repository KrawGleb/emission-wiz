namespace EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;

internal class LowWindCFormula : BaseFormula
{
    internal class Model
    {
        public double A { get; set; }
        public double M { get; set; }
        public double F { get; set; }
        public double Mi { get; set; }
        public double Eta { get; set; }
        public double H { get; set; }
        public double Result { get; set; }
    }

    public override string Template => @"c_{m} = \frac{A \cdot M\cdot F \cdot m' \cdot \eta}{H^{\frac{7}{3} } } = \frac{ {{trimByPrecision A}} \cdot {{trimByPrecision M}} \cdot {{trimByPrecision F}} \cdot {{trimByPrecision Mi}} \cdot {{trimByPrecision Eta}} }{  {{trimByPrecision H}}^{\frac{7}{3} } } = {{trimByPrecision Result}}";
}
