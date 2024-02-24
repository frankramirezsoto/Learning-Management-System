using System.Drawing;

namespace CanvasLMS.Utilities
{
    public class ColorUtility
    {
        public static string GetContrastTextColor(string hexColor)
        {
            Color color = ColorTranslator.FromHtml(hexColor);
            double brightness = (color.R * 0.299 + color.G * 0.587 + color.B * 0.114) / 255;

            return brightness > 0.5 ? "#000000" : "#FFFFFF";
        }

        public static string DarkenColor(string hexColor, double percent)
        {
            if (hexColor.StartsWith("#"))
            {
                hexColor = hexColor.Substring(1);
            }

            int red = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int green = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int blue = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            red = (int)Math.Floor(red * (1 - percent));
            green = (int)Math.Floor(green * (1 - percent));
            blue = (int)Math.Floor(blue * (1 - percent));

            return $"#{red:X2}{green:X2}{blue:X2}";
        }
    }
}
