using System.Drawing;
using AudioClient.Network;
using AudioClient.Utility;
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
    private readonly int _speed;
    private Task? _loop;

    private const int Increment = 25;

    // colours start from freq 0 and jumps in x increments
    // TODO change the colour palette based on the speed of the music
    /*
     * Deep Blue: This color evokes feelings of calm and tranquility, making it a great choice for slow, mellow music.

    Electric Purple: This color is bold and vibrant, making it a great choice for upbeat, high-energy music.

    Forest Green: This color is earthy and grounding, making it a great choice for music with a more organic or natural feel.

    Bright Yellow: This color is cheerful and uplifting, making it a great choice for happy, positive music.

    Burgundy Red: This color is rich and intense, making it a great choice for dramatic or emotional music.

    Neutral gray: This color is versatile and can work well with a variety of music styles and frequencies.
     */
    private readonly Color[] _colors = new Color[]
    {
        ColourConvert.NormaliseColour(Color.Red),
        ColourConvert.NormaliseColour(Color.DarkRed),
        ColourConvert.NormaliseColour(Color.Purple),
        ColourConvert.NormaliseColour(Color.Salmon),
        ColourConvert.NormaliseColour(Color.Crimson),
        ColourConvert.NormaliseColour(Color.Navy),
        ColourConvert.NormaliseColour(Color.Blue),
        ColourConvert.NormaliseColour(Color.BlueViolet),
        ColourConvert.NormaliseColour(Color.SkyBlue),
        ColourConvert.NormaliseColour(Color.DeepSkyBlue),
        ColourConvert.NormaliseColour(Color.HotPink),
        ColourConvert.NormaliseColour(Color.DeepPink),
        ColourConvert.NormaliseColour(Color.Fuchsia),
        ColourConvert.NormaliseColour(Color.Yellow),
        ColourConvert.NormaliseColour(Color.Yellow),
        ColourConvert.NormaliseColour(Color.Yellow),
        ColourConvert.NormaliseColour(Color.Yellow),
        ColourConvert.NormaliseColour(Color.Yellow),
        ColourConvert.NormaliseColour(Color.Red),
        ColourConvert.NormaliseColour(Color.Red),
        ColourConvert.NormaliseColour(Color.Red),
        ColourConvert.NormaliseColour(Color.Red),
        ColourConvert.NormaliseColour(Color.Red),
        ColourConvert.NormaliseColour(Color.Red)
    };


    public AudClient(double threshold, string ip, int port, int stripSize = 150, int speed = 15)
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
            if ((magnitude > _dataFft[i])) continue;
            magnitude = _dataFft[i];
            frequency = i * SampleRate / fftPoints;
        }


        if (magnitude > _threshold)
        {
            Console.WriteLine(frequency);
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
            // Widen the frequency band to reduce the flickering? 200 - 300 instead of 100 jumps?
            // assign out of range values

            colourR = calc;
            colourG = calc;
            colourB = calc;
            for (var i = 0; i < _colors.Length; i++)
            {
                var freqRange = i * Increment;
                if (frequency > freqRange || frequency < (i * Increment) / 2) continue;
                if (Program.Debug)
                    Console.WriteLine(_colors[i]);
                colourR = (byte)(_colors[i].R * calc);
                colourB = (byte)(_colors[i].G * calc);
                colourG = (byte)(_colors[i].B * calc);
            }

            var colour = Color.FromArgb(colourR, colourB, colourG);
            //Console.WriteLine($"Colour:{colour}");
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