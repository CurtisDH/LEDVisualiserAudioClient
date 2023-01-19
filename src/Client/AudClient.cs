using System.Drawing;
using AudioClient.Network;
using NAudio.Wave;

namespace AudioClient.Client;

public class AudClient
{
    private readonly double _threshold;
    private const int SampleRate = 44100;
    private Int16[] _dataPcm;
    double[] _dataFft;
    private WaveFormat _captureWf;
    private string ip;
    private bool _sentRealPacket;
    private int port;
    private LedStrip strip;


    public AudClient(double threshold, string ip, int port, int stripSize = 150)
    {
        _threshold = threshold;
        this.ip = ip;
        this.port = port;
        strip = new LedStrip(stripSize);
    }

    public async Task Init()
    {
        var loop = UpdateLoop();
        using (var capture = new WasapiLoopbackCapture())

        {
            Console.WriteLine("INIT!!");
            capture.WaveFormat = new WaveFormat(SampleRate, 16, 1);
            _captureWf = capture.WaveFormat;
            // Set up an event handler to receive the audio data
            capture.DataAvailable += OnDataAvailable;

            // Start capturing audio
            capture.StartRecording();

            // Wait for the user to stop capturing audio
            Console.WriteLine("Press any key to reconnect to audio");
            Console.ReadKey();

            capture.DataAvailable -= OnDataAvailable;
            Init();
            await loop;
        }
    }

    private async Task UpdateLoop()
    {
        while (true)
        {
            // If we don't delay the device we are sending to will create a backlog of commands.
            strip.IncrementStrip();
            var networkClient = new NetClient(ip, port);
            networkClient.SendData(strip.GetByteArray());
            await Task.Delay(5);

        }
    }


    void OnDataAvailable(object sender, WaveInEventArgs e)
    {
        int bytesPerSample = _captureWf.BitsPerSample / 8;
        int samplesRecorded = e.BytesRecorded / bytesPerSample;

        // reallocating seems stupid here, but the values vary in size.
        // perhaps it is better to use a value that will fit the size regardless?
        _dataPcm = new Int16[samplesRecorded];
        for (int i = 0; i < samplesRecorded; i++)
        {
            _dataPcm[i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample);
        }

        // the PCM size to be analyzed with FFT must be a power of 2
        int fftPoints = 2;
        while (fftPoints * 2 <= _dataPcm.Length)
            fftPoints *= 2;

        // apply a Hamming window function as we load the FFT array then calculate the FFT
        NAudio.Dsp.Complex[] fftFull = new NAudio.Dsp.Complex[fftPoints];
        for (int i = 0; i < fftPoints; i++)
        {
            try
            {
                fftFull[i].X = (float)(_dataPcm[i] * NAudio.Dsp.FastFourierTransform.HammingWindow(i, fftPoints));
            }
            catch
            {
                // ignored -- Prevents the program from breaking upon long audio pauses.
                // (Originally thought this was just a bluetooth audio device issue but have traced it back to here)
            }
        }

        NAudio.Dsp.FastFourierTransform.FFT(true, (int)Math.Log(fftPoints, 2.0), fftFull);


        _dataFft = new double[fftPoints / 2];

        double magnitude = 0;
        int frequency = 0;
        for (int i = 0; i < fftPoints / 2; i++)
        {
            double fftLeft = Math.Abs(fftFull[i].X + fftFull[i].Y);
            double fftRight = Math.Abs(fftFull[fftPoints - i - 1].X + fftFull[fftPoints - i - 1].Y);
            _dataFft[i] = fftLeft + fftRight;
            if (!(magnitude < _dataFft[i])) continue;

            magnitude = _dataFft[i];
            frequency = i * SampleRate / fftPoints;
        }


        if (magnitude > _threshold)
        {
            byte colourR = 0;
            byte colourG = 0;
            byte colourB = 0;
            // dividing for reduced brightness
            // var calc = (byte)(magnitude / 32);

            var calc = (byte)(magnitude / 32);
            if (Program.Debug)
            {
                Console.WriteLine($"Max magnitude: {magnitude}, At Frequency: {frequency}  Calc:{calc}");
            }

            // TODO, this is still gross
            // Widen the frequency band to reduce the flickering? 200 - 300 instead of 100 jumps?
            switch (frequency)
            {
                case >= 0 and <= 100:
                    colourR = calc;
                    colourG = 0;
                    colourB = 0;
                    break;
                case >= 100 and <= 200:
                    colourR = calc;
                    colourG = 0;
                    colourB = (byte)(calc / 2);
                    break;
                case >= 200 and <= 300:
                    colourR = (byte)(calc / 1.25);
                    colourG = 0;
                    colourB = calc;

                    break;
                case >= 300 and <= 400:
                    colourR = (byte)(calc / 4);
                    colourG = 0;
                    colourB = calc;

                    break;
                case >= 400 and <= 500:
                    colourR = 0;
                    colourG = calc;
                    colourB = (byte)(calc / 2);

                    break;
                case >= 600 and <= 700:
                    colourR = 0;
                    colourG = calc;
                    colourB = (byte)(calc / 1.25);

                    break;
                case >= 700 and <= 800:
                    colourR = 0;
                    colourG = calc;
                    colourB = 0;

                    break;
                case >= 800 and <= 900:
                    colourR = (byte)(calc / 2);
                    colourG = calc;
                    colourB = 0;

                    break;
                case >= 1000 and <= 1100:
                    colourR = (byte)(calc / 3);
                    colourG = calc;
                    colourB = 0;

                    break;
                case >= 1100 and <= 1200:
                    colourR = calc;
                    colourG = (byte)(calc / 2);
                    colourB = 0;

                    break;
                case >= 1200 and <= 1300:
                    colourR = calc;
                    colourG = (byte)(calc / 3);
                    colourB = 0;

                    break;
                case >= 1300 and <= 1400:
                    colourR = calc;
                    colourG = (byte)(calc / 4);
                    colourB = 0;

                    break;
                case >= 1400 and <= 1500:
                    colourR = (byte)(calc / 4);
                    colourG = calc;
                    colourB = (byte)(calc / 1.2);

                    break;
                case >= 1600 and <= 1700:
                    colourR = 0;
                    colourG = 0;
                    colourB = calc;

                    break;
                case >= 1700 and <= 1800:
                    colourR = 0;
                    colourG = calc;
                    colourB = 0;

                    break;
                case >= 1900 and <= 2000:
                    colourR = 0;
                    colourG = calc;
                    colourB = (byte)(calc / 4);

                    break;
                case >= 2000 and <= 2100:
                    colourR = 0;
                    colourG = calc;
                    colourB = (byte)(calc / 1.25);

                    break;
                case >= 2100 and <= 2200:
                    colourR = 0;
                    colourG = (byte)(calc / 2);
                    colourB = calc;

                    break;
                default:
                    if (Program.Debug)
                        Console.WriteLine($"Default, Frequency:{frequency}");
                    colourR = calc;
                    colourG = calc;
                    colourB = calc;
                    break;
            }

            var colour = Color.FromArgb(colourR, colourB, colourG);
            strip.IncrementStrip(colour);

            var networkClient = new NetClient(ip, port);
            networkClient.SendData(strip.GetByteArray());
            _sentRealPacket = true;
            return;
        }
        else if (_sentRealPacket)
        {
            strip.IncrementStrip(Color.Black);
            var networkClient = new NetClient(ip, port);
            networkClient.SendData(strip.GetByteArray());
            _sentRealPacket = false;
        }
    }
}