using EmissionWiz.Models.Helpers;

namespace EmissionWiz.Models;

public static class Constants
{
    public static class Pdf
    {
        public static class CustomStyleNames
        {
            public const string Table = "Table";
            public const string Title = "Title";

            public const string SmallText = "SmallText";
        }
    }

    public static class Templates
    {
        public const int StandartPrecision = 3;
    }

    public class MathChars
    {
        public class Lower
        {
            public const char m = 'ₘ';
            public const char e = 'ₑ';
            public const char dot = '̣';

        }

        public const char GoE = '≥';
        public const char LoE = '≤';
        public const string G = "&gt;";
        public const string L = "&#60;";

        public const char Delta = 'Δ';
        public const char f = 'ƒ';
    }

    public static IDictionary<string, object?>? MathCharsObj = ExpandoObjectBuilder.FromObject(new MathChars(), true);

    public static class MapKeys
    {
        public static class SingleSource
        {
            public const string XmDistance = nameof(XmDistance);
            public const string DangerousDistance = nameof(DangerousDistance);
        }
    }

    public static class HttpClientName
    {
        public const string GeoApi = nameof(GeoApi);
    }

    public static class Math
    {
        public const double EarthRadius = 6_371 * 1_000;
    }
}



