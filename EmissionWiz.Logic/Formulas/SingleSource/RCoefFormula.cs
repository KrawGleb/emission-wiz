namespace EmissionWiz.Logic.Formulas.SingleSource;

internal class RCoefFormula : BaseFormula
{
    internal class Model
    {
        public double U { get; set; }
        public double Um { get; set; }
        public double Result { get; set; }
    }

    private readonly string _template;
    private readonly string _nearbyComment;

    public RCoefFormula(double ratio)
    {
        if (ratio <= 1)
        {
            _template = @"r = 0.67 \cdot \frac{u}{u_{m}} + 1.67 \cdot (\frac{u}{u_{m}})^2 - 1.34 \cdot (\frac{u}{u_{m}})^3 = 0.67 \cdot \frac{ {{trimByPrecision U}} }{ {{trimByPrecision Um}} } + 1.67 \cdot (\frac{ {{trimByPrecision U}} }{ {{trimByPrecision Um}} })^2 - 1.34 \cdot (\frac{ {{trimByPrecision U}} }{  {{trimByPrecision Um}} })^3 = {{trimByPrecision Result}}";
            _nearbyComment = ", при u/u{{math Lower|m}} {{math LoE}} 1";
        }
        else
        {
            _template = @"r = \frac{3 \cdot (u/u_{m})}{ 2 \cdot (u/u_{m})^2 - u/u_{m} + 2} = \frac{3 \cdot ( {{trimByPrecision U}} / {{trimByPrecision Um}})}{ 2 \cdot ( {{trimByPrecision U}} / {{trimByPrecision Um}})^2 - {{trimByPrecision U}}/{{trimByPrecision Um}} + 2} = {{trimByPrecision Result}}";
            _nearbyComment = ", при u/u{{math Lower|m}} > 1";
        }
    }

    public override string Template => _template;
    public override string? NearbyComment => _nearbyComment;
}
