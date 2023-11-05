namespace EmissionWiz.Models.Interfaces;

public interface IFormula
{
    string? Comment { get; }
    string? NearbyComment { get; }
    string Format(object model);
}
