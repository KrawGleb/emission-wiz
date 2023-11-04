using EmissionWiz.Models;
using HandlebarsDotNet;
using System.Globalization;

namespace EmissionWiz.Logic.Managers;

public static class HbsTemplateManager
{
    static HbsTemplateManager()
    {
        Handlebars.RegisterHelper("trimByPrecision", (output, context, data) =>
        {
            var format = $"0.{new string('#', Constants.Templates.StandartPrecision)}";

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
    }

    public static string Format(string template, object model)
    {
        var templateCompiled = Handlebars.Compile(template);

        return templateCompiled(model);
    }
}
