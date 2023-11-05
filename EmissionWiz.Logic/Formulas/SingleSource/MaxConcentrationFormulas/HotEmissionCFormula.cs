namespace EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;

internal class HotEmissionCFormula : BaseFormula
{
    internal class Model
    {
        public double A { get; set; }
        public double M { get; set; }
        public double F { get; set; }
        public double m { get; set; }
        public double n { get; set; }
        public double eta { get; set; }
        public double H { get; set; }
        public double V1 { get; set; }
        public double DeltaT { get; set; }
        public double Result { get; set; }
    }

    public override string? Comment =>
        "Максимальная приземная разовая концентрация ЗВ cₘ, мг/м³, при выбросе ГВС из одиночного точечного источника с круглым устьем достигается при опасной скорости ветра uₘ на расстоянии xₘ от источника выброса и определяется по формуле:";

    public override string Template =>
        @"c_{m}=\frac{ A \cdot M \cdot F \cdot m \cdot n\cdot \eta }{ H^2 \cdot \sqrt[3]{V_{1} \cdot \Delta T } }=\frac{ {{trimByPrecision A}} \cdot {{trimByPrecision M}} \cdot {{trimByPrecision F}} \cdot {{trimByPrecision m}} \cdot {{trimByPrecision n}} \cdot {{trimByPrecision eta}} }{ {{trimByPrecision H}}^2 \cdot \sqrt[3]{ {{trimByPrecision V1}} \cdot {{trimByPrecision DeltaT}} } } = {{trimByPrecision Result}} \frac{мг}{м^3}";
}
