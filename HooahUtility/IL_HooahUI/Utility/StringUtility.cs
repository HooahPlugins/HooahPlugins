using System.Text.RegularExpressions;

namespace Utility
{
    public static class StringUtility
    {
        public static string ToProperCase(this string text)
        {
            var str = Regex.Replace(text, "(?<=\\w)(?=[A-Z])", " ", RegexOptions.None);
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }
    }
}
