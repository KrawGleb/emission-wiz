using System.Net.Sockets;
using System.Net;

namespace EmissionWiz.API.Code.Helpers;

/// <summary>
/// Original: https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/Spa/SpaServices.Extensions/src/Util/TcpPortFinder.cs
/// </summary>
internal static class TcpPortFinder
{
    public static int FindAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }
        finally
        {
            listener.Stop();
        }
    }
}