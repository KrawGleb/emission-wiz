using System.Security.Cryptography;
using System.Text;

namespace EmissionWiz.API.Code.Helpers;

public class ContentSecurityPolicyBuilder
{
    public ContentSecurityPolicyBuilder(string initialPolicy)
    {
        var cspDictionary = initialPolicy
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .ToDictionary(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0], x => x.Split(' ').Skip(1).ToList());

        Default.Values.AddRange(cspDictionary.TryGetValue(Default.Key, out var defaultVal) ? defaultVal : Enumerable.Empty<string>());
        Style.Values.AddRange(cspDictionary.TryGetValue(Style.Key, out var style) ? style : Enumerable.Empty<string>());
        StyleElem.Values.AddRange(cspDictionary.TryGetValue(StyleElem.Key, out var styleElem) ? styleElem : Enumerable.Empty<string>());
        Connect.Values.AddRange(cspDictionary.TryGetValue(Connect.Key, out var connect) ? connect : Enumerable.Empty<string>());
        Script.Values.AddRange(cspDictionary.TryGetValue(Script.Key, out var script) ? script : Enumerable.Empty<string>());
        Frame.Values.AddRange(cspDictionary.TryGetValue(Frame.Key, out var frame) ? frame : Enumerable.Empty<string>());
        FrameSrc.Values.AddRange(cspDictionary.TryGetValue(FrameSrc.Key, out var frameSrc) ? frameSrc : Enumerable.Empty<string>());
    }

    public ContentSecurityPolicyItem Default { get; set; } = new("default-src");
    public ContentSecurityPolicyItem Style { get; set; } = new("style-src");
    public ContentSecurityPolicyItem StyleElem { get; set; } = new("style-src-elem");
    public ContentSecurityPolicyItem Connect { get; set; } = new("connect-src");
    public ContentSecurityPolicyItem Script { get; set; } = new("script-src");
    public ContentSecurityPolicyItem Frame { get; set; } = new("frame-ancestors");
    public ContentSecurityPolicyItem FrameSrc { get; set; } = new("frame-src");

    public string Build()
    {
        var items = new List<ContentSecurityPolicyItem> { Default, Style, StyleElem, Connect, Script, Frame };

        var sb = new StringBuilder();

        foreach (var item in items)
        {
            if (item.Values.Any())
                sb.Append($"{item.Key} {item.Build()};");
        }

        return sb.ToString().Trim(';');
    }
}

public class ContentSecurityPolicyItem
{
    public List<string> Values { get; set; } = new();
    public string Key { get; set; }

    public ContentSecurityPolicyItem(string key)
    {
        Key = key;
    }

    public string Build()
    {
        return string.Join(' ', Values.Distinct());
    }

    public ContentSecurityPolicyItem Add(string value)
    {
        Values.Add(value);
        return this;
    }

    public ContentSecurityPolicyItem AddSelf()
    {
        Values.Add("'self'");
        return this;
    }

    public ContentSecurityPolicyItem AddHttp()
    {
        Values.Add("http:");
        return this;
    }

    public ContentSecurityPolicyItem AddHttps()
    {
        Values.Add("https:");
        return this;
    }

    public ContentSecurityPolicyItem AddWs()
    {
        Values.Add("ws:");
        return this;
    }

    public ContentSecurityPolicyItem AddData()
    {
        Values.Add("data:");
        return this;
    }

    public ContentSecurityPolicyItem AddBlob()
    {
        Values.Add("blob:");
        return this;
    }

    public ContentSecurityPolicyItem AddUnsafeInline()
    {
        Values.Add("'unsafe-inline'");
        return this;
    }

    public ContentSecurityPolicyItem AddSha256(string content)
    {
        var hash = ComputeSha256(content);
        Values.Add($"'sha256-{hash}'");
        return this;
    }

    private static string ComputeSha256(string source)
    {
        var algorithm = SHA256.Create();
        var bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(source));

        return Convert.ToBase64String(bytes);
    }
}
