using System.ComponentModel.DataAnnotations.Schema;

namespace EmissionWiz.Models.Database;

[Table("CalculationResult")]
public class CalculationResult
{
    public Guid Id { get; set; }

    public string Results { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    [InverseProperty("CalculationResult")]
    public ICollection<Report> Reports { get; set; } = null!;
}
