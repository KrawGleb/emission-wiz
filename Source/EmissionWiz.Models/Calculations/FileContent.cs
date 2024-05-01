using EmissionWiz.Models.Enums;

namespace EmissionWiz.Models.Calculations;

public class FileContent
{
    public Guid FileId { get; set; }
    public FileContentType Type { get; set; }
    public string Name { get; set; } = null!;
    public int SortOrder { get; set; }
}
