using System.Linq.Expressions;

namespace EmissionWiz.Models;

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

    public static class MathChars
    {
        public static class Lower
        {
            public const char m = 'ₘ';
        }

        public const char GoE = '≥';
        public const char LoE = '≤';

        public const char f = 'ƒ';

    }
}
