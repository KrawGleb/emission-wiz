namespace EmissionWiz.Logic.Formulas.SingleSource.DangerousWindSpeedFormulas;

internal class LowWindDangerousWindSpeedFormula : BaseFormula
{
    internal class Model
    {
        public double F { get; set; }
        public double Vm { get; set; }
        public double Result { get; set; }
    }

    private readonly string _template;
    private readonly string? _nearbyComment;
    private readonly string _comment;

    public LowWindDangerousWindSpeedFormula(double vm)
    {
        _comment = "В случае {{math f}} < 100 определяется по формуле: ";

        if (vm <= 0.5)
        {
            _template = @"u_{m} = 0.5 \frac{м}{с}";
            _nearbyComment = ", при v{{math Lower|m}} {{math LoE}} 0.5";
        }
        else if (vm <= 2)
        {
            _template = @"u_{m} = v_{m} = {{trimByPrecision Vm}} = {{trimByPrecision Result}} \frac{м}{с}";
            _nearbyComment = ", при 0.5 < v{{math Lower|m}} {{math LoE}} 2";
        }
        else
        {
            _template = @"u_{m} = v_{m} \cdot (1 + 0.12 \cdot \sqrt{f}) = {{trimByPrecision Vm}} \cdot (1 + 0.12 \cdot \sqrt{ {{trimByPrecision F}} }) = {{trimByPrecision Result}} \frac{м}{с}";
            _nearbyComment = ", при v{{math Lower|m}} > 2";
        }
    }

    public override string Template => _template;
    public override string? NearbyComment => _nearbyComment;
    public override string? Comment => _comment;
}
