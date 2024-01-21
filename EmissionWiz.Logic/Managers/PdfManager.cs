using EmissionWiz.Models;
using EmissionWiz.Models.Attributes;
using EmissionWiz.Models.Interfaces.Managers;
using MigraDocCore.DocumentObjectModel;

namespace EmissionWiz.Logic.Managers;

[InstancePerDependency]
public class PdfManager : BaseManager, IPdfManager
{
    public void DefinePdfStyles(Document document)
    {
        // Get the predefined style Normal.
        var style = document.Styles[StyleNames.Normal];
        // Because all styles are derived from Normal, the next line changes the 
        // font of the whole document. Or, more exactly, it changes the font of
        // all styles and paragraphs that do not redefine the font.
        style.Font.Name = "Times New Roman";
        style.Font.Size = 14;

        // Create a new style called Table based on style Normal
        style = document.Styles.AddStyle(Constants.Pdf.CustomStyleNames.Table, StyleNames.Normal);
        style.Font.Name = "Times New Roman";
        style.Font.Size = 6;

        style = document.Styles.AddStyle(Constants.Pdf.CustomStyleNames.SmallText, StyleNames.Normal);
        style.Font.Name = "Times New Roman";
        style.Font.Size = 10;

        // Create a new style called Title based on style Normal
        style = document.Styles.AddStyle(Constants.Pdf.CustomStyleNames.Title, StyleNames.Normal);
        style.Font.Name = "Times New Roman";
        style.Font.Size = 14;
        style.Font.Bold = true;
    }
}
