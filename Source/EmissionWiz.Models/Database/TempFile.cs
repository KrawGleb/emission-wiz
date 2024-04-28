using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmissionWiz.Models.Database;


[Table("TempFile")]
public partial class TempFile
{
    public Guid Id { get; set; }
	
    [Column(TypeName = "datetime")]
    public DateTime Timestamp { get; set; }
	
    public Guid? PrincipalId { get; set; }
	
    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = null!;
	
    [Required]
    [StringLength(255)]
    public string ContentType { get; set; } = null!;
	
    public byte[]? Data { get; set; }
	
    [StringLength(512)]
    public string? Label { get; set; }
	
}
