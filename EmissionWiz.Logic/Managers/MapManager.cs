using EmissionWiz.Models.Attributes;
using EmissionWiz.Models.Configs;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Map.Shapes;
using Microsoft.Extensions.Options;
using System.Text;
using EmissionWiz.Models;
using CoordinateSharp;
using HandlebarsDotNet;
using System.Globalization;
using System.Web;

namespace EmissionWiz.Logic.Managers;

[InstancePerDependency]
internal class MapManager : BaseManager, IMapManager
{
    private readonly GeoApiConfiguration _geoApiConfiguration;
    private readonly HttpClient _geoApiClient;
    private readonly Random _random = new();

    private readonly List<Marker> _markers = new();
    private readonly List<Circle> _circles = new();

    public MapManager(
        IOptions<GeoApiConfiguration> geoApiConfiguration,
        IHttpClientFactory httpClientFactory)
    {
        _geoApiConfiguration = geoApiConfiguration.Value;
        _geoApiClient = httpClientFactory.CreateClient(Constants.HttpClientName.GeoApi);
    }

    public async Task<(Stream?, Dictionary<string, string>)> PrintAsync()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        var legend = new Dictionary<string, string>();

        var uriBuilder = new UriBuilder(_geoApiConfiguration.BaseUrl);
        var query = new Dictionary<string, string>();

        var markers = string.Join("|", _markers.Select(MarkerForQuery));
        var circlesForQuery = new List<string>();

        foreach (var circle in _circles)
        {
            var preparedCircle = CircleForQuery(circle);
            circlesForQuery.Add(preparedCircle.Item1);

            if (circle.Label != null)
                legend.Add(circle.Label, preparedCircle.Item2);
        }

        query["geometry"] = string.Join("|", circlesForQuery);
        query["marker"] = markers;
        query["apiKey"] = _geoApiConfiguration.ApiKey;
        query["width"] = "600";
        query["height"] = "600";
        uriBuilder.Query = string.Join("&", query.Select(x => $"{x.Key}={x.Value}").ToArray());
        string uri = uriBuilder.ToString();

        var response = await _geoApiClient.GetAsync(uri);

            
        response.EnsureSuccessStatusCode();
        return (response.Content.ReadAsStream(), legend);
    }

    public void AddMarker(Marker marker)
    {
        _markers.Add(marker);
    }

    public void DrawShape(Models.Map.Shapes.Shape shape)
    {
        if (shape.GetType() == typeof(Circle))
        {
            var circle = (Circle)shape;
            _circles.Add(circle);
            _markers.Add(new Marker()
            {
                Coordinates = circle.Center
            });
        }
    }


    private (string, string) CircleForQuery(Circle circle)
    {
        var points = CircleToPolygon(circle);
        var color = GetRandomColor();
        var polyline = string.Join(",", points.Select(x => $"{x.Item2},{x.Item1}"));
        return ($"polyline:{polyline};linewidth:3;linecolor:{HttpUtility.UrlEncode(color).ToLower()}", color);
    }

    private string MarkerForQuery(Marker marker)
    {
        return $"lonlat:{marker.Coordinates.Lon},{marker.Coordinates.Lat};type:material;color:red;size:small;iconsize:small;textsize:small;shadow:no";
    }

    private List<(double, double)> CircleToPolygon(Circle circle)
    {

        var points = new List<(double, double)>();
        for (int i = 0; i <= 360; i+=9)
        {
            var center = new Coordinate(circle.Center.Lat, circle.Center.Lon);
            center.Move(circle.Radius, i, CoordinateSharp.Shape.Sphere);

            points.Add((center.Latitude.DecimalDegree, center.Longitude.DecimalDegree));
        }

        return points;
    }

    private string GetRandomColor()
    {
        var color = string.Format("#{0:X6}", _random.Next(0x1000000));
        return color;
    }
}
