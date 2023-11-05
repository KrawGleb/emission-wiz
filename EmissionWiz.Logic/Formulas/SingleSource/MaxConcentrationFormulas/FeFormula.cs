namespace EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;

internal class FeFormula : BaseFormula
{
    internal class Model
    {
        public double Vm { get; set; }
        public double Result { get; set; }
    }

    public override string Template => @"f_{e} = 800\cdot ({\nu}'_{m})^{3} = 800\cdot ({{trimByPrecision Vm }})^{3} = {{trimByPrecision Result}}";
}
