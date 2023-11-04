using EmissionWiz.Logic.Managers;
using EmissionWiz.Models.Interfaces;

namespace EmissionWiz.Logic.Formulas;

internal abstract class BaseFormula : IFormula
{
    public abstract string Template { get; }

    public string Format(object model) => HbsTemplateManager.Format(Template, model);
}
