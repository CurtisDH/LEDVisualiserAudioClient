using System.Drawing;

namespace AudioClient.Utility;

public class ColourController
{
    // TODO change the colour palette based on the speed of the music
    // TODO blend colours
    /*
     * Deep Blue: This color evokes feelings of calm and tranquility, making it a great choice for slow, mellow music.

    Electric Purple: This color is bold and vibrant, making it a great choice for upbeat, high-energy music.

    Forest Green: This color is earthy and grounding, making it a great choice for music with a more organic or natural feel.

    Bright Yellow: This color is cheerful and uplifting, making it a great choice for happy, positive music.

    Burgundy Red: This color is rich and intense, making it a great choice for dramatic or emotional music.

    Neutral gray: This color is versatile and can work well with a variety of music styles and frequencies.
     */

    public static Color Blend(Color initial, Color previous)
    {
        // half all the values currently in the initial and the previous, and then add them together.
        // ?? This will probably result in more white 
        // solution, create a weight system? initial colour has more weight than the previous?
        float weight = 0.75f;


        var initialR = ((float)initial.R * weight);
        var initialG = ((float)initial.G * weight);
        var initialB = ((float)initial.B * weight);

        var pR = ((float)previous.R * Math.Abs(weight - 1));
        var pG = ((float)previous.G * Math.Abs(weight - 1));
        var pB = ((float)previous.B * Math.Abs(weight - 1));

        var calculatedR = (byte)(initialR + pR);
        var calculatedG = (byte)(initialG + pG);
        var calculatedB = (byte)(initialB + pB);


        return Color.FromArgb(calculatedR, calculatedB, calculatedG);
    }

    // colours start from freq 0 and jumps in x increments -- currently at AudClient.cs int Increment
    public static readonly Color[] Colours = new Color[]
    {
        ColourController.NormaliseColour(Color.Black), // First entry ignored.
        ColourController.NormaliseColour(Color.Red),
        ColourController.NormaliseColour(Color.Purple),
        ColourController.NormaliseColour(Color.Brown),
        ColourController.NormaliseColour(Color.Crimson),
        ColourController.NormaliseColour(Color.Navy),
        ColourController.NormaliseColour(Color.DeepSkyBlue),
        ColourController.NormaliseColour(Color.BlueViolet),
        ColourController.NormaliseColour(Color.SkyBlue),
        ColourController.NormaliseColour(Color.DeepSkyBlue),
        ColourController.NormaliseColour(Color.HotPink),
        ColourController.NormaliseColour(Color.DeepPink),
        ColourController.NormaliseColour(Color.Fuchsia),
        ColourController.NormaliseColour(Color.LightBlue),
        ColourController.NormaliseColour(Color.LightBlue),
        ColourController.NormaliseColour(Color.LightBlue),
        ColourController.NormaliseColour(Color.LightBlue),
        ColourController.NormaliseColour(Color.LightBlue),
        ColourController.NormaliseColour(Color.White),
        ColourController.NormaliseColour(Color.White),
        ColourController.NormaliseColour(Color.White),
        ColourController.NormaliseColour(Color.White),
        ColourController.NormaliseColour(Color.White),
        ColourController.NormaliseColour(Color.White)
    };

    private static Color NormaliseColour(Color color)
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