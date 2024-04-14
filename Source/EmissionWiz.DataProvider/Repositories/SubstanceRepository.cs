using EmissionWiz.DataProvider.Repositories.Base;
using EmissionWiz.Models.Database;
using EmissionWiz.Models.Dto;
using EmissionWiz.Models.Interfaces.Repositories;

namespace EmissionWiz.DataProvider.Repositories;

internal class SubstanceRepository : GenericRepository<Substance>, ISubstanceRepository
{
    public IQueryable<SubstanceDto> GetSubstances()
    {
        var query = from s in Context.Substances
                    select new SubstanceDto
                    {
                        Code = s.Code,
                        Name = s.Name,
                        DangerClass = s.DangerClass,
                        ChemicalFormula = s.ChemicalFormula,
                        AnnualAverageMaximumAllowableConcentration = s.AnnualAverageMaximumAllowableConcentration,
                        DailyAverageMaximumAllowableConcentration = s.DailyAverageMaximumAllowableConcentration,
                        SingleMaximumAllowableConcentration = s.SingleMaximumAllowableConcentration
                    };

        return query;
    }
}
