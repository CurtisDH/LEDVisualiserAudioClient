using System.Drawing;
using AudioClient.Network;
using NAudio.Wave;

namespace AudioClient.Client;

public class AudClient
{
    private readonly double _threshold;
    private const int SampleRate = 44100;
    private short[] _dataPcm;
    private double[] _dataFft;
    private WaveFormat _captureWf;
    private readonly string _ip;
    private bool _sentRealPacket;
    private readonly int _port;
    private readonly LedStrip _strip;
    private int _speed;
    private Task? _loop = null;


    public AudClient(double threshold, string ip, int port, int stripSize = 150, int speed = 25)
    {
        _threshold = threshold;
        this._ip = ip;
        this._port = port;
        _strip = new LedStrip(stripSize);
        this._speed = speed;
    }

    // ReSharper disable once FunctionRecursiveOnAllPaths
    public async Task Init()
    {
        _loop = UpdateLoop();
        using (var capture = new WasapiLoopbackCapture())

        {
            Console.WriteLine("INIT!!");
            capture.WaveFormat = new WaveFormat(SampleRate, 16, 1);
            _captureWf = capture.WaveFormat;
            // Set up an event handler to receive the audio data
            capture.DataAvailable += OnDataAvailable!;

            // Start capturing audio
            capture.StartRecording();

            // Wait for the user to stop capturing audio
            Console.WriteLine("Press any key to reconnect to audio");
            Console.ReadKey();

            capture.DataAvailable -= OnDataAvailable!;
#pragma warning disable CS4014
            Init();
#pragma warning restore CS4014
            await _loop!;
        }
    }

    private async Task? UpdateLoop()
    {
        while (true)
        {
            if (!_strip.AllLedsOff())
            {
                _strip.IncrementStrip();


                var networkClient = new NetClient(_ip, _port);

                networkClient.SendData(_strip.GetByteArray());
            }
            // If we don't delay the device we are sending to will create a backlog of commands.
            await Task.Delay(_speed);

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
            // dividing for reduced brightness
            // var calc = (byte)(magnitude / 32);

            var calc = (byte)(magnitude / 32);
            if (Program.Debug)
            {
                Console.WriteLine($"Max magnitude: {magnitude}, At Frequency: {frequency}  Calc:{calc}");
            }

            byte colourR = 0;
            byte colourB = 0;
            byte colourG = 0;
            // TODO, this is still gross
            // Widen the frequency band to reduce the flickering? 200 - 300 instead of 100 jumps?
            Color[] colors = new Color[]
            {
                Color.FromArgb(calc,0,0),
                Color.FromArgb(calc,0,(byte)(calc/2)),
                Color.FromArgb((byte)(calc/1.25),0,0),
                Color.FromArgb((byte)(calc/4),0,calc),
                Color.FromArgb((byte)(calc/4),0,calc),
                Color.FromArgb(0,calc,(byte)(calc/1.25)),
                Color.FromArgb(0,calc,0),
                Color.FromArgb((byte)(calc/2),calc,0),
                Color.FromArgb((byte)(calc/3),calc,0),
                Color.FromArgb(calc,(byte)(calc/2),0),
                Color.FromArgb(calc, (byte)(calc/3),0),
                Color.FromArgb(calc,(byte)(calc/4),0),
                Color.FromArgb((byte)(calc/4),calc,(byte)(calc/1.2)),
                Color.FromArgb(0,0,calc),
                Color.FromArgb(0,calc,0),
                Color.FromArgb(0,calc,(byte)(calc/1.25)),
                Color.FromArgb(0,(byte)(calc/2),calc)

            };
            // assign out of range values

            colourR = calc;
            colourG = calc;
            colourB = calc;

            for (int i = 0; i < colors.Length; i++)
            {
                var freqRange = i * 100;

                if (frequency <= freqRange && frequency >= i * 75)
                {
                    colourR = colors[i].R;
                    colourB = colors[i].G;
                    colourG = colors[i].B;
                }
            }

            var colour = Color.FromArgb(colourR, colourB, colourG);
            Console.WriteLine($"Colour:{colour}");
            _strip.IncrementStrip(colour);

            var networkClient = new NetClient(_ip, _port);
            networkClient.SendData(_strip.GetByteArray());
            _sentRealPacket = true;
            return;
        }
        else if (_sentRealPacket)
        {
            _strip.IncrementStrip(Color.Black);
            var networkClient = new NetClient(_ip, _port);
            networkClient.SendData(_strip.GetByteArray());
            _sentRealPacket = false;
        }
    }
}