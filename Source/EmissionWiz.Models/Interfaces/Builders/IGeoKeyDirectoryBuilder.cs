using EmissionWiz.Models.Dto;

namespace EmissionWiz.Models.Interfaces.Builders;

public interface IGeoKeyDirectoryBuilder
{
    IGeoKeyDirectoryBuilder AddKey<T>(GeoKey<T> key);
    GeoKeyDirectoryResult Build();
}
