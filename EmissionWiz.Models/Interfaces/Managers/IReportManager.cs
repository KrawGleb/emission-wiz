namespace EmissionWiz.Models.Interfaces.Managers;

public interface IReportManager : IBaseManager
{
    Task FromTemplate(Stream destination, string path, object model);
}
