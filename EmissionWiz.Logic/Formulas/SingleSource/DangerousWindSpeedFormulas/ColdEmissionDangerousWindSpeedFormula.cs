using static EmissionWiz.Common.Constants;

namespace EmissionWiz.Logic.Formulas.SingleSource.DangerousWindSpeedFormulas;

internal class ColdEmissionDangerousWindSpeedFormula : BaseFormula
{
    internal class Model
    {
        public double VmI { get; set; }
        public double Result { get; set; }
    }

    private readonly string _template;
    private readonly string? _nearbyComment;
    private readonly string _comment;

    public ColdEmissionDangerousWindSpeedFormula(double vmI)
    {
        _comment = "В случае {{math f}} {{math GoE}} 100 или 0 {{math LoE}} {{math Delta}}T {{math LoE}} 0.5 определяется по формуле: ";

        if (vmI <= 0.5)
        {
            _template = @"u_{m} = 0.5 \frac{м}{с}";
            _nearbyComment = ", при v{{math Lower|m}}\' {{math LoE}} 0.5";
        }
        else if (vmI <= 2)
        {
            _template = @"u_{m} = v_{m}' = {{trimByPrecision Result}} \frac{м}{с}";
            _nearbyComment = ", при 0.5 < v{{math Lower|m}}' {{math LoE}} 2";
        }
        else
        {
            _template = @"u_{m} = 2.2 \cdot v_{m}' = 2.2 \cdot {{trimByPrecision VmI}} = {{trimByPrecision Result}} \frac{м}{с}";
            _nearbyComment = ", при v{{math Lower|m}}' > 2";
        }
    }

    public override string Template => _template;
    public override string? NearbyComment => _nearbyComment;
    public override string? Comment => _comment;
}
