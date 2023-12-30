using MigraDocCore.DocumentObjectModel;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface IPdfManager : IBaseManager
{
    void DefinePdfStyles(Document document);
}
