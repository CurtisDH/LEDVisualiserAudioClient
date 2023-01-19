using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;

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
        Console.WriteLine(sw.Elapsed);
    }

    public byte[] GetByteArray()
    {
        byte[] bytes = new byte[Strip.Length * 3];
        for (int i = 0; i < Strip.Length; i++)
        {
            var index = i * 3;
            bytes[index] = Strip[i].R;
            bytes[index + 1] = Strip[i].G;
            bytes[index + 2] = Strip[i].B;
        }

        return bytes;
    }

    public Color[] ConvertByteArrayToColorArray(byte[] bytes)
    {
        Color[] colors = new Color[bytes.Length / 3];
        for (int i = 0; i < bytes.Length / 3; i++)
        {
            var byteArrayIndex = i + 3;

            colors[i] = Color.FromArgb(
                bytes[byteArrayIndex], bytes[byteArrayIndex + 1], bytes[byteArrayIndex + 2]);
        }

        return colors;
    }
}