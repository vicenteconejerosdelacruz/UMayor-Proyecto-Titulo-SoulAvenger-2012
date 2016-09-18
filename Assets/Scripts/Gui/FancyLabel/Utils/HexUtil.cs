using UnityEngine;
public static class HexUtil
{
    /// <summary>
    /// Convert an hexadecimal value into an integer value
    /// </summary>
    /// <param name="value">The hexadecimal value</param>
    /// <returns>The integer value</returns>
    public static int HexToInt(string value)
    {
        int intValue;
        try
        {
            intValue = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
        }
        catch (System.Exception)
        {
            intValue = 0;
        }
        return intValue;
    }

    /// <summary>
    /// Convert an integer value into an hexadecimal value
    /// </summary>
    /// <param name="value">The integer value</param>
    /// <returns>The hexadecimal value</returns>
    public static string IntToHex(int value)
    {
        string hexValue = value.ToString("X");
        if (hexValue.Length == 1)
        {
            hexValue = "0" + hexValue;
        }
        return hexValue;
    }

    /// <summary>
    /// Convert a float value into an hexadecimal value
    /// </summary>
    /// <param name="value">The float value</param>
    /// <returns>The hexadecimal value</returns>
    public static string FloatToHex(float value)
    {
        return IntToHex(int.Parse(value.ToString()));
    }

    /// <summary>
    /// Convert an hexadecimal color representation RRGGBBAA into a Color value
    /// </summary>
    /// <param name="value">The hexadecimal color</param>
    /// <param name="color">The corresponding color</param>
    /// <returns>Whether the returned color is valid or not</returns>
    public static bool HexToColor(string value, out Color color)
    {
        if (value.Length != 8)
        {
            color = Color.white;
            return false;
        }

        string rText = value.Substring(0, 2);
        string gText = value.Substring(2, 2);
        string bText = value.Substring(4, 2);
        string aText = value.Substring(6, 2);

        float r = HexUtil.HexToInt(rText) / 255.0f;
        float g = HexUtil.HexToInt(gText) / 255.0f;
        float b = HexUtil.HexToInt(bText) / 255.0f;
        float a = HexUtil.HexToInt(aText) / 255.0f;
        if (r < 0 || g < 0 || b < 0 || a < 0)
        {
            color = Color.white;
            return false;
        }

        color = new Color(r, g, b, a);
        return true;
    }

    /// <summary>
    /// Convert a Color value into an hexadecimal representation RRGGBBAA
    /// </summary>
    /// <param name="color">The color to convert</param>
    /// <returns>The hexadecimal representation of the color</returns>
    public static string ColorToHex(Color color)
    {
        return FloatToHex(color.r * 255.0f)
             + FloatToHex(color.g * 255.0f)
             + FloatToHex(color.b * 255.0f)
             + FloatToHex(color.a * 255.0f);
    }

}
