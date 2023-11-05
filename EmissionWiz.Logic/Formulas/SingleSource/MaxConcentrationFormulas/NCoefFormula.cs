namespace EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;

internal class NCoefFormula : BaseFormula
{
    internal class Model
    {
        public double Vm { get; set; }
        public double Result { get; set; }
    }

    private string? _comment;
    private readonly double _vm;

    public NCoefFormula(double vm)
    {
        _vm = vm;
    }

    public override string Template => GetFormulaTemplate();

    public override string? Comment => _comment;

    private string GetFormulaTemplate()
    {
        _comment = "Коэффициент n при ƒ < 100 определяется по формуле ";
        if (_vm < 0.5)
        {
            _comment += "(при vₘ < 0.5):";
            return @"n = 4.4 \cdot v_{m} = 4.4 \cdot {{trimByPrecision Vm}} = {{trimByPrecision  Result}}";
        }
        else if (_vm < 2)
        {
            _comment += "(при 0.5 < vₘ < 2):";
            return @"n = 0.532 \cdot v_{m}^2 - 2.13 \cdot v_{m} + 3.13 = 0.532 \cdot {{trimByPrecision Vm}}^2 - 2.13 \cdot {{ trimByPrecision Vm}} + 3.13 = {{trimByPrecision Result}}";
        }
        else
        {
            _comment += "(при vₘ ≥ 2):";
            return @"n = 1";
        }
    }
}
