using EmissionWiz.Models.Attributes;
using EmissionWiz.Models.Configs;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Map.Shapes;
using Microsoft.Extensions.Options;
using System.Text;
using EmissionWiz.Models;
using CoordinateSharp;

namespace EmissionWiz.Logic.Managers;

[TransientDependency]
internal class MapManager : BaseManager, IMapManager
{
    private StringBuilder _geometry = new();
    private readonly GeoApiConfiguration _geoApiConfiguration;
    private readonly HttpClient _geoApiClient;

    public MapManager(
        IOptions<GeoApiConfiguration> geoApiConfiguration,
        IHttpClientFactory httpClientFactory)
    {
        _geoApiConfiguration = geoApiConfiguration.Value;
        _geoApiClient = httpClientFactory.CreateClient(Constants.HttpClientName.GeoApi);
    }

    public async Task<Stream?> PrintAsync()
    {
        var uriBuilder = new UriBuilder(_geoApiConfiguration.BaseUrl);
        var query = new Dictionary<string, string>();
        query["geometry"] = _geometry.ToString().Trim('|');
        query["apiKey"] = _geoApiConfiguration.ApiKey;
        query["width"] = "600";
        query["height"] = "600";
        uriBuilder.Query = string.Join("&", query.Select(x => $"{x.Key}={x.Value}").ToArray());
        string uri = uriBuilder.ToString();

        var response = await _geoApiClient.GetAsync(uri);

        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStream();
    }

    public void DrawShape(Models.Map.Shapes.Shape shape)
    {
        if (shape.GetType() == typeof(Circle))
            DrawCircle((Circle)shape);
    }

    private void DrawCircle(Circle circle)
    {
        var points = CircleToPolygon(circle);
        var polyline = string.Join(",", points.Select(x => $"{x.Item2},{x.Item1}"));
        _geometry.Append($"|polyline:{polyline};linewidth:3");
    }

    private List<(double, double)> CircleToPolygon(Circle circle)
    {

        var points = new List<(double, double)>();
        for (int i = 0; i <= 360; i+=4)
        {
            var center = new Coordinate(circle.Center.Lat, circle.Center.Lon);
            center.Move(circle.Radius, i, CoordinateSharp.Shape.Sphere);

            points.Add((center.Latitude.DecimalDegree, center.Longitude.DecimalDegree));
        }

        return points;
    }
}
