using EmissionWiz.Common.Helpers;

namespace EmissionWiz.Common;

public static class Constants
{
    public static class Pdf
    {
        public static class CustomStyleNames
        {
            public const string Table = "Table";
            public const string Title = "Title";
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
        }

        public const char GoE = '≥';
        public const char LoE = '≤';
        public const string G = "&gt;";
        public const string L = "&lt;";

        public const char Delta = 'Δ';
        public const char f = 'ƒ';
    }

    public static IDictionary<string, object?>? MathCharsObj = ExpandoObjectBuilder.FromObject(new MathChars(), true)
        
        
        
        
        
        
        
        
        
        
        
     ;
}



