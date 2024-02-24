using System.Reflection;
using EmissionWiz.Models.Attributes;

namespace EmissionWiz.Models.Helpers;

public static class ExpandoObjectBuilder
{
    public static IDictionary<string, object?>? FromObject(object? obj, bool includeLevel = true)
    {
        return Load(obj, flags: BindingFlags.Static | BindingFlags.Public, includeLevel: includeLevel);
    }

    private static IDictionary<string, object?>? Load(object? obj, IDictionary<string, object?>? res = null, string level = "", BindingFlags flags = BindingFlags.Default, bool includeLevel = false)
    {
        if (obj == null)
            return null;

        var result = res ?? new Dictionary<string, object?>();
        foreach (var fields in obj.GetType().GetFields(flags))
        {
            var propType = fields.FieldType;
            var underlyingType = Nullable.GetUnderlyingType(propType);
            propType = underlyingType != null ? underlyingType : propType;

            if (propType.IsPrimitive || propType == typeof(decimal) || propType == typeof(string))
            {
                var name = includeLevel
                    ? $"{level}|{fields.Name}".Trim('|')
                    : fields.Name;

                if (!result.ContainsKey(name))
                    result.Add(name, fields.GetValue(obj));
            }
            else
            {
                Load(fields.GetValue(obj), result, level, flags, includeLevel);
            }
        }

        foreach (var prop in obj.GetType().GetProperties().Where(x => x.GetCustomAttribute<ExpandoIgnoreAttribute>() == null))
        {
            var propType = prop.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propType);
            propType = underlyingType != null ? underlyingType : propType;

            if (propType.IsPrimitive || propType == typeof(decimal) || propType == typeof(string) || propType == typeof(Nullable<decimal>))
            {
                var name = includeLevel
                    ? $"{level}|{prop.Name}".Trim('|')
                    : prop.Name;

                if (!result.ContainsKey(name))
                    result.Add(name, prop.GetValue(obj) ?? "");
            }
            else
            {
                Load(prop.GetValue(obj), result, level, flags, includeLevel);
            }
        }

        foreach (var nestedType in obj.GetType().GetNestedTypes().Where(x => !x.ContainsGenericParameters))
        {
            Load(Activator.CreateInstance(nestedType), result, nestedType.Name, flags, includeLevel);
        }

        return result;
    }
}
