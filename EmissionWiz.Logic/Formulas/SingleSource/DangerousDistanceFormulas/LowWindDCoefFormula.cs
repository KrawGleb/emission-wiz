using static EmissionWiz.Models.Constants;

namespace EmissionWiz.Logic.Formulas.SingleSource.DangerousDistanceFormulas;

internal class LowWindDCoefFormula : BaseFormula
{
    internal class Model
    {
        public double Fe { get; set; }
        public double F { get; set; }
        public double Vm { get; set; }
        public double Result { get; set; }
    }

    private string? _comment;
    private readonly double _vm;

    public LowWindDCoefFormula(double vm)
    {
        _vm = vm;
    }

    public override string? Comment => _comment;
    public override string Template => GetTemplate();

    private string GetTemplate()
    {
        _comment = $"Безразмерный коэффициент d при {MathChars.f} < 100 находится по формулe ";
        if (_vm <= 0.5)
        {
            _comment += "(при vₘ ≤ 0.5):";
            return @"d = 2.48 \cdot (1 + 0.28 \cdot \sqrt[3]{f_{e} }) = 2.48 \cdot (1 + 0.28 \cdot \sqrt[3]{ {{trimByPrecision Fe}} }) = {{trimByPrecision Result}}";
        }
        else if (_vm <= 2)
        {
            _comment += "(при 0.5 < vₘ ≤ 2):";
            return @"d = 4.95 \cdot v_{m} \cdot (1 + 0.28 \cdot \sqrt[3]{f}) = 4.95 \cdot {{trimByPrecision Vm}} \cdot (1 + 0.28 \cdot \sqrt[3]{ {{trimByPrecision F}} }) = {{trimByPrecision Result}}";
        }
        else
        {
            _comment += "(при vₘ > 2):";
            return @"d = 7 \cdot \sqrt{v_{m}} \cdot (1 + 0.28 \cdot \sqrt[3]{f}) = 7 \cdot \sqrt{ {{trimByPrecision Vm}} } \cdot (1 + 0.28 \cdot \sqrt[3]{ {{trimByPrecision F}} }) = {{trimByPrecision Result}}";
        }
    }
}
