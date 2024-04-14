using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmissionWiz.Models.Database;


[Table("Report")]
public partial class Report
{
    public Guid Id { get; set; }
	
    public Guid OperationId { get; set; }
	
    [Required]
    [StringLength(1024)]
    public string FileName { get; set; } = null!;
	
    [Column(TypeName = "datetime")]
    public DateTime Timestamp { get; set; }
	
    [Required]
    [StringLength(255)]
    public string ContentType { get; set; } = null!;
	
    public byte[]? Data { get; set; }
	
    [StringLength(512)]
    public string? Label { get; set; }
	
    [ForeignKey("OperationId")]
    [InverseProperty("Reports")]
    public CalculationResult CalculationResult { get; set; } = null!;
}
