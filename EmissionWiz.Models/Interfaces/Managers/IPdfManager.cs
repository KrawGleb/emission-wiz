using MigraDocCore.DocumentObjectModel;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface IPdfManager
{
    void DefinePdfStyles(Document document);
}
