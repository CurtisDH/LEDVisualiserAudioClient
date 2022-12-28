using NAudio.Dsp;
using NAudio.Wave;

namespace AudioClient;

public class AudClient
{
    private readonly double _threshold;
    private const int sampleRate = 32000;
    private Int16[] dataPcm;
    double[] dataFft;
    private WaveFormat captureWF;

    public AudClient(double threshold)
    {
        _threshold = threshold;
    }
    
    public void Init()
    {
        using (var capture = new WasapiLoopbackCapture())

        {
            capture.WaveFormat = new WaveFormat(sampleRate, 16, 1);
            captureWF = capture.WaveFormat;
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
        int bytesPerSample = captureWF.BitsPerSample / 8;
        int samplesRecorded = e.BytesRecorded / bytesPerSample;

        // reallocating seems stupid here, but the values vary in size.
        // perhaps it is better to use a value that will fit the size regardless?
        dataPcm = new Int16[samplesRecorded];
        for (int i = 0; i < samplesRecorded; i++)
        {
            dataPcm[i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample);
        }

        // the PCM size to be analyzed with FFT must be a power of 2
        int fftPoints = 2;
        while (fftPoints * 2 <= dataPcm.Length)
            fftPoints *= 2;

        // apply a Hamming window function as we load the FFT array then calculate the FFT
        NAudio.Dsp.Complex[] fftFull = new NAudio.Dsp.Complex[fftPoints];
        for (int i = 0; i < fftPoints; i++)
        {
            fftFull[i].X = (float)(dataPcm[i] * NAudio.Dsp.FastFourierTransform.HammingWindow(i, fftPoints));
        }

        NAudio.Dsp.FastFourierTransform.FFT(true, (int)Math.Log(fftPoints, 2.0), fftFull);

        // copy the complex values into the double array that will be plotted

        // same as above... should we really be creating a new double?
        dataFft = new double[fftPoints / 2];
        for (int i = 0; i < fftPoints / 2; i++)
        {
            double fftLeft = Math.Abs(fftFull[i].X + fftFull[i].Y);
            double fftRight = Math.Abs(fftFull[fftPoints - i - 1].X + fftFull[fftPoints - i - 1].Y);
            dataFft[i] = fftLeft + fftRight;
            var y = dataFft[i];
            var freq = i * sampleRate / fftPoints;
            if (y > _threshold)
            {
                Console.WriteLine($"{y}, {freq}");
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
            }
        }
    }
}