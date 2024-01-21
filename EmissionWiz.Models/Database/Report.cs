using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmissionWiz.Models.Database;

[Table("Report")]
public class Report
{
    public Guid Id { get; set; }

    public Guid OperationId { get; set; }

    [StringLength(1024)]
    public string FileName { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    [StringLength(255)]
    public string ContentType { get; set; } = null!;

    public byte[]? Data { get; set; }

    [StringLength(512)]
    public string Label { get; set; } = null!;

    [ForeignKey("OperationId")]
    [InverseProperty("Reports")]
    public CalculationResult CalculationResult { get; set; } = null!;
}
