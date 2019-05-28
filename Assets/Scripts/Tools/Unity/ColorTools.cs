using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Tools
{
    public static class ColorTools
    {
        public static Color FromHex(string pHexString)
        {
            Regex regex = new Regex(@"^#?(?i)(?<r>[0-9A-F]{2})(?<g>[0-9A-F]{2})(?<b>[0-9A-F]{2})(?<a>[0-9A-F]{2})?(?-i)$");
            Match match = regex.Match(pHexString);
            if(!match.Success)
                throw new ArgumentException("String must have the following format: [#]RRGGBB[AA]");

            byte r = byte.Parse(match.Groups["r"].Value, System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(match.Groups["g"].Value, System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(match.Groups["b"].Value, System.Globalization.NumberStyles.HexNumber);
            byte a = byte.MaxValue;

            Group alpha = match.Groups["a"];
            if(alpha.Value != string.Empty)
            {
                a = byte.Parse(alpha.Value, System.Globalization.NumberStyles.HexNumber);
            }

            return new Color32(r, g, b, a);
        }
    }
}
