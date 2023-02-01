using System.Drawing;

namespace AudioClient.Utility;

public class ColourConvert
{
    public static Color NormaliseColour(Color color)
    {
        float r = ((float)color.R / 255) * 10;
        float g = ((float)color.G / 255) * 10;
        float b = ((float)color.B / 255) * 10;

        var colour = Color.FromArgb((byte)r, (byte)g, (byte)b);
        if (Program.Debug)
            Console.WriteLine(colour);
        return colour;
    }
}