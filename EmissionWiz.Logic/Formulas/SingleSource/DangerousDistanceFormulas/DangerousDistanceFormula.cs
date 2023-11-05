namespace EmissionWiz.Logic.Formulas.SingleSource.DangerousDistanceFormulas;

internal class DangerousDistanceFormula : BaseFormula
{
    internal class Model
    {
        public double F { get; set; }
        public double DCoef { get; set; }
        public double H { get; set; }
        public double Result { get; set; }
    }

    public override string? Comment => "Расстояние xₘ от источника выброса, на котором приземная концентрация c ЗВ при неблагоприятных метеорологических условиях достигает максимального значения cₘ, определяется по формуле:";

    public override string Template =>
        @"x_{m}=\frac{5 - F}{4} \cdot d \cdot H = \frac{5 - {{trimByPrecision F}} }{4} \cdot {{trimByPrecision DCoef}} \cdot {{trimByPrecision H}} = {{trimByPrecision Result}} м";
}
