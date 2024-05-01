using EmissionWiz.Models.Helpers;

namespace EmissionWiz.Models;

public static class Constants
{
    public static class ContentType
    {
        public const string Tiff = "image/tiff";
        public const string Pdf = "application/pdf";
        public const string Xls = "application/vnd.ms-excel";
    }

    public static class Pdf
    {
        public static class CustomStyleNames
        {
            public const string Table = "Table";
            public const string Title = "Title";

            public const string SmallText = "SmallText";
        }
    }

    public class SpecialChars
    {
        public class Lower
        {
            public const char m = 'ₘ';
            public const char e = 'ₑ';
            public const char dot = '̣';

        }

        public const char GoE = '≥';
        public const char LoE = '≤';

        public const char Delta = 'Δ';
        public const char f = 'ƒ';

        public const char Square = '█';
    }

    public static IDictionary<string, object?>? SpecialCharsObj = ExpandoObjectBuilder.FromObject(new SpecialChars(), true);

    public static class MapKeys
    {
        public static class SingleSource
        {
            public const string XmDistance = nameof(XmDistance);
            public const string XmuDistance = nameof(XmuDistance);
        }
    }

    public static class HttpClientName
    {
        public const string GeoApify = nameof(GeoApify);
    }

    public static class GeoApi
    {
        public const int MaxHeightAndWidth = 4096;
    }
}



