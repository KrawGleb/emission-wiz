using EmissionWiz.Models.Database;
using EmissionWiz.Models.Dto;

namespace EmissionWiz.Models.Interfaces.Repositories;

public interface ISubstanceRepository : IGenericRepository<Substance>
{
    IQueryable<SubstanceDto> GetSubstances();
}
