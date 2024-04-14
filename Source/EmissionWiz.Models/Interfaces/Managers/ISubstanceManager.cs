using EmissionWiz.Models.Dto;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface ISubstanceManager : IBaseManager
{
    Task<List<SubstanceDto>> GetSubstancesAsync();
}
