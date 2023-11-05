namespace EmissionWiz.Logic.Formulas.SingleSource.DangerousDistanceFormulas;

internal class HotEmissionDangerousDistanceFormula : DangerousDistanceFormula
{
    public override string Template => @"x_{m}=5.7 \cdot H = 5.7 \cdot {{trimByPrecision H}} = {{trimByPrecision Result}}м ";
}
