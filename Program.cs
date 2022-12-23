using System;
using NAudio.Dsp;
using NAudio.Wave;

namespace AudioClient
{
    internal static class Program
    {
        private static double _threshold = 0.01; // this is kinda like volume

        private static void Main(string[] args)
        {
            using (WasapiLoopbackCapture capture = new())
            {
                capture.WaveFormat = new WaveFormat(44100, 16, 1);
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


        static void OnDataAvailable(object sender, WaveInEventArgs e)
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
            for (int i = 0; i < audioData.Length; i++)
            {
                var y = Math.Sqrt(audioData[i].X * audioData[i].X + audioData[i].Y * audioData[i].Y);
                if (y > _threshold)
                {
                    Console.WriteLine(y);
                    // now we want to determine what to send
                    
                }
            }
        }
    }
}