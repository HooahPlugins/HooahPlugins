using System.Text.RegularExpressions;
using UnityEngine;

namespace AdvancedStudioUI.Utility
{
    public static class ColorUtility
    {
        private static Regex hexString = new Regex("^(#?)([A-F0-9]+)$", RegexOptions.IgnoreCase);
        private static Regex rgb = new Regex("^(#?)([A-F0-9]+)$", RegexOptions.IgnoreCase);

        public static bool ParseColorString(string value, out Color finalColor)
        {
            finalColor = Color.black;
        
            var matches = hexString.Match(value);
            if (!matches.Success) return false;
            if (!UnityEngine.ColorUtility.TryParseHtmlString($"#{matches.Groups[2]}", out var color)) return false;
            finalColor = color;
            return true;
        }
    }
}