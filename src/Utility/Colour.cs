namespace AudioClient.Utility;

public class Colour
{
    // Can't use the System.Drawing.Color struct because we're casting to object[] in LEDStrip.cs

    public byte R { get; private set; }
    public byte G { get; private set; }
    public byte B { get; private set; }

    public Colour(byte r, byte g, byte b)
    {
        this.R = r;
        this.G = g;
        this.B = b;
    }
}