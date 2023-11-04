﻿namespace EmissionWiz.Logic.Formulas.MaxConcentrationSingleSourceFormulas;

internal class VmFormula : BaseFormula
{
    internal class VFormulaModel
    {
        public double V1 { get; set; }
        public double DeltaT { get; set; }
        public double H { get; set; }
        public double Result { get; set; }
    }

    public override string Template =>
        @"\nu_{m} = 0.65 \cdot \sqrt[3]{\frac{V_{1} \cdot \Delta T}{H}} = 0.65 \cdot \sqrt[3]{\frac{ {{trimByPrecision V}} \cdot {{trimByPrecision DeltaT}} }{ {{trimByPrecision H}} }} = {{trimByPrecision Result}}";
}
