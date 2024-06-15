using EmissionWiz.API.Code.Helpers;
using System.Reflection;

namespace EmissionWiz.API.Code.Middleware;

public class HtmlFallbackMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public HtmlFallbackMiddleware(
        RequestDelegate next,
        IWebHostEnvironment hostingEnvironment)
    {
        _next = next;
        _hostingEnvironment = hostingEnvironment;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.GetEndpoint() != null)
        {
            await _next(context);
            return;
        }

        var response = context.Response;

        var viteClient = """<script type="module" src="/@vite/client"></script>""";
        var viteModule = "\nimport RefreshRuntime from \"/@react-refresh\"\n" +
                         "RefreshRuntime.injectIntoGlobalHook(window)\n" +
                         "window.$RefreshReg$ = () => { }\n" +
                         "window.$RefreshSig$ = () => (type) => type\n" +
                         "window.__vite_plugin_react_preamble_installed__ = true\n";

        var indexHtmlPath = _hostingEnvironment.IsDevelopment()
            ? Path.Combine(_hostingEnvironment.ContentRootPath, "..", "EmissionWiz.Client", "index.html")
            : Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "index.html");

        var html = await File.ReadAllTextAsync(indexHtmlPath);

        var isDevelopment = _hostingEnvironment.IsDevelopment();

        if (isDevelopment)
        {
            html = html.Replace("<head>", "<head>" + Environment.NewLine + viteClient);
            html = html.Replace("<head>", "<head>" + Environment.NewLine + $"<script type=\"module\">{viteModule}</script>");
        }

        //var cspBuilder = new ContentSecurityPolicyBuilder(response.Headers.ContentSecurityPolicy.ToString());

        //cspBuilder.Script
        //    .AddSelf()
        //    .AddSha256(viteModule);

        //cspBuilder.Style
        //    .AddUnsafeInline();

        //cspBuilder.StyleElem
        //    .AddUnsafeInline();

        //response.Headers.ContentSecurityPolicy = cspBuilder.Build();

        response.Headers.ContentType = "text/html";

        await response.WriteAsync(html);
        await response.CompleteAsync();
    }

}

public static class HtmlFallbackMiddlewareExtension
{
    public static IApplicationBuilder UseHtmlFallback(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HtmlFallbackMiddleware>();
    }
}