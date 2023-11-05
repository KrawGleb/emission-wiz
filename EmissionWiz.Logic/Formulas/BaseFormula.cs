using EmissionWiz.Common;
using EmissionWiz.Common.Templates;
using EmissionWiz.Models.Interfaces;

namespace EmissionWiz.Logic.Formulas;

internal abstract class BaseFormula : IFormula
{
    public virtual string? Comment { get; }
    public virtual string? NearbyComment { get; }
    public abstract string Template { get; }

    public string Format(object model) => HbsTemplateManager.Format(Template, model);
    public string FormatComment() => HbsTemplateManager.Format(Comment, Constants.MathCharsObj);
}
