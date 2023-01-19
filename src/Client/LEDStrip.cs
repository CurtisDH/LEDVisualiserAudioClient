using System.Diagnostics;
using System.Drawing;

namespace AudioClient.Client;

public class LedStrip
{
    public Color[] Strip;

    public LedStrip(int stripLength)
    {
        Strip = new Color[stripLength];
    }

    public void IncrementStrip(Color color)
    {
        Strip[0] = color;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        for (int i = Strip.Length - 2; i >= 0; i--)
        {
            Strip[i + 1] = Strip[i];
        }

        sw.Stop();
        if (Program.Debug)
            Console.WriteLine(sw.Elapsed);
    }

    public void IncrementStrip()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        for (int i = Strip.Length - 2; i >= 0; i--)
        {
            Strip[i + 1] = Strip[i];
        }

        sw.Stop();
        if (Program.Debug)
            Console.WriteLine(sw.Elapsed);
    }

    public byte[] GetByteArray()
    {
        Stopwatch sw = new Stopwatch();

        byte[] bytes = new byte[Strip.Length * 3];
        for (int i = 0; i < Strip.Length; i++)
        {
            var index = i * 3;
            bytes[index] = Strip[i].R;
            bytes[index + 1] = Strip[i].G;
            bytes[index + 2] = Strip[i].B;
        }

        sw.Stop();
        if (Program.Debug)
            Console.WriteLine(sw.Elapsed);

        return bytes;
    }

    public bool AllLedsOff()
    {
        foreach (var color in Strip)
        {
            if (color.R > 0 || color.G > 0 || color.B > 0)
            {
                return false;
            }
        }

        return true;
    }

    public Color[] ConvertByteArrayToColorArray(byte[] bytes)
    {
        Console.WriteLine(bytes.Length);
        Color[] colors = new Color[bytes.Length / 3];
        for (int i = 0; i < bytes.Length / 3; i++)
        {
            Console.WriteLine(i);
            Console.WriteLine(i * 3);
            var byteArrayIndex = i * 3;

            colors[i] = Color.FromArgb(
                bytes[byteArrayIndex], bytes[byteArrayIndex + 1], bytes[byteArrayIndex + 2]);
        }

        foreach (var VARIABLE in colors)
        {
            Console.WriteLine(VARIABLE);
        }

        return colors;
    }
}