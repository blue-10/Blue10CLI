using System.Globalization;
using System.Linq;

namespace Blue10CLI
{
    public static class Region
    {
        private static CultureInfo[] Cultures => CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        private static RegionInfo[] All => Cultures.Select(x => new RegionInfo(x.LCID)).ToArray();

        public static string[] Codes => All
            .Select(x => x.TwoLetterISORegionName.ToUpper())
            .ToArray();

        public static string[] Currencies => All
            .Select(x => x.ISOCurrencySymbol.ToUpper())
            .ToArray();
    }
}