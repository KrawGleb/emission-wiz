using System.ComponentModel;

namespace EmissionWiz.Models.Enums;

public enum FileContentType
{
    [Description("Unknown")]
    Unknown = -1,

    [Description("Image")]
    Image = 0,

    [Description("Pdf")]
    Pdf = 1,
}
