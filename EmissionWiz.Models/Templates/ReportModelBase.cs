using EmissionWiz.Models.Attributes;
using EmissionWiz.Models.Map.Shapes;

namespace EmissionWiz.Models.Templates;

public abstract class ReportModelBase
{
    [ExpandoIgnore]
    public Dictionary<string, Func<Shape>> MapShapes { get; set; } = new();
}
