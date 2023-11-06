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
    private readonly bool _useSpecial;

    public NCoefFormula(double vm, bool useSpecial = false)
    {
        _vm = vm;
        _useSpecial = useSpecial;
    }

    public override string Template => GetFormulaTemplate();

    public override string? Comment => _comment;

    private string GetFormulaTemplate()
    {
        _comment = "Коэффициент n определяется по формуле ";
        var vmString = _useSpecial
            ? "v{{math Lower|m}}'"
            : "v{{math Lower|m}}";

        if (_vm < 0.5)
        {
            _comment += $"(при {vmString} < 0.5):";
            return @"n = 4.4 \cdot v_{m} = 4.4 \cdot {{trimByPrecision Vm}} = {{trimByPrecision  Result}}";
        }
        else if (_vm < 2)
        {
            _comment += $"(при 0.5 < {vmString} < 2):";
            return @"n = 0.532 \cdot v_{m}^2 - 2.13 \cdot v_{m} + 3.13 = 0.532 \cdot {{trimByPrecision Vm}}^2 - 2.13 \cdot {{ trimByPrecision Vm}} + 3.13 = {{trimByPrecision Result}}";
        }
        else
        {
            _comment += $"(при {vmString} {{{{math GoE}}}} 2):";
            return @"n = 1";
        }
    }
}
