using static EmissionWiz.Models.Constants;

namespace EmissionWiz.Logic.Formulas.SingleSource.MaxConcentrationFormulas;

internal class MIFormula : BaseFormula
{
    internal class Model
    {
        public double MCoef { get; set; }
        public double Result { get; set; }
    }

    private string? _nearbyComment;
    private readonly double _vm;

    public MIFormula(double vm)
    {
        _vm = vm;
    }

    public override string Template => GetTemplate();
    public override string? NearbyComment => _nearbyComment;

    private string GetTemplate()
    {
        if (_vm < 0.5)
        {
            _nearbyComment = $", при v{MathChars.Lower.m} < 0.5";
            return @"m' = 2.86 \cdot m = 2.86 \cdot {{trimByPrecision MCoef}} = {{trimByPrecision Result}}";
        }
        else
        {
            _nearbyComment = $", при {MathChars.f}{MathChars.GoE}100, v{MathChars.Lower.m}\'<0.5";
            return @"m' = 0.9";
        }
    }
}
