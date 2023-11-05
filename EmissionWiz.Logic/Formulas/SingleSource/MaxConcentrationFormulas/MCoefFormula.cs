namespace EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;

internal class MCoefFormula : BaseFormula
{
    internal class Model
    {
        public double F { get; set; }
        public double Result { get; set; }
    }

    private readonly double _fValue;
    private string? _comment;

    public MCoefFormula(double fValue)
    {
        _fValue = fValue;
    }

    public override string? Comment => _comment;

    public override string Template => GetTemplate();

    private string GetTemplate()
    {
        _comment = "Коэффициент m определяется по формуле ";
        if (_fValue < 100)
        {
            _comment += "(при {{math f}} < 100): ";
            return @"m = \frac{1}{ 0.67 + 0.1 \cdot \sqrt{f} + 0.34 \cdot \sqrt[3]{f} } = \frac{1}{ 0.67 + 0.1 \cdot \sqrt{ {{ trimByPrecision F}} } + 0.34 \cdot \sqrt[3]{ {{trimByPrecision F}} } } = {{trimByPrecision Result}}";
        }
        else
        {
            _comment += "(при {{math f}} {{math GoE}} 100)";
            return @"m = \frac{1.47}{\sqrt[3]{f}} = \frac{1.47}{\sqrt[3]{ {{trimByPrecision F}} }} = {{Result}}";
        }
    }
}
