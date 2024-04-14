using CoordinateSharp;
using EmissionWiz.Models.Attributes;
using EmissionWiz.Models.Configs;
using EmissionWiz.Models.Interfaces.Managers;
using Microsoft.Extensions.Options;
using EmissionWiz.Models;
using EmissionWiz.Models.Dto;
using System;

namespace EmissionWiz.Logic.Managers;

[InstancePerDependency]
internal class MapManager : BaseManager, IMapManager
{
    private readonly GeoApiConfiguration _geoApiConfiguration;
    private readonly HttpClient _geoApiClient;
   

    public MapManager(
        IOptions<GeoApiConfiguration> geoApiConfiguration,
        IHttpClientFactory httpClientFactory)
    {
        _geoApiConfiguration = geoApiConfiguration.Value;
        _geoApiClient = httpClientFactory.CreateClient(Constants.HttpClientName.GeoApify);
    }

    public async Task<Stream> GetTileAsync(MapTileOptions options)
    {
        var upperLeftCorner =
            new Coordinate(options.Center.Latitude.DecimalDegree, options.Center.Longitude.DecimalDegree);
        upperLeftCorner.Move(options.Distance, 0, Shape.Ellipsoid);
        upperLeftCorner.Move(options.Distance, -90, Shape.Ellipsoid);

        var lowerRightCorner =
            new Coordinate(options.Center.Latitude.DecimalDegree, options.Center.Longitude.DecimalDegree);
        lowerRightCorner.Move(options.Distance, 90, Shape.Ellipsoid);
        lowerRightCorner.Move(options.Distance, 180, Shape.Ellipsoid);

        var area =
            $"rect:{upperLeftCorner.Longitude.DecimalDegree},{upperLeftCorner.Latitude.DecimalDegree},{lowerRightCorner.Longitude.DecimalDegree},{lowerRightCorner.Latitude.DecimalDegree}";

        // TODO: Add query keys to constants
        var query = new Dictionary<string, string>();
        query["apiKey"] = _geoApiConfiguration.ApiKey;
        query["area"] = area;
        if (options is { Width: not null, Height: not null })
        {
            query["width"] = int.Min(options.Width.Value, Constants.GeoApi.MaxHeightAndWidth).ToString();
            query["height"] = int.Min(options.Height.Value, Constants.GeoApi.MaxHeightAndWidth).ToString();
        }
        
        var uriBuilder = new UriBuilder(_geoApiConfiguration.BaseUrl);
        uriBuilder.Query = string.Join("&", query.Select(x => $"{x.Key}={x.Value}").ToArray());
        var uri = uriBuilder.ToString();

        var response = await _geoApiClient.GetAsync(uri);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }
}
