namespace EmissionWiz.Common.Helpers;

public static class ExpandoObjectBuilder
{
    public static IDictionary<string, string>? FromInstance(object? obj, IDictionary<string, string>? res = null, string level = "")
    {
        if (obj == null)
            return null;

        var result = res ?? new Dictionary<string, string>();
        foreach (var prop in obj.GetType().GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
        {
            var name = $"{level}|{prop.Name}".Trim('|');
            result.Add(name, prop.GetValue(obj)?.ToString() ?? "");
        }

        foreach (var nestedType in obj.GetType().GetNestedTypes())
        {
            FromInstance(Activator.CreateInstance(nestedType), result, nestedType.Name);
        }

        return result;
    }
}
