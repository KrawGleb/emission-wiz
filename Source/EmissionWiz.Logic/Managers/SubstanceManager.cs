using EmissionWiz.Models.Dto;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmissionWiz.Logic.Managers;

internal class SubstanceManager : BaseManager, ISubstanceManager
{
    private readonly ISubstanceRepository _substanceRepository;

    public SubstanceManager(ISubstanceRepository substanceRepository)
    {
        _substanceRepository = substanceRepository;
    }

    public async Task<List<SubstanceDto>> GetSubstancesAsync()
    {
        var result = await _substanceRepository.GetSubstances().ToListAsync();
        return result;
    }
}
