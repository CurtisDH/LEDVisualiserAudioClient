using NAudio.Dsp;
using NAudio.Wave;

namespace AudioClient;

public class AudClient
{
    private readonly double _threshold;
    private const int sampleRate = 44100;
    private Int16[] dataPcm;
    double[] dataFft;
    private WaveFormat captureWF;
    private string ip;

    public AudClient(double threshold, string ip)
    {
        _threshold = threshold;
        this.ip = ip;
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

        /*
         * only take the value for highest magnitude 

        this could be split into freq ranges but this will reduce a lot of the "noise" and make it more beat focused?
         */


        dataFft = new double[fftPoints / 2];

        for (int i = 0; i < fftPoints / 2; i++)
        {
            double fftLeft = Math.Abs(fftFull[i].X + fftFull[i].Y);
            double fftRight = Math.Abs(fftFull[fftPoints - i - 1].X + fftFull[fftPoints - i - 1].Y);
            dataFft[i] = fftLeft + fftRight;
        }

        var y = dataFft.Max();
        //var freq = i * sampleRate / fftPoints;


        // if (freq < 45 && freq >= 25) // bass  -- threshold tbd

        // if (freq > 23 && freq < 120)
        if (y > _threshold)
        {
            Console.WriteLine($"{y}, {y / 4}");
            byte colourR = 0;
            byte colourG = 0;
            byte colourB = 0;
            byte delay = 0;
            // dividing for reduced brightness
            var calc = (byte)(y / 32);
            switch (y / 4)
            {
                case > 0 and < 21:
                    colourR = calc;
                    colourG = 0;
                    colourB = 0;
                    break;
                case > 21 and < 43:
                    colourR = calc;
                    colourG = 0;
                    colourB = (byte)(calc / 2);
                    break;
                case > 43 and < 64:
                    colourR = (byte)(calc / 1.25);
                    colourG = 0;
                    colourB = calc;

                    break;
                case > 64 and < 86:
                    colourR = (byte)(calc / 4);
                    colourG = 0;
                    colourB = calc;

                    break;
                case 86:
                    colourR = 0;
                    colourG = calc;
                    colourB = (byte)(calc / 2);

                    break;
                case 107:
                    colourR = 0;
                    colourG = calc;
                    colourB = (byte)(calc / 1.25);

                    break;
                case 129:
                    colourR = 0;
                    colourG = calc;
                    colourB = 0;

                    break;
                case 150:
                    colourR = (byte)(calc / 2);
                    colourG = calc;
                    colourB = 0;

                    break;
                case 172:
                    colourR = (byte)(calc / 3);
                    colourG = calc;
                    colourB = 0;

                    break;
                case 193:
                    colourR = calc;
                    colourG = (byte)(calc / 2);
                    colourB = 0;

                    break;
                case 215:
                    colourR = calc;
                    colourG = (byte)(calc / 3);
                    colourB = 0;

                    break;
                case 237:
                    colourR = calc;
                    colourG = (byte)(calc / 4);
                    colourB = 0;

                    break;
                case 258:
                    colourR = (byte)(calc / 4);
                    colourG = calc;
                    colourB = (byte)(calc / 1.2);

                    break;
                case 280:
                    colourR = 0;
                    colourG = 0;
                    colourB = calc;

                    break;
                case 301:
                    colourR = 0;
                    colourG = calc;
                    colourB = 0;

                    break;
                case 323:
                    colourR = 0;
                    colourG = calc;
                    colourB = (byte)(calc / 4);

                    break;
                case 344:
                    colourR = 0;
                    colourG = calc;
                    colourB = (byte)(calc / 1.25);

                    break;
                case 366:
                    colourR = 0;
                    colourG = (byte)(calc / 2);
                    colourB = calc;

                    break;
                default:
                    colourR = calc;
                    colourG = calc;
                    colourB = calc;
                    break;
            }


            byte brightness = calc;
            byte numLeds = (byte)(calc / 2);

            byte[] meaningfulData = new[] { numLeds, colourR, colourG, colourB, brightness, delay };
            //Console.WriteLine(y);
            var networkClient = new NetClient(ip, 5555);
            networkClient.SendData(meaningfulData);
            return;
        }
    }
}