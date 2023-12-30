using EmissionWiz.Models;
using HandlebarsDotNet;
using System.Globalization;

namespace EmissionWiz.Common.Templates;

public static class HbsTemplateManager
{
    static HbsTemplateManager()
    {
        Handlebars.RegisterHelper("trimByPrecision", (output, context, data) =>
        {
            var format = $"0.{new string('#', 3)}";

            var number = data[0];
            if (number is int i)
            {
                output.WriteSafeString(i.ToString(CultureInfo.InvariantCulture));
            }
            if (number is decimal d)
            {
                output.WriteSafeString(d.ToString(format, CultureInfo.InvariantCulture));
            }
            if (number is float f)
            {
                output.WriteSafeString(f.ToString(format, CultureInfo.InvariantCulture));
            }
            if (number is double dd)
            {
                output.WriteSafeString(dd.ToString(format, CultureInfo.InvariantCulture));
            }
        });
        Handlebars.RegisterHelper("math", (output, context, data) =>
        {
            var name = data[0].ToString();
            if (name != null && Constants.MathCharsObj != null && Constants.MathCharsObj.ContainsKey(name))
            {
                output.WriteSafeString(Constants.MathCharsObj[name]);
            }
            else
            {
                output.WriteSafeString(name);
            }
        });
    }

    public static string Format(string? template, object? model)
    {
        if (model == null)
            return template ?? "";

        if (string.IsNullOrEmpty(template))
            return "";
 
        var templateCompiled = Handlebars.Compile(template);

        return templateCompiled(model);
    }
}
