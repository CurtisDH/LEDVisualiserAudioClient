using System.Drawing;

namespace AudioClient.Utility;

public static class ColourController
{
    // TODO change the colour palette based on the speed of the music
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


        var initialR = (initial.R * weight);
        var initialG = (initial.G * weight);
        var initialB = (initial.B * weight);

        var pR = (previous.R * Math.Abs(weight - 1));
        var pG = (previous.G * Math.Abs(weight - 1));
        var pB = (previous.B * Math.Abs(weight - 1));

        var calculatedR = (byte)(initialR + pR);
        var calculatedG = (byte)(initialG + pG);
        var calculatedB = (byte)(initialB + pB);


        return Color.FromArgb(calculatedR, calculatedG, calculatedB);
    }

    // colours start from freq 0 and jumps in x increments -- currently at AudClient.cs int Increment
    public static readonly Color[] Colours =
    {
        NormaliseColour(Color.Black), // First entry ignored.
        NormaliseColour(Color.Red),
        NormaliseColour(Color.Purple),
        NormaliseColour(Color.Brown),
        NormaliseColour(Color.Crimson),
        NormaliseColour(Color.Navy),
        NormaliseColour(Color.DeepSkyBlue),
        NormaliseColour(Color.BlueViolet),
        NormaliseColour(Color.SkyBlue),
        NormaliseColour(Color.DeepSkyBlue),
        NormaliseColour(Color.DeepSkyBlue),
        NormaliseColour(Color.DeepSkyBlue),
        NormaliseColour(Color.DeepSkyBlue),
        NormaliseColour(Color.LightBlue),
        NormaliseColour(Color.LightBlue),
        NormaliseColour(Color.LightBlue),
        NormaliseColour(Color.LightBlue),
        NormaliseColour(Color.LightBlue),
        NormaliseColour(Color.White),
        NormaliseColour(Color.White),
        NormaliseColour(Color.White),
        NormaliseColour(Color.White),
        NormaliseColour(Color.White),
        NormaliseColour(Color.White)
    };

    public static Color[] GetColourArray()
    {
        // check elapsed time then give a different spectrum of colours?
        return Colours;
    }

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