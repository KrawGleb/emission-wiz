﻿using EmissionWiz.Models.Dto;

namespace EmissionWiz.Models.Interfaces.Managers;

public interface IGeoTiffManager : IBaseManager
{
    Task<Guid> GenerateGeoTiffAsync(GeoTiffOptions options);
}