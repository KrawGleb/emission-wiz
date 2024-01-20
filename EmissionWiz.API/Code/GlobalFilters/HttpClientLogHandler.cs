namespace EmissionWiz.API.Code.GlobalFilters;
public class HttpClientLogHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var logger = httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<ILogger<HttpClientLogHandler>>();
        if (logger == null)
            return await base.SendAsync(request, cancellationToken);

        var requestContent = request.Content == null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        logger.LogInformation("Http request: {0}", request);
        if (requestContent != null)
            logger.LogInformation("Http request body: {0}", requestContent);

        var response = await base.SendAsync(request, cancellationToken);
        var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken);

        logger.LogInformation("Http response: {0}", responseAsString);

        return response;
    }
}
