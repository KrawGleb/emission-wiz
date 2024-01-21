﻿using EmissionWiz.DataProvider.Repositories.Base;
using EmissionWiz.Models.Database;
using EmissionWiz.Models.Interfaces.Repositories;

namespace EmissionWiz.DataProvider.Repositories;

internal class CalculationResultRepository : GenericRepository<CalculationResult>, ICalculationResultRepository
{
}
