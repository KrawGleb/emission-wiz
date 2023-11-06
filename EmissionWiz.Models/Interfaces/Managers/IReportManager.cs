namespace EmissionWiz.Models.Interfaces.Managers;

public interface IReportManager
{
    Task FromTemplate(Stream destination, string path, object model);
}
