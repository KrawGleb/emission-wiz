using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmissionWiz.Models.Database;


[Table("CalculationResult")]
public partial class CalculationResult
{
    public CalculationResult()
    {
        Reports = new HashSet<Report>();
    }
    public Guid Id { get; set; }
	
    [Required]
    public string Results { get; set; } = null!;
	
    [Column(TypeName = "datetime")]
    public DateTime Timestamp { get; set; }
	
    [InverseProperty("CalculationResult")]
    public ICollection<Report> Reports { get; set; }
}
