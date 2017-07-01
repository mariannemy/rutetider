using System.Text.RegularExpressions;

namespace RuterApp.Lib
{
    public static class StringUtils
    {
        public static string GetNormalizedStationName(string stationName)
        {
            return Regex.Replace(stationName, "(\\[.*\\])", "").Trim();
        }
    }
}