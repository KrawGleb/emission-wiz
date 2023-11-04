using EmissionWiz.Models.Interfaces;
using EmissionWiz.Models.Reports.Areas;

namespace EmissionWiz.Models.Reports.Blocks;

public class FormulaBlock : BaseBlock
{
    public List<(IFormula, object)> Formulas { get; set; } = new();

    public void PushFormula(IFormula formula, object model) => Formulas.Add((formula, model));
}
