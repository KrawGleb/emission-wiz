namespace EmissionWiz.Models.Interfaces;

public interface IFormula
{
    string Format(object model);
    string Comment { get; }
}
