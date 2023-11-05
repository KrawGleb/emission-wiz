namespace EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;

internal class ColdEmissionCFormula : BaseFormula
{
    internal class Model
    {
        public double A { get; set; }
        public double M { get; set; }
        public double F { get; set; }
        public double NCoef { get; set; }
        public double Eta { get; set; }
        public double H { get; set; }
        public double KCoef { get; set; }
        public double Result { get; set; }
    }

    public override string Template => @"c_{m} = \frac{A \cdot M \cdot F \cdot n \cdot \eta}{ H^{4/3} } \cdot K = \frac{ {{trimByPrecision A}}  \cdot {{trimByPrecision M}} \cdot {{trimByPrecision F}} \cdot {{trimByPrecision NCoef}} \cdot {{trimByPrecision Eta}} }{ {{trimByPrecision H}} ^{4/3} } \cdot {{trimByPrecision KCoef}} = {{trimByPrecision Result}}";
}
