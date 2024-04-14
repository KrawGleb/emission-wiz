using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmissionWiz.Models.Database;


[Table("Substance")]
public partial class Substance
{
	[Key]
    public int Code { get; set; }
	
    [Required]
    [StringLength(1024)]
    public string Name { get; set; } = null!;
	
    [StringLength(128)]
    public string? ChemicalFormula { get; set; }
	
    public byte? DangerClass { get; set; }
	
    [Column(TypeName = "decimal(20, 5)")]
    public decimal? SingleMaximumAllowableConcentration { get; set; }
	
    [Column(TypeName = "decimal(20, 5)")]
    public decimal? DailyAverageMaximumAllowableConcentration { get; set; }
	
    [Column(TypeName = "decimal(20, 5)")]
    public decimal? AnnualAverageMaximumAllowableConcentration { get; set; }
	
}
