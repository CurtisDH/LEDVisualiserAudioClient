using NAudio.Dsp;
using NAudio.Wave;

namespace AudioClient;

public class AudClient
{
    private readonly double _threshold;
    private const int sampleRate = 44100;

    public AudClient(double threshold)
    {
        _threshold = threshold;
    }

    public void Init()
    {
        using (WasapiLoopbackCapture capture = new())
        {
            capture.WaveFormat = new WaveFormat(sampleRate, 16, 1);
            // Set up an event handler to receive the audio data
            capture.DataAvailable += OnDataAvailable;

            // Start capturing audio
            capture.StartRecording();

            // Wait for the user to stop capturing audio
            Console.WriteLine("Press any key to stop capturing audio...");
            Console.ReadKey();

            // Stop capturing audio
            capture.StopRecording();
        }
    }

    void OnDataAvailable(object sender, WaveInEventArgs e)
    {
        // Create an array to hold the complex numbers representing the audio data
        Complex[] audioData = new Complex[e.BytesRecorded / 2];

        // Fill the array with the audio data
        for (int i = 0; i < e.BytesRecorded / 2; i++)
        {
            audioData[i].X = (float)((short)(e.Buffer[i * 2] | e.Buffer[i * 2 + 1] << 8)) / 32768;
            audioData[i].Y = 0;
        }

        // Perform the FFT on the audio data
        FastFourierTransform.FFT(true, (int)Math.Log(audioData.Length, 2.0), audioData);

        // Add the data from the power spectrum to the line series
        for (int i = 2300; i < audioData.Length; i++)
        {
            
            // TODO play with colour spectrum's and shifting at random intervals
            
            // TODO only take the peak of the sine wave instead of the 'build ups'
            
            
            var y = Math.Sqrt(audioData[i].X * audioData[i].X + audioData[i].Y * audioData[i].Y);
            if (y > _threshold)
            {
                Console.WriteLine($"{y}, {i}");
                byte colourR = 0;
                byte colourG = 0;
                byte colourB = 0;
                byte delay = 0;
                var calc = (byte)(y * 1000);
                
                
                
                

                var ledColour = (byte)(calc * 2);
                if (y < _threshold * 3)
                {
                    colourB = ledColour;
                    delay = 50;
                }
                else
                {
                    colourB = ledColour;
                    colourR = ledColour;
                    delay = 25;
                }

                if (y < _threshold / 2)
                {
                    colourR = 0;
                    colourG = 0;
                    colourB = 0;
                    delay = 0;
                }

                byte brightness = calc;
                byte numLeds = (byte)(calc / 2);

                byte[] meaningfulData = new[] { numLeds, colourR, colourG, colourB, brightness, delay };
                //Console.WriteLine(y);
                var networkClient = new NetClient("192.168.1.11", 5555);
                networkClient.SendData(meaningfulData);
                // now we want to determine what to send
            }
        }
    }
}